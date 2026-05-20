using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

/// <summary>
/// Reusable system that injects a text-based content view into an existing
/// "howtoplay" panel at runtime, replacing the legacy image-only display.
///
/// Usage (from HowToPlayScript):
///   1. Load content once:
///        _helpContent = HelpPopupManager.LoadContent("Localization/callbreak_help");
///   2. When panel opens, call:
///        HelpPopupManager.Ensure(howtoplayGO);
///   3. Every time the page changes, call:
///        HelpPopupManager.RenderPage(howtoplayGO, pageNumber, totalPages, _helpContent);
/// </summary>
public static class HelpPopupManager
{
    // ── GameObject name constants ───────────────────────────────────────────
    private const string LEGACY_IMAGE_NAME = "helpimage";
    private const string TEXT_VIEW_NAME    = "HelpTextView";
    private const string INDICATOR_NAME    = "HelpPageIndicator";

    // ── Visual style ────────────────────────────────────────────────────────
    private static readonly Color CARD_BG        = new Color(0.22f, 0.48f, 0.70f, 0.95f);
    private static readonly Color TEXT_COLOR      = new Color(0.96f, 0.98f, 1.00f, 1.00f);
    private static readonly Color INDICATOR_COLOR = new Color(1.00f, 1.00f, 1.00f, 0.90f);
    private const float  FONT_SIZE_BODY      = 21f;
    private const float  FONT_SIZE_INDICATOR = 19f;

    // ════════════════════════════════════════════════════════════════════════
    // Public API
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loads a HelpContent object from a JSON TextAsset in Resources.
    /// Returns null and logs a warning if the asset is missing or malformed.
    /// </summary>
    public static HelpContent LoadContent(string resourcePath)
    {
        TextAsset asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogWarning("[HelpPopupManager] Help content not found at Resources/" + resourcePath);
            return null;
        }
        try
        {
            return JsonConvert.DeserializeObject<HelpContent>(asset.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[HelpPopupManager] Failed to parse help JSON: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Idempotent setup: hides the legacy helpimage, creates the TMP text
    /// view and page indicator if they don't already exist.
    /// Call once when the popup opens (safe to call every time).
    /// </summary>
    public static void Ensure(GameObject panel)
    {
        if (panel == null) return;

        // Hide the legacy image slot
        Transform legacyImg = panel.transform.Find(LEGACY_IMAGE_NAME);
        if (legacyImg != null) legacyImg.gameObject.SetActive(false);

        // Build text view once
        if (panel.transform.Find(TEXT_VIEW_NAME) == null)
            BuildTextView(panel);

        // Build page indicator once
        if (panel.transform.Find(INDICATOR_NAME) == null)
            BuildIndicator(panel);
    }

    /// <summary>
    /// Renders the specified page (1-based) from helpContent into the panel,
    /// with a smooth crossfade animation.
    /// </summary>
    public static void RenderPage(GameObject panel, int pageNumber, int totalPages,
                                  HelpContent helpContent)
    {
        if (panel == null) return;

        int idx = pageNumber - 1;
        bool hasContent = (helpContent != null && idx >= 0 && idx < helpContent.pages.Count);

        Transform viewT = panel.transform.Find(TEXT_VIEW_NAME);
        if (viewT == null) { Ensure(panel); viewT = panel.transform.Find(TEXT_VIEW_NAME); }
        if (viewT == null) return;

        CanvasGroup cg = viewT.GetComponent<CanvasGroup>();
        if (cg == null) cg = viewT.gameObject.AddComponent<CanvasGroup>();

        TMP_Text bodyTmp = viewT.GetComponentInChildren<TMP_Text>();

        if (!hasContent)
        {
            // Fall back to legacy image display
            viewT.gameObject.SetActive(false);
            Transform legacyImg = panel.transform.Find(LEGACY_IMAGE_NAME);
            if (legacyImg != null)
            {
                legacyImg.gameObject.SetActive(true);
                Sprite spr = Resources.Load<Sprite>("howtoplay/help" + pageNumber);
                if (spr != null) legacyImg.GetComponent<Image>().sprite = spr;
            }
            UpdateIndicator(panel, pageNumber, totalPages);
            return;
        }

        HelpPage page = helpContent.pages[idx];

        // For "image" pages, defer to legacy sprite rendering
        if (page.contentType == "image" && !string.IsNullOrEmpty(page.imagePath))
        {
            viewT.gameObject.SetActive(false);
            Transform legacyImg = panel.transform.Find(LEGACY_IMAGE_NAME);
            if (legacyImg != null)
            {
                legacyImg.gameObject.SetActive(true);
                Sprite spr = Resources.Load<Sprite>(page.imagePath);
                if (spr != null) legacyImg.GetComponent<Image>().sprite = spr;
            }
            UpdateIndicator(panel, pageNumber, totalPages);
            return;
        }

        // Text / mixed mode — show the TMP view
        viewT.gameObject.SetActive(true);
        Transform legacyImgT = panel.transform.Find(LEGACY_IMAGE_NAME);
        if (legacyImgT != null) legacyImgT.gameObject.SetActive(false);

        // Fetch localised body
        string bodyText = string.IsNullOrEmpty(page.bodyKey)
            ? ""
            : LocalizationManager.Instance.Get(page.bodyKey);

        // Crossfade: fade out → swap text → fade in
        DOTween.Kill(viewT.gameObject);
        DOTween.To(() => cg.alpha, v => cg.alpha = v, 0f, 0.15f)
            .SetTarget(viewT.gameObject)
            .OnComplete(() =>
            {
                if (bodyTmp != null) bodyTmp.text = bodyText;
                UpdateIndicator(panel, pageNumber, totalPages);
                DOTween.To(() => cg.alpha, v => cg.alpha = v, 1f, 0.20f)
                    .SetTarget(viewT.gameObject);
            });
    }

    // ════════════════════════════════════════════════════════════════════════
    // Builders (called once per panel lifetime)
    // ════════════════════════════════════════════════════════════════════════

    private static void BuildTextView(GameObject panel)
    {
        // Borrow TMP font from an existing scene TMP_Text so non-Latin scripts
        // (Devanagari, Bengali, Arabic, etc.) render correctly instead of □□□□.
        // Must be resolved BEFORE AddComponent<TextMeshProUGUI>() to avoid
        // accidentally returning our own newly created component.
        TMP_Text sceneRef = Object.FindObjectOfType<TMP_Text>();

        // ── Card container ────────────────────────────────────────────────
        GameObject viewGO = new GameObject(TEXT_VIEW_NAME);
        viewGO.transform.SetParent(panel.transform, false);

        RectTransform viewRT = viewGO.AddComponent<RectTransform>();
        // Occupy the same region as the legacy helpimage (centre of panel,
        // between header and nav arrows).
        viewRT.anchorMin        = new Vector2(0.03f, 0.20f);
        viewRT.anchorMax        = new Vector2(0.97f, 0.88f);
        viewRT.offsetMin        = Vector2.zero;
        viewRT.offsetMax        = Vector2.zero;

        Image bg   = viewGO.AddComponent<Image>();
        bg.color   = CARD_BG;

        CanvasGroup cg  = viewGO.AddComponent<CanvasGroup>();
        cg.alpha        = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // ── Body TMP text ─────────────────────────────────────────────────
        GameObject textGO = new GameObject("BodyText");
        textGO.transform.SetParent(viewGO.transform, false);

        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(18f, 14f);
        textRT.offsetMax = new Vector2(-18f, -14f);

        TextMeshProUGUI tmp  = textGO.AddComponent<TextMeshProUGUI>();
        tmp.enableWordWrapping = true;
        tmp.overflowMode       = TextOverflowModes.Overflow;
        tmp.fontSize           = FONT_SIZE_BODY;
        tmp.color              = TEXT_COLOR;
        tmp.richText           = true;
        tmp.alignment          = TextAlignmentOptions.TopLeft;
        tmp.text               = "";
        if (sceneRef != null) tmp.font = sceneRef.font;
    }

    private static void BuildIndicator(GameObject panel)
    {
        // Reuse BodyText TMP font (set by BuildTextView) or fall back to scene search.
        TMP_Text fontSrc = panel.GetComponentInChildren<TMP_Text>()
                        ?? Object.FindObjectOfType<TMP_Text>();

        GameObject indGO = new GameObject(INDICATOR_NAME);
        indGO.transform.SetParent(panel.transform, false);

        RectTransform indRT = indGO.AddComponent<RectTransform>();
        // Centred horizontally, sits between text view bottom and nav arrows
        indRT.anchorMin        = new Vector2(0.30f, 0.13f);
        indRT.anchorMax        = new Vector2(0.70f, 0.20f);
        indRT.offsetMin        = Vector2.zero;
        indRT.offsetMax        = Vector2.zero;

        TextMeshProUGUI tmp = indGO.AddComponent<TextMeshProUGUI>();
        tmp.fontSize        = FONT_SIZE_INDICATOR;
        tmp.color           = INDICATOR_COLOR;
        tmp.alignment       = TextAlignmentOptions.Center;
        tmp.richText        = false;
        tmp.text            = "";
        if (fontSrc != null) tmp.font = fontSrc.font;
    }

    private static void UpdateIndicator(GameObject panel, int pageNumber, int totalPages)
    {
        Transform indT = panel.transform.Find(INDICATOR_NAME);
        if (indT == null) return;
        TMP_Text tmp = indT.GetComponent<TMP_Text>();
        if (tmp != null) tmp.text = pageNumber + " / " + totalPages;
    }
}
