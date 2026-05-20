using System.Collections.Generic;

[System.Serializable]
public class LocalizationConfigData
{
    public string defaultLanguage;
    public List<LanguageInfo> supportedLanguages;
}

[System.Serializable]
public class LanguageInfo
{
    public string code;
    public string displayName;
    public string nativeName;
    public bool isRTL;
}
