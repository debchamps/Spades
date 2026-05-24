// SpadesNotificationManager.cs
// Schedules three types of notifications:
//
//   [RECURRING]  1st Saturday of each month  at 19:00 local time  — "Game Night" energy
//   [RECURRING]  3rd Sunday  of each month   at 10:00 local time  — Relaxed Sunday morning
//   [LAPSED]     7 days after last app open  at 19:00 local time
//                 ↳ Reset on every launch — fires only after genuine 7-day absence
//
// Self-initialises via [RuntimeInitializeOnLoadMethod] — no scene wiring needed.
// Android: Default style — text and emoji only (no banner image).
// iOS:     Plain text only (rich image support deferred).

using System;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public static class SpadesNotificationManager
{
    // ─── Config ────────────────────────────────────────────────────────────────
    private const string ChannelId    = "spades_reminders";
    private const string ChannelName  = "Spades Reminders";
    private const string SmallIconRes = "notify_icon_small";   // res/drawable-hdpi/  (white-on-transparent)
    // NOTE: CustomSound is not exposed on AndroidNotificationChannel in
    // com.unity.mobile.notifications 2.3.2. The sound file (res/raw/spades_notification.ogg)
    // is bundled and can be wired via Edit > Project Settings > Mobile Notifications if needed.

    private const int FireHourEvening = 19;   // 7:00 PM  — 1st Saturday & lapsed trigger
    private const int FireHourMorning = 10;   // 10:00 AM — 3rd Sunday

    private const string PrefKey1st    = "spades_notif_id_1st";
    private const string PrefKey3rd    = "spades_notif_id_3rd";
    private const string PrefKeyLapsed = "spades_notif_id_lapsed";  // Android: stored int id
    private const string IOSLapsedId   = "spades_lapsed_trigger";   // iOS: fixed string identifier

    private const int LapsedDays = 7;

    // ─── Notification copy ────────────────────────────────────────────────────
    // Saturday evening — game night vibe
    private const string EveningTitle = "Spades ♠  Your Game Night!";
    private const string EveningBody  = "Time to bid! Will you set them tonight? \U0001F319";

    // Sunday morning — relaxed weekend vibe
    private const string MorningTitle = "Good morning! ☕ Ready for Spades? ♠";
    private const string MorningBody  = "Start your Sunday with Spades. Bid bold! \U0001F0CF";

    // Lapsed re-engagement
    private const string LapsedTitle  = "Miss playing Spades? ♠";
    private const string LapsedBody   = "It’s been a week — jump back in for a quick game! \U0001F0CF";

    // ─── Auto-init (no scene wiring needed) ────────────────────────────────────
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnRuntimeInit() => Initialize();

    public static void Initialize()
    {
#if UNITY_ANDROID
        var go = new GameObject("_SpadesNotifInit") { hideFlags = HideFlags.HideAndDontSave };
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<SpadesNotifInitHelper>().Run();
#elif UNITY_IOS
        ScheduleIOS();
#endif
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ANDROID
    // ═══════════════════════════════════════════════════════════════════════════
#if UNITY_ANDROID

    // Helper MonoBehaviour lives just long enough to run one coroutine then destroys itself.
    private sealed class SpadesNotifInitHelper : MonoBehaviour
    {
        public void Run() => StartCoroutine(RequestThenSchedule());

        private System.Collections.IEnumerator RequestThenSchedule()
        {
            // Android 13+ (API 33) requires runtime POST_NOTIFICATIONS permission.
            // On earlier versions Status becomes Allowed immediately.
            // PermissionRequest is not a YieldInstruction — poll until no longer pending.
            var request = new PermissionRequest();
            while (request.Status == PermissionStatus.RequestPending)
                yield return null;

            if (request.Status == PermissionStatus.Allowed)
            {
                RegisterChannel();
                EnsureScheduled();
            }

            Destroy(gameObject);
        }
    }

    /// <summary>Register the Spades notification channel (idempotent — safe to call every launch).</summary>
    private static void RegisterChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id                   = ChannelId,
            Name                 = ChannelName,
            Importance           = Importance.Default,
            Description          = "Bi-monthly Spades game reminders",
            EnableLights         = true,
            EnableVibration      = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            CanShowBadge         = true,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    /// <summary>
    /// Schedules the next 1st-Saturday (19:00) and 3rd-Sunday (10:00) notifications
    /// if not already pending, then resets the rolling 7-day lapsed trigger.
    /// Safe to call on every app launch.
    /// </summary>
    private static void EnsureScheduled()
    {
        var now = DateTime.Now;
        Debug.Log($"[Spades] EnsureScheduled called at {now:ddd dd-MMM-yyyy HH:mm:ss}");

        // 1st Saturday of month at 19:00 — game night
        ScheduleIfNeeded(PrefKey1st,
            NextOccurrence(now, DayOfWeek.Saturday, weekNumber: 1, hour: FireHourEvening),
            EveningTitle, EveningBody);

        // 3rd Sunday of month at 10:00 — relaxed morning
        ScheduleIfNeeded(PrefKey3rd,
            NextOccurrence(now, DayOfWeek.Sunday, weekNumber: 3, hour: FireHourMorning),
            MorningTitle, MorningBody);

        // Rolling 7-day lapsed trigger — always pushed forward from NOW
        RescheduleLapsed(now);

        PlayerPrefs.Save();
        Debug.Log($"[Spades] All notifications scheduled successfully.");
    }

    /// <summary>
    /// Cancels any existing lapsed notification and schedules a fresh one for
    /// <see cref="LapsedDays"/> days from <paramref name="now"/> at <see cref="FireHourEvening"/>:00.
    /// Called on every app launch so the timer resets to "7 days from today".
    /// </summary>
    private static void RescheduleLapsed(DateTime now)
    {
        int oldId = PlayerPrefs.GetInt(PrefKeyLapsed, -1);
        if (oldId != -1)
            AndroidNotificationCenter.CancelNotification(oldId);

        var fireTime = now.Date.AddDays(LapsedDays).AddHours(FireHourEvening);

        var notification = new AndroidNotification
        {
            Title            = LapsedTitle,
            Text             = LapsedBody,
            SmallIcon        = SmallIconRes,
            FireTime         = fireTime,
            ShouldAutoCancel = true,
        };

        int newId = AndroidNotificationCenter.SendNotification(notification, ChannelId);
        PlayerPrefs.SetInt(PrefKeyLapsed, newId);
        Debug.Log($"[Spades] Lapsed trigger reset → {fireTime:ddd dd-MMM HH:mm}  (id={newId})");
    }

    private static void ScheduleIfNeeded(string prefKey, DateTime fireTime,
                                          string title, string body)
    {
        int savedId = PlayerPrefs.GetInt(prefKey, -1);

        if (savedId != -1)
        {
            var status = AndroidNotificationCenter.CheckScheduledNotificationStatus(savedId);
            if (status == NotificationStatus.Scheduled)
                return; // Already waiting — nothing to do

            AndroidNotificationCenter.CancelNotification(savedId);
        }

        var notification = new AndroidNotification
        {
            Title            = title,
            Text             = body,
            SmallIcon        = SmallIconRes,
            FireTime         = fireTime,
            ShouldAutoCancel = true,
        };

        int newId = AndroidNotificationCenter.SendNotification(notification, ChannelId);
        PlayerPrefs.SetInt(prefKey, newId);
        Debug.Log($"[Spades] Notification scheduled → {fireTime:ddd dd-MMM HH:mm}  (id={newId})");
    }

    /// <summary>
    /// Fires a test notification after <paramref name="delayMinutes"/> minutes (default 1).
    /// Uses the real Saturday-evening title/body so you see exactly what users will see.
    /// Delete SpadesNotifTest.cs before release — this method can stay.
    /// </summary>
    public static void SendTestNotification(int delayMinutes = 1)
    {
        var fireTime = DateTime.Now.AddMinutes(delayMinutes);
        var notification = new AndroidNotification
        {
            Title            = EveningTitle,
            Text             = EveningBody,
            SmallIcon        = SmallIconRes,
            FireTime         = fireTime,
            ShouldAutoCancel = true,
        };
        int id = AndroidNotificationCenter.SendNotification(notification, ChannelId);
        Debug.Log($"[Spades] TEST notification scheduled → {fireTime:HH:mm:ss}  (id={id})");
    }

    /// <summary>
    /// Returns the next occurrence of the Nth weekday of any month strictly after
    /// <paramref name="after"/>, firing at <paramref name="hour"/>:00 local time.
    ///   weekNumber = 1 → 1st occurrence of that weekday in the month
    ///   weekNumber = 3 → 3rd occurrence  (= 1st + 14 days)
    /// </summary>
    private static DateTime NextOccurrence(DateTime after, DayOfWeek dayOfWeek,
                                            int weekNumber, int hour)
    {
        int year = after.Year, month = after.Month;

        while (true)
        {
            var firstOfMonth = new DateTime(year, month, 1);
            int daysTo       = ((int)dayOfWeek - (int)firstOfMonth.DayOfWeek + 7) % 7;
            var target       = firstOfMonth
                                   .AddDays(daysTo + (weekNumber - 1) * 7)
                                   .AddHours(hour);

            if (target > after)
                return target;

            if (++month > 12) { month = 1; year++; }
        }
    }

#endif // UNITY_ANDROID

    // ═══════════════════════════════════════════════════════════════════════════
    // iOS  (text-only for now — rich image support via Notification Service
    //       Extension to be added in a future iteration)
    // ═══════════════════════════════════════════════════════════════════════════
#if UNITY_IOS

    private static void ScheduleIOS()
    {
        iOSNotificationCenter.RequestAuthorization(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
            (approved, _) =>
            {
                if (!approved) return;

                iOSNotificationCenter.RemoveAllScheduledNotifications();
                iOSNotificationCenter.ApplicationBadge = 0;

                var now = DateTime.Now;

                // Pre-schedule 12 months ahead (iOS limit is 64 pending notifications)
                for (int i = 0; i < 12; i++)
                {
                    int y = now.Year, m = now.Month + i;
                    while (m > 12) { m -= 12; y++; }

                    // 1st Saturday at 19:00
                    var firstSat = FirstOccurrenceOf(DayOfWeek.Saturday, y, m, FireHourEvening);
                    // 3rd Sunday at 10:00  (1st Sunday + 14 days)
                    var thirdSun = FirstOccurrenceOf(DayOfWeek.Sunday, y, m, FireHourMorning)
                                       .AddDays(14);

                    if (firstSat > now) SendIOSNotification(firstSat, EveningTitle, EveningBody,
                                                             IOSLapsedId + "_sat_" + i);
                    if (thirdSun > now) SendIOSNotification(thirdSun, MorningTitle, MorningBody,
                                                             IOSLapsedId + "_sun_" + i);
                }

                RescheduleIOSLapsed();
            });
    }

    private static void RescheduleIOSLapsed()
    {
        iOSNotificationCenter.RemoveScheduledNotification(IOSLapsedId);

        var trigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = TimeSpan.FromDays(LapsedDays),
            Repeats      = false,
        };

        var notification = new iOSNotification
        {
            Identifier                   = IOSLapsedId,
            Title                        = LapsedTitle,
            Body                         = LapsedBody,
            Badge                        = 1,
            ShowInForeground             = false,
            ForegroundPresentationOption = PresentationOption.None,
            CategoryIdentifier           = "spades_reminder",
            Trigger                      = trigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }

    private static void SendIOSNotification(DateTime fireTime, string title, string body,
                                             string identifier)
    {
        var trigger = new iOSNotificationCalendarTrigger
        {
            Year    = fireTime.Year,
            Month   = fireTime.Month,
            Day     = fireTime.Day,
            Hour    = fireTime.Hour,
            Minute  = 0,
            Second  = 0,
            Repeats = false,
        };

        var notification = new iOSNotification
        {
            Identifier                   = identifier,
            Title                        = title,
            Body                         = body,
            Badge                        = 1,
            ShowInForeground             = false,
            ForegroundPresentationOption = PresentationOption.None,
            CategoryIdentifier           = "spades_reminder",
            Trigger                      = trigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }

    /// <summary>Returns the 1st occurrence of <paramref name="day"/> in the given month at <paramref name="hour"/>:00.</summary>
    private static DateTime FirstOccurrenceOf(DayOfWeek day, int year, int month, int hour)
    {
        var first  = new DateTime(year, month, 1);
        int daysTo = ((int)day - (int)first.DayOfWeek + 7) % 7;
        return first.AddDays(daysTo).AddHours(hour);
    }

#endif // UNITY_IOS
}
