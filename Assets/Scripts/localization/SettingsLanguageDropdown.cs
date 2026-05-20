using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Programmatically injects a "Language" selector row (label + custom dropdown)
/// into an existing settings panel at runtime.
///
/// Usage: call  SettingsLanguageDropdown.Ensure(GameObject.Find("settingsparent").transform)
///        from SettingsScript.enable().
/// </summary>
public class SettingsLanguageDropdown : MonoBehaviour
{
    // ── GameObject name constants ───────────────────────────────────────────
    private const string ROW_NAME   = "LanguageDropdownRow";
    private const string POPUP_NAME = "LanguagePopupPanel";

    // ── Colours ─────────────────────────────────────────────────────────────
    private static readonly Color BG_BUTTON   = new Color(0.20f, 0.50f, 0.85f, 1.00f);
    private static readonly Color BG_SELECTED = new Color(0.26f, 0.52f, 0.96f, 0.90f);
    private static readonly Color BG_POPUP    = new Color(0.13f, 0.18f, 0.30f, 0.97f);

    // ── Instance state ──────────────────────────────────────────────────────
    private Text       _btnLabel;
    private GameObject _popup;
    private bool       _popupOpen;

    // ════════════════════════════════════════════════════════════════════════
    // Public API
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ensures the language row exists inside <paramref name="settingsParent"/>.
    /// Safe to call every time the settings panel is opened.
    /// </summary>
    public static void Ensure(Transform settingsParent)
    {
        if (!LocalizationManager.ENABLED) return;
        if (settingsParent == null) return;

        Transform existing = settingsParent.Find(ROW_NAME);
        if (existing != null)
        {
            // Refresh the button label in case the language changed elsewhere
            existing.GetComponent<SettingsLanguageDropdown>()?.RefreshLabel();
            return;
        }
        CreateRow(settingsParent);
    }

    // ════════════════════════════════════════════════════════════════════════
    // Row creation
    // ════════════════════════════════════════════════════════════════════════

    private static void CreateRow(Transform parent)
    {
        // ── Borrow visual style from the first Text found in the panel ────────
        Text  refText   = parent.GetComponentInChildren<Text>();
        Font  font      = refText != null ? refText.font      : Resources.GetBuiltinResource<Font>("Arial.ttf");
        Color textColor = Color.white;
        int   fontSize  = Mathf.Clamp(refText != null ? refText.fontSize : 28, 20, 32);

        // ── Row root ──────────────────────────────────────────────────────────
        GameObject rowGO = new GameObject(ROW_NAME);
        rowGO.transform.SetParent(parent, false);

        SettingsLanguageDropdown self = rowGO.AddComponent<SettingsLanguageDropdown>();

        RectTransform rowRT    = rowGO.AddComponent<RectTransform>();
        // Anchored to bottom of panel, ~82 units up (above typical Close button area)
        rowRT.anchorMin        = new Vector2(0.05f, 0f);
        rowRT.anchorMax        = new Vector2(0.95f, 0f);
        rowRT.pivot            = new Vector2(0.5f,  0f);
        rowRT.anchoredPosition = new Vector2(0f, 82f);
        rowRT.sizeDelta        = new Vector2(0f, 46f);

        // ── "Language:" label (left 44%) ──────────────────────────────────────
        GameObject lblGO = new GameObject("LangLabel");
        lblGO.transform.SetParent(rowGO.transform, false);
        RectTransform lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = new Vector2(0f,    0f);
        lblRT.anchorMax = new Vector2(0.44f, 1f);
        lblRT.offsetMin = Vector2.zero;
        lblRT.offsetMax = Vector2.zero;

        Text lblTxt      = lblGO.AddComponent<Text>();
        lblTxt.text      = LocalizationManager.Instance.Get("settings.language");
        lblTxt.font      = font;
        lblTxt.fontSize  = fontSize;
        lblTxt.color     = textColor;
        lblTxt.alignment = TextAnchor.MiddleLeft;

        // ── Dropdown button (right 53%) ────────────────────────────────────────
        GameObject btnGO = new GameObject("LangBtn");
        btnGO.transform.SetParent(rowGO.transform, false);
        RectTransform btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.47f, 0f);
        btnRT.anchorMax = new Vector2(1f,    1f);
        btnRT.offsetMin = Vector2.zero;
        btnRT.offsetMax = Vector2.zero;

