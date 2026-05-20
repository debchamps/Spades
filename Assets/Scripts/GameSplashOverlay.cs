// ════════════════════════════════════════════════════════════════════════════
//  GameSplashOverlay  —  v2  •  SPADES
//  Reusable premium splash screen system for all DatGames card games.
//
//  HOW TO REUSE FOR ANOTHER GAME:
//    1. Copy this file into the other game's Scripts folder.
//    2. Change everything inside the ── PER-GAME CONFIG ── block below.
//    3. Copy MaldiniBold.ttf into that game's Assets/Resources/fonts/
//    4. Done — no scene wiring, no prefabs needed.
// ════════════════════════════════════════════════════════════════════════════

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameSplashOverlay : MonoBehaviour
{
    // ════════════════════════════════════════════════════════════════════════
    //  ── PER-GAME CONFIG  (only section that changes between games) ────────
    // ════════════════════════════════════════════════════════════════════════

    private const string GameTitle    = "SPADES";
    private const string GameSubtitle = "Classic Partner Trick-Taking Game";

    // Resources paths (relative to any Resources/ folder, no extension)
    private const string StudioLogoPath = "bray/datgameslogo";   // set null to skip

    // The royal court of spades — Queen, Ace (front/centre), King
    private static readonly string[] FanCardPaths =
        { "Cards/sq", "Cards/sa", "Cards/sk" };

    // Spades is always trump — all four corners are spade icons
    private static readonly string[] CornerSuitPaths =
        { "Cards/spadeicon", "Cards/spadeicon", "Cards/spadeicon", "Cards/spadeicon" };

    // Background gradient  (top → mid → bottom, portrait screen)
    private static readonly Color GradTop = new Color(0.02f, 0.02f, 0.04f); // near-black
    private static readonly Color GradMid = new Color(0.06f, 0.06f, 0.10f); // very dark blue-black
    private static readonly Color GradBtm = new Color(0.08f, 0.08f, 0.16f); // dark midnight navy

    private static readonly Color TitleColor    = new Color(1.00f, 0.86f, 0.20f); // gold
    private static readonly Color SubtitleColor = new Color(0.92f, 0.96f, 1.00f); // near-white
    private static readonly Color AccentColor   = new Color(0.82f, 0.15f, 0.18f); // red pops on near-black

    // ════════════════════════════════════════════════════════════════════════
    //  ── TIMING (shared across all games, tweak if needed) ────────────────
    // ════════════════════════════════════════════════════════════════════════

    private const float HoldDuration    = 1.55f;
    private const float FadeOutDuration = 0.40f;

    private const float T_BG       = 0.00f;
    private const float T_CORNERS  = 0.10f;
    private const float T_CARD1    = 0.18f;
    private const float T_CARD2    = 0.30f;
    private const float T_CARD3    = 0.42f;
    private const float T_TITLE    = 0.58f;
    private const float T_SUBTITLE = 0.76f;
    private const float T_LINE     = 0.88f;
    private const float T_STUDIO   = 1.00f;

    // ════════════════════════════════════════════════════════════════════════
    //  ── SYSTEM — do not edit below this line ─────────────────────────────
    // ════════════════════════════════════════════════════════════════════════

    private static bool s_HasShown;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState() => s_HasShown = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ShowOnFirstScene()
    {
        if (s_HasShown) return;
        s_HasShown = true;
        var root = new GameObject("GameSplashOverlay") { hideFlags = HideFlags.HideAndDontSave };
        DontDestroyOnLoad(root);
        root.AddComponent<GameSplashOverlay>();
    }

    private const float CanvasW       = 1080f;
    private const float CanvasH       = 1920f;
    private const float CardW         = 190f;
    private const float CardH         = 266f;
    private const float CardSlideFrom = 420f;
    private const float TitleSlideAmt = 40f;
    private const float CornerSize    = 210f;

    private void Awake()
    {
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvasGO.AddComponent<GraphicRaycaster>();
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(CanvasW, CanvasH);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;
        var root = canvasGO.transform;
        StartCoroutine(RunAnimation(
            BuildBackground(root), BuildVignette(root), BuildCornerSuits(root),
            BuildCardFan(root), BuildTitle(root), BuildSubtitle(root),
            BuildSeparatorLine(root), BuildStudioLogo(root)));
    }

    private static Image BuildBackground(Transform p)
    {
        var go = new GameObject("BG"); go.transform.SetParent(p, false);
        Stretch(go.AddComponent<RectTransform>());
        var img = go.AddComponent<Image>();
        img.sprite = MakeGradientSprite(GradTop, GradMid, GradBtm); img.color = new Color(1,1,1,0);
        return img;
    }

    private static Image BuildVignette(Transform p)
    {
        var go = new GameObject("Vignette"); go.transform.SetParent(p, false);
        Stretch(go.AddComponent<RectTransform>());
        var img = go.AddComponent<Image>();
        img.sprite = MakeVignetteSprite(96); img.color = new Color(1,1,1,0);
        return img;
    }

    private static Image[] BuildCornerSuits(Transform p)
    {
        var res = new Image[4];
        var pos = new Vector2[] {
            new Vector2(-440f,840f), new Vector2(440f,840f),
            new Vector2(-440f,-840f), new Vector2(440f,-840f) };
        var rot = new float[] { -15f, 25f, 20f, -20f };
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject("CS_"+i); go.transform.SetParent(p, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
            r.sizeDelta = new Vector2(CornerSize, CornerSize); r.anchoredPosition = pos[i];
            go.transform.localRotation = Quaternion.Euler(0, 0, rot[i]);
            var img = go.AddComponent<Image>();
            var s = i < CornerSuitPaths.Length ? Resources.Load<Sprite>(CornerSuitPaths[i]) : null;
            if (s != null) { img.sprite = s; img.preserveAspect = true; }
            img.color = new Color(AccentColor.r, AccentColor.g, AccentColor.b, 0f); res[i] = img;
        }
        return res;
    }

    private static Image[] BuildCardFan(Transform p)
    {
        var layout = new (Vector2 pos, float rot, float sc)[] {
            (new Vector2(-118f, 155f), -21f, 0.92f),
            (new Vector2(8f, 215f), -4f, 1.00f),
            (new Vector2(124f, 148f), 19f, 0.92f) };
        var imgs = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var (pos, rot, sc) = layout[i];
            var sg = new GameObject("CardShadow_"+i); sg.transform.SetParent(p, false);
            var sr = sg.AddComponent<RectTransform>();
            sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(0.5f, 0.5f);
            sr.sizeDelta = new Vector2(CardW*sc+6, CardH*sc+6); sr.anchoredPosition = new Vector2(pos.x+10f, pos.y-14f);
            sg.transform.localRotation = Quaternion.Euler(0, 0, rot);
            var si = sg.AddComponent<Image>(); si.color = new Color(0,0,0,0); si.sprite = MakeRoundedRectSprite(8);
            var cg = new GameObject("Card_"+i); cg.transform.SetParent(p, false);
            var cr = cg.AddComponent<RectTransform>();
            cr.anchorMin = cr.anchorMax = cr.pivot = new Vector2(0.5f, 0.5f);
            cr.sizeDelta = new Vector2(CardW*sc, CardH*sc); cr.anchoredPosition = pos;
            cg.transform.localRotation = Quaternion.Euler(0, 0, rot);
            var ci = cg.AddComponent<Image>();
            if (i < FanCardPaths.Length) { var s = Resources.Load<Sprite>(FanCardPaths[i]); if (s!=null){ci.sprite=s;ci.preserveAspect=true;} }
            ci.color = new Color(1,1,1,0); imgs[i] = ci;
            cg.name = "Card_"+i+"__shadow__"+sg.GetInstanceID();
        }
        return imgs;
    }

    private static (Text, Text) BuildTitle(Transform p)
    {
        var f = LoadFont();
        var sg = new GameObject("TitleShadow"); sg.transform.SetParent(p, false);
        var sr = sg.AddComponent<RectTransform>();
        sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(0.5f, 0.5f);
        sr.sizeDelta = new Vector2(900f, 160f); sr.anchoredPosition = new Vector2(4f, -85f);
        var st = sg.AddComponent<Text>();
        ApplyText(st, GameTitle, f, 108, FontStyle.Bold, new Color(0,0,0,0), TextAnchor.MiddleCenter);
        var go = new GameObject("Title"); go.transform.SetParent(p, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(900f, 160f); r.anchoredPosition = new Vector2(0f, -81f);
        var t = go.AddComponent<Text>();
        ApplyText(t, GameTitle, f, 108, FontStyle.Bold, new Color(TitleColor.r,TitleColor.g,TitleColor.b,0f), TextAnchor.MiddleCenter);
        return (t, st);
    }

    private static (Text, Text) BuildSubtitle(Transform p)
    {
        var f = LoadFont();
        var sg = new GameObject("SubtitleShadow"); sg.transform.SetParent(p, false);
        var sr = sg.AddComponent<RectTransform>();
        sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(0.5f, 0.5f);
        sr.sizeDelta = new Vector2(860f, 80f); sr.anchoredPosition = new Vector2(3f, -213f);
        var st = sg.AddComponent<Text>();
        ApplyText(st, GameSubtitle, f, 44, FontStyle.Normal, new Color(0,0,0,0), TextAnchor.MiddleCenter);
        var go = new GameObject("Subtitle"); go.transform.SetParent(p, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(860f, 80f); r.anchoredPosition = new Vector2(0f, -210f);
        var t = go.AddComponent<Text>();
        ApplyText(t, GameSubtitle, f, 44, FontStyle.Normal, new Color(SubtitleColor.r,SubtitleColor.g,SubtitleColor.b,0f), TextAnchor.MiddleCenter);
        return (t, st);
    }

    private static Image BuildSeparatorLine(Transform p)
    {
        var go = new GameObject("Separator"); go.transform.SetParent(p, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(0f, 3f); r.anchoredPosition = new Vector2(0f, -290f);
        var img = go.AddComponent<Image>(); img.color = new Color(TitleColor.r,TitleColor.g,TitleColor.b,0f);
        return img;
    }

    private static Image BuildStudioLogo(Transform p)
    {
        if (string.IsNullOrEmpty(StudioLogoPath)) return null;
        var go = new GameObject("StudioLogo"); go.transform.SetParent(p, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(340f, 90f); r.anchoredPosition = new Vector2(0f, -780f);
        var img = go.AddComponent<Image>();
        var logo = Resources.Load<Sprite>(StudioLogoPath);
        if (logo != null) { img.sprite = logo; img.preserveAspect = true; }
        img.color = new Color(1,1,1,0); return img;
    }

    private IEnumerator RunAnimation(
        Image bgImg, Image vigImg, Image[] cornerImgs, Image[] cardImgs,
        (Text title, Text shadow) titleGroup, (Text sub, Text shadow) subGroup,
        Image lineImg, Image studioImg)
    {
        const float SHA = 0.55f, CA = 0.07f, VA = 0.72f;
        float elapsed = 0f;
        var cardShadows = new Image[cardImgs.Length];
        for (int i = 0; i < cardImgs.Length; i++)
        {
            var s = cardImgs[i].transform.parent.Find("CardShadow_"+i);
            if (s != null) cardShadows[i] = s.GetComponent<Image>();
        }
        var cfp = new Vector2[cardImgs.Length]; var crt = new RectTransform[cardImgs.Length]; var srt = new RectTransform[cardImgs.Length];
        for (int i = 0; i < cardImgs.Length; i++)
        {
            crt[i] = cardImgs[i].GetComponent<RectTransform>(); cfp[i] = crt[i].anchoredPosition;
            crt[i].anchoredPosition = cfp[i] + Vector2.down * CardSlideFrom;
            if (cardShadows[i] != null) { srt[i] = cardShadows[i].GetComponent<RectTransform>(); srt[i].anchoredPosition += Vector2.down * CardSlideFrom; }
        }
        var trt  = titleGroup.title.GetComponent<RectTransform>(); var tsrt = titleGroup.shadow.GetComponent<RectTransform>(); float tfy = trt.anchoredPosition.y;
        var srt2 = subGroup.sub.GetComponent<RectTransform>();     var ssrt = subGroup.shadow.GetComponent<RectTransform>();   float sfy = srt2.anchoredPosition.y;
        trt.anchoredPosition  += Vector2.down*TitleSlideAmt; tsrt.anchoredPosition += Vector2.down*TitleSlideAmt;
        srt2.anchoredPosition += Vector2.down*TitleSlideAmt; ssrt.anchoredPosition += Vector2.down*TitleSlideAmt;
        float holdEnd = T_STUDIO+0.25f+HoldDuration, totalTime = holdEnd+FadeOutDuration;
        while (elapsed < totalTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float bgT = InvLerp(T_BG,T_BG+0.30f,elapsed);
            bgImg.color = new Color(1,1,1,bgT); vigImg.color = new Color(1,1,1,bgT*VA);
            float cT = EaseOutCubic(InvLerp(T_CORNERS,T_CORNERS+0.55f,elapsed));
            foreach (var c in cornerImgs) if(c!=null) c.color=new Color(AccentColor.r,AccentColor.g,AccentColor.b,cT*CA);
            for (int i = 0; i < cardImgs.Length; i++)
            {
                float t0=i==0?T_CARD1:i==1?T_CARD2:T_CARD3; float ct=EaseOutCubic(InvLerp(t0,t0+0.26f,elapsed));
                var np=new Vector2(cfp[i].x,Mathf.Lerp(cfp[i].y-CardSlideFrom,cfp[i].y,ct));
                crt[i].anchoredPosition=np; cardImgs[i].color=new Color(1,1,1,ct);
                if(cardShadows[i]!=null){srt[i].anchoredPosition=new Vector2(np.x+10f,np.y-14f);cardShadows[i].color=new Color(0,0,0,ct*SHA);}
            }
            float tt=EaseOutCubic(InvLerp(T_TITLE,T_TITLE+0.28f,elapsed)),ty=Mathf.Lerp(tfy-TitleSlideAmt,tfy,tt);
            trt.anchoredPosition=new Vector2(0f,ty); tsrt.anchoredPosition=new Vector2(4f,ty-4f);
            titleGroup.title.color=new Color(TitleColor.r,TitleColor.g,TitleColor.b,tt); titleGroup.shadow.color=new Color(0,0,0,tt*SHA);
            float st=EaseOutCubic(InvLerp(T_SUBTITLE,T_SUBTITLE+0.24f,elapsed)),sy=Mathf.Lerp(sfy-TitleSlideAmt,sfy,st);
            srt2.anchoredPosition=new Vector2(0f,sy); ssrt.anchoredPosition=new Vector2(3f,sy-3f);
            subGroup.sub.color=new Color(SubtitleColor.r,SubtitleColor.g,SubtitleColor.b,st); subGroup.shadow.color=new Color(0,0,0,st*SHA*0.5f);
            if(lineImg!=null){float lt=EaseOutCubic(InvLerp(T_LINE,T_LINE+0.30f,elapsed));lineImg.GetComponent<RectTransform>().sizeDelta=new Vector2(Mathf.Lerp(0f,480f,lt),3f);lineImg.color=new Color(TitleColor.r,TitleColor.g,TitleColor.b,lt*0.70f);}
            if(studioImg!=null){float slt=EaseOutCubic(InvLerp(T_STUDIO,T_STUDIO+0.22f,elapsed));studioImg.color=new Color(1,1,1,slt*0.80f);}
            if(elapsed>=holdEnd){float a=1f-InvLerp(holdEnd,totalTime,elapsed);
                bgImg.color=new Color(1,1,1,a);vigImg.color=new Color(1,1,1,a*VA);
                foreach(var c in cornerImgs)if(c!=null)c.color=new Color(AccentColor.r,AccentColor.g,AccentColor.b,a*CA);
                for(int i=0;i<cardImgs.Length;i++){cardImgs[i].color=new Color(1,1,1,a);if(cardShadows[i]!=null)cardShadows[i].color=new Color(0,0,0,a*SHA);}
                titleGroup.title.color=new Color(TitleColor.r,TitleColor.g,TitleColor.b,a);titleGroup.shadow.color=new Color(0,0,0,a*SHA);
                subGroup.sub.color=new Color(SubtitleColor.r,SubtitleColor.g,SubtitleColor.b,a);subGroup.shadow.color=new Color(0,0,0,a*SHA*0.5f);
                if(lineImg!=null)lineImg.color=new Color(TitleColor.r,TitleColor.g,TitleColor.b,a*0.70f);
                if(studioImg!=null)studioImg.color=new Color(1,1,1,a*0.80f);}
            yield return null;
        }
        Destroy(gameObject);
    }

    private static Sprite MakeGradientSprite(Color top, Color mid, Color bot)
    {
        const int H=256; var tex=new Texture2D(2,H,TextureFormat.RGBA32,false){wrapMode=TextureWrapMode.Clamp,filterMode=FilterMode.Bilinear};
        for(int y=0;y<H;y++){float t=y/(float)(H-1);var c=t<0.45f?Color.Lerp(top,mid,t/0.45f):Color.Lerp(mid,bot,(t-0.45f)/0.55f);tex.SetPixel(0,y,c);tex.SetPixel(1,y,c);}
        tex.Apply(false,true); return Sprite.Create(tex,new Rect(0,0,2,H),new Vector2(0.5f,0.5f),100f);
    }
    private static Sprite MakeVignetteSprite(int sz=96)
    {
        var tex=new Texture2D(sz,sz,TextureFormat.RGBA32,false){wrapMode=TextureWrapMode.Clamp,filterMode=FilterMode.Bilinear};
        var c=new Vector2(sz*0.5f,sz*0.5f);float h=sz*0.5f;
        for(int y=0;y<sz;y++)for(int x=0;x<sz;x++){float d=Vector2.Distance(new Vector2(x,y),c)/h;tex.SetPixel(x,y,new Color(0,0,0,Mathf.Pow(Mathf.Clamp01(d),1.6f)*0.78f));}
        tex.Apply(false,true); return Sprite.Create(tex,new Rect(0,0,sz,sz),new Vector2(0.5f,0.5f),100f);
    }
    private static Sprite MakeRoundedRectSprite(int r=8)
    {
        const int S=64; var tex=new Texture2D(S,S,TextureFormat.RGBA32,false); var c=new Vector2(S*0.5f,S*0.5f);
        for(int y=0;y<S;y++)for(int x=0;x<S;x++){float cx=Mathf.Max(Mathf.Abs(x-c.x)-(c.x-r),0),cy=Mathf.Max(Mathf.Abs(y-c.y)-(c.y-r),0);tex.SetPixel(x,y,new Color(1,1,1,1f-Mathf.Clamp01(Mathf.Sqrt(cx*cx+cy*cy)-r+1)));}
        tex.Apply(false,true); return Sprite.Create(tex,new Rect(0,0,S,S),new Vector2(0.5f,0.5f),100f);
    }
    private static Font LoadFont(){var f=Resources.Load<Font>("fonts/MaldiniBold");return f!=null?f:Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");}
    private static void ApplyText(Text t,string v,Font f,int sz,FontStyle fs,Color c,TextAnchor a){t.font=f;t.text=v;t.fontSize=sz;t.fontStyle=fs;t.color=c;t.alignment=a;t.resizeTextForBestFit=true;t.resizeTextMinSize=18;t.resizeTextMaxSize=sz+8;}
    private static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;}
    private static float InvLerp(float a,float b,float v)=>Mathf.Clamp01((v-a)/(b-a));
    private static float EaseOutCubic(float t)=>1f-Mathf.Pow(1f-t,3f);
}
