using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationManager
{
    // Feature flag — set false to suppress language-selection UI while keeping Get() working
    public const bool ENABLED = false;

    private static LocalizationManager _instance;
    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LocalizationManager();
            return _instance;
        }
    }

    private const string LANGUAGE_PREF_KEY = "SelectedLanguage";
    private const string FIRST_LAUNCH_KEY = "HasSelectedLanguage";

    private Dictionary<string, string> currentTranslations = new Dictionary<string, string>();
    private Dictionary<string, string> fallbackTranslations = new Dictionary<string, string>();
    private LocalizationConfigData config;
    private string currentLanguage;
    private bool initialized = false;

    public delegate void LanguageChangedHandler();
    public static event LanguageChangedHandler OnLanguageChanged;

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        // Language selection disabled — always use English.
        LoadFallback();
        SetLanguage("en", false);
    }

    public bool IsFirstLaunch()
    {
        if (!ENABLED) return false;
        return !PlayerPrefs.HasKey(FIRST_LAUNCH_KEY);
    }

    public void MarkLanguageSelected()
    {
        PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
        PlayerPrefs.Save();
    }

    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public List<LanguageInfo> GetSupportedLanguages()
    {
        return config.supportedLanguages;
    }

    public bool IsCurrentLanguageRTL()
    {
        if (config == null) return false;
        foreach (var lang in config.supportedLanguages)
        {
            if (lang.code == currentLanguage) return lang.isRTL;
        }
        return false;
    }

    public void SetLanguage(string languageCode, bool notify = true)
    {
        currentLanguage = languageCode;
        PlayerPrefs.SetString(LANGUAGE_PREF_KEY, languageCode);
        PlayerPrefs.Save();
        LoadTranslations(languageCode);
        if (notify) OnLanguageChanged?.Invoke();
    }

    public string Get(string key)
    {
        if (currentTranslations.TryGetValue(key, out string val))
            return val;
        if (fallbackTranslations.TryGetValue(key, out string fallback))
            return fallback;
        return "[" + key + "]";
    }

    public string Get(string key, params object[] args)
    {
        string template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    private bool IsLanguageSupported(string code)
    {
        if (config == null) return false;
        foreach (var lang in config.supportedLanguages)
        {
            if (lang.code == code) return true;
        }
        return false;
    }

    private string GetDeviceLanguageCode()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English: return "en";
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.Portuguese: return "pt";
            case SystemLanguage.Arabic: return "ar";
            case SystemLanguage.Hindi: return "hi";
            case SystemLanguage.Unknown: return "en";
            default: return "en";
        }
    }

    private void LoadConfig()
    {
        TextAsset configAsset = Resources.Load<TextAsset>("Localization/_config");
        if (configAsset != null)
            config = JsonConvert.DeserializeObject<LocalizationConfigData>(configAsset.text);
    }

    private void LoadFallback()
    {
        fallbackTranslations = LoadLanguageFile("en");
    }

    private void LoadTranslations(string langCode)
    {
        currentTranslations = LoadLanguageFile(langCode);
    }

    private Dictionary<string, string> LoadLanguageFile(string langCode)
    {
        TextAsset asset = Resources.Load<TextAsset>("Localization/" + langCode);
        if (asset == null) return new Dictionary<string, string>();
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text)
               ?? new Dictionary<string, string>();
    }
}