        Image  btnImg = btnGO.AddComponent<Image>();
        btnImg.color  = BG_BUTTON;

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock btnColors   = btn.colors;
        btnColors.normalColor  = BG_BUTTON;
        btnColors.highlightedColor = new Color(0.28f, 0.36f, 0.56f, 0.95f);
        btnColors.pressedColor = new Color(0.14f, 0.20f, 0.34f, 0.95f);
        btn.colors             = btnColors;
        btn.targetGraphic      = btnImg;

        // Button label text
        GameObject btnLblGO = new GameObject("BtnText");
        btnLblGO.transform.SetParent(btnGO.transform, false);
        RectTransform btnLblRT = btnLblGO.AddComponent<RectTransform>();
        btnLblRT.anchorMin = Vector2.zero;
        btnLblRT.anchorMax = Vector2.one;
        btnLblRT.offsetMin = new Vector2(6f, 2f);
        btnLblRT.offsetMax = new Vector2(-6f, -2f);

        Text btnLblTxt      = btnLblGO.AddComponent<Text>();
        btnLblTxt.font      = font;
        btnLblTxt.fontSize  = fontSize - 2;
        btnLblTxt.color     = textColor;
        btnLblTxt.alignment = TextAnchor.MiddleCenter;

        self._btnLabel = btnLblTxt;
        self.RefreshLabel();

        // ── Popup panel (built but hidden) ─────────────────────────────────────
        self._popup = CreatePopup(parent, font, fontSize - 2, textColor, self);

