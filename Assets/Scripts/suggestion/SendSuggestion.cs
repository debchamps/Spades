using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class SendSuggestion : MonoBehaviour
{
    public GameObject suggestionButton;
    public string recipientEmail = "devteam@example.com";

    public string subject = "Game Suggestion";

    public GameObject dialogue;

    private void Start()
    {
        suggestionButton.GetComponent<Button>().onClick.AddListener(OnSuggestionButtonClick);
    }

    private void OnSuggestionButtonClick()
    {
        string playerName = "Player"; // Replace with the actual player's name
        string body = $"Hello,\n\nI'm {playerName}, and I have a suggestion for improving the game:\n\n";

        // You can customize the body of the email with the player's suggestion
        // For example:
        // body += "- Add more levels.\n";
        // body += "- Improve character movement.\n";

        body += "\n\nThank you for considering my suggestion!\n";

        OpenDefaultEmailClient(recipientEmail, subject, body);
    }

    private void OpenDefaultEmailClient(string recipient, string subject, string body)
    {
        string uri = "mailto:" + recipient + "?subject=" + Uri.EscapeDataString(subject) + "&body=" + Uri.EscapeDataString(body);
        Process.Start(uri);

        //Close the dialogue.
    }
}