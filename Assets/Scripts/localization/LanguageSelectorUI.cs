using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class LanguageSelectorUI : MonoBehaviour
{
    private static bool isOpen = false;
    private static GameObject selectorPanel;

    public static void CheckAndShowLanguagePrompt()
    {
        if (!LocalizationManager.ENABLED) return;
        if (LocalizationManager.Instance.IsFirstLaunch())
        {
            ShowLanguageSelector(true);
        }
    }

    public static void ShowLanguageSelector(bool isFirstLaunch = false)
    {
        if (!LocalizationManager.ENABLED) return;
        if (isOpen) return;
        isOpen = true;

        List<LanguageInfo> languages = LocalizationManager.Instance.GetSupportedLanguages();

        selectorPanel = new GameObject("LanguageSelectorPanel");
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (canvas == null) canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas != null) selectorPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = selectorPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Dark background
        Image bgImage = selectorPanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);

        // Container for buttons
        GameObject container = new GameObject("Container");
        container.transform.SetParent(selectorPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 80 + languages.Count * 90);
        containerRect.anchoredPosition = Vector2.zero;

        Image containerBg = container.AddComponent<Image>();
        containerBg.color = new Color(0.15f, 0.15f, 0.25f, 0.95f);

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(container.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 60);
        titleRect.anchoredPosition = new Vector2(0, -10);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "Select Language";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 28;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;

        // Language buttons
        for (int i = 0; i < languages.Count; i++)
        {
            LanguageInfo lang = languages[i];
            float yPos = -80 - (i * 85);

            GameObject btnObj = new GameObject("LangBtn_" + lang.code);
            btnObj.transform.SetParent(container.transform, false);
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 1);
            btnRect.anchorMax = new Vector2(0.5f, 1);
            btnRect.pivot = new Vector2(0.5f, 1);
            btnRect.sizeDelta = new Vector2(300, 70);
            btnRect.anchoredPosition = new Vector2(0, yPos);

            Image btnBg = btnObj.AddComponent<Image>();
            btnBg.color = new Color(0.25f, 0.25f, 0.4f, 1f);

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.35f, 0.35f, 0.55f, 1f);
            colors.pressedColor = new Color(0.45f, 0.45f, 0.65f, 1f);
            btn.colors = colors;

            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = btnTextObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text btnText = btnTextObj.AddComponent<Text>();
            btnText.text = lang.nativeName + " (" + lang.displayName + ")";
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnText.fontSize = 22;
            btnText.color = Color.white;
            btnText.alignment = TextAnchor.MiddleCenter;

            string langCode = lang.code;
            btn.onClick.AddListener(() => OnLanguageSelected(langCode, isFirstLaunch));
        }

        // Animate in
        container.transform.localScale = Vector3.zero;
        container.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private static void OnLanguageSelected(string langCode, bool isFirstLaunch)
    {
        LocalizationManager.Instance.SetLanguage(langCode);
        if (isFirstLaunch)
            LocalizationManager.Instance.MarkLanguageSelected();

        CloseSelector();
    }

    public static void CloseSelector()
    {
        isOpen = false;
        if (selectorPanel != null)
        {
            Transform container = selectorPanel.transform.Find("Container");
            if (container != null)
            {
                container.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    GameObject.Destroy(selectorPanel);
                    selectorPanel = null;
                });
            }
            else
            {
                GameObject.Destroy(selectorPanel);
                selectorPanel = null;
            }
        }
    }
}