        // ── Wire click: toggle popup ───────────────────────────────────────────
        btn.onClick.AddListener(() => self.TogglePopup());
    }

    // ════════════════════════════════════════════════════════════════════════
    // Popup creation
    // ════════════════════════════════════════════════════════════════════════

    private static GameObject CreatePopup(
        Transform parent, Font font, int fontSize, Color textColor,
        SettingsLanguageDropdown owner)
    {
        List<LanguageInfo> langs = LocalizationManager.Instance.GetSupportedLanguages();
        float itemH  = 44f;
        float totalH = langs.Count * itemH + 8f;

        // ── Panel ──────────────────────────────────────────────────────────────
        GameObject panelGO = new GameObject(POPUP_NAME);
        panelGO.transform.SetParent(parent, false);
        panelGO.transform.SetAsLastSibling();   // render on top of siblings

        RectTransform panelRT    = panelGO.AddComponent<RectTransform>();
        panelRT.anchorMin        = new Vector2(0.47f, 0f);
        panelRT.anchorMax        = new Vector2(0.95f, 0f);
        panelRT.pivot            = new Vector2(0.5f,  0f);
        // Position just above the row (row is at y=82, height=46 → popup bottom at y=132)
        panelRT.anchoredPosition = new Vector2(0f, 132f);
        panelRT.sizeDelta        = new Vector2(0f, totalH);

        Image panelImg = panelGO.AddComponent<Image>();
        panelImg.color = BG_POPUP;

        // ── Language option items ──────────────────────────────────────────────
        string currentLang = LocalizationManager.Instance.GetCurrentLanguage();

        for (int i = 0; i < langs.Count; i++)
        {
            LanguageInfo lang     = langs[i];
            bool         selected = lang.code == currentLang;

            GameObject itemGO = new GameObject("Item_" + lang.code);
            itemGO.transform.SetParent(panelGO.transform, false);

            RectTransform itemRT    = itemGO.AddComponent<RectTransform>();
            // Stack items top-to-bottom
            float itemY             = totalH - 4f - (i + 1) * itemH;
            itemRT.anchorMin        = new Vector2(0f, 0f);
            itemRT.anchorMax        = new Vector2(1f, 0f);
            itemRT.pivot            = new Vector2(0.5f, 0f);
            itemRT.anchoredPosition = new Vector2(0f, itemY);
            itemRT.sizeDelta        = new Vector2(-8f, itemH - 2f);

            Image itemBg   = itemGO.AddComponent<Image>();
            itemBg.color   = selected ? BG_SELECTED : Color.clear;

            Button itemBtn = itemGO.AddComponent<Button>();
            ColorBlock cb  = itemBtn.colors;
            cb.normalColor      = selected ? BG_SELECTED : Color.clear;
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.18f);
            cb.selectedColor    = BG_SELECTED;
            cb.pressedColor     = BG_SELECTED;
            itemBtn.colors      = cb;
            itemBtn.targetGraphic = itemBg;

            // Item label
            GameObject itemLblGO = new GameObject("Label");
            itemLblGO.transform.SetParent(itemGO.transform, false);
            RectTransform itemLblRT = itemLblGO.AddComponent<RectTransform>();
            itemLblRT.anchorMin = Vector2.zero;
            itemLblRT.anchorMax = Vector2.one;
            itemLblRT.offsetMin = new Vector2(8f, 0f);
            itemLblRT.offsetMax = new Vector2(-8f, 0f);

            Text itemLblTxt      = itemLblGO.AddComponent<Text>();
            itemLblTxt.text      = lang.nativeName;
            itemLblTxt.font      = font;
            itemLblTxt.fontSize  = fontSize;
            itemLblTxt.color     = textColor;
            itemLblTxt.alignment = TextAnchor.MiddleCenter;

            // Capture loop variables for the closure
            string capturedCode = lang.code;
            Image  capturedBg   = itemBg;
            Button capturedBtn  = itemBtn;

            itemBtn.onClick.AddListener(() =>
            {
                LocalizationManager.Instance.SetLanguage(capturedCode);
                LocalizationManager.Instance.MarkLanguageSelected();
                RefreshPopupSelection(panelGO, capturedCode);
                owner.RefreshLabel();
                owner.ClosePopup();
            });
        }

        panelGO.SetActive(false);
        return panelGO;
    }

    // ── Helper: update item highlight colours after a selection ───────────────
    private static void RefreshPopupSelection(GameObject panel, string selectedCode)
    {
        List<LanguageInfo> langs = LocalizationManager.Instance.GetSupportedLanguages();
        foreach (LanguageInfo lang in langs)
        {
            Transform item = panel.transform.Find("Item_" + lang.code);
            if (item == null) continue;

            bool  selected = lang.code == selectedCode;
            Image bg       = item.GetComponent<Image>();
            if (bg != null) bg.color = selected ? BG_SELECTED : Color.clear;

            Button btn = item.GetComponent<Button>();
            if (btn != null)
            {
                ColorBlock cb  = btn.colors;
                cb.normalColor = selected ? BG_SELECTED : Color.clear;
                btn.colors     = cb;
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // Instance helpers
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Updates the dropdown button text to the current language name.</summary>
    public void RefreshLabel()
    {
        if (_btnLabel == null) return;

        List<LanguageInfo> langs = LocalizationManager.Instance.GetSupportedLanguages();
        string current = LocalizationManager.Instance.GetCurrentLanguage();

        foreach (LanguageInfo lang in langs)
        {
            if (lang.code == current)
            {
                _btnLabel.text = lang.nativeName + " \u25be"; // ▾ down-pointing triangle
                return;
            }
        }
        _btnLabel.text = current + " \u25be";
    }

    private void TogglePopup() { if (_popupOpen) ClosePopup(); else OpenPopup(); }

    private void OpenPopup()
    {
        if (_popup == null) return;
        _popupOpen = true;
        _popup.SetActive(true);
        _popup.transform.SetAsLastSibling();
    }

    public void ClosePopup()
    {
        if (_popup == null) return;
        _popupOpen = false;
        _popup.SetActive(false);
    }

    // Close popup when this row is disabled (settings panel closed)
    private void OnDisable() { ClosePopup(); }
}
