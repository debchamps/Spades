using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string localizationKey;

    private Text uiText;
    private TMP_Text tmpText;

    void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(localizationKey)) return;

        string translated = LocalizationManager.Instance.Get(localizationKey);
        bool isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();

        if (tmpText != null)
        {
            tmpText.text = translated;
            tmpText.isRightToLeftText = isRTL;
            if (isRTL)
                tmpText.alignment = TextAlignmentOptions.Right;
            else
                tmpText.alignment = TextAlignmentOptions.Left;
        }
        else if (uiText != null)
        {
            uiText.text = translated;
            if (isRTL)
                uiText.alignment = TextAnchor.MiddleRight;
            else
                uiText.alignment = TextAnchor.MiddleLeft;
        }
    }
}
