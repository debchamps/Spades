using UnityEngine;
using TMPro;

public static class RTLTextHelper
{
    public static void ApplyRTL(TMP_Text textComponent, bool isRTL)
    {
        if (textComponent == null) return;
        textComponent.isRightToLeftText = isRTL;
        if (isRTL)
            textComponent.alignment = TextAlignmentOptions.Right;
    }

    public static void ApplyRTLToAll(bool isRTL)
    {
        TMP_Text[] allTexts = GameObject.FindObjectsOfType<TMP_Text>();
        foreach (var text in allTexts)
        {
            ApplyRTL(text, isRTL);
        }
    }
}
