using System;
using System.Collections.Generic;

/// <summary>
/// Data model for a single page in the How-To-Play popup.
/// Loaded from a game-specific JSON file in Resources/Localization/.
/// </summary>
[Serializable]
public class HelpPage
{
    /// <summary>Localization key for the page title shown in "howtoplay/header".</summary>
    public string headerKey;

    /// <summary>
    /// "text"  – render bodyKey as TMP rich text (default)
    /// "image" – display imagePath as a sprite (legacy fallback)
    /// "mixed" – show text then image below it
    /// </summary>
    public string contentType;

    /// <summary>
    /// Localization key whose value is a full TMP rich-text body string.
    /// Used when contentType is "text" or "mixed".
    /// </summary>
    public string bodyKey;

    /// <summary>
    /// Resources-relative path to a sprite (e.g. "howtoplay/help1").
    /// Used when contentType is "image" or "mixed", or as a fallback
    /// when the body key is missing.
    /// </summary>
    public string imagePath;

    public HelpPage()
    {
        contentType = "text";
    }
}

/// <summary>
/// The complete how-to-play content definition for one game.
/// Deserialised from  Resources/Localization/{gameId}_help.json
/// </summary>
[Serializable]
public class HelpContent
{
    /// <summary>Game identifier, e.g. "callbreak", "hearts", "spades".</summary>
    public string gameId;

    /// <summary>Ordered list of help pages (page 1 = index 0).</summary>
    public List<HelpPage> pages;

    public HelpContent()
    {
        pages = new List<HelpPage>();
    }
}
