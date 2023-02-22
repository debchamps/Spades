using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenCaptureCamera : MonoBehaviour
{
    //here you can set the folder you want to use, 
    //IMPORTANT - use "@" before the string, because this is a verbatim string
    //IMPORTANT - the folder must exists
    string pathToYourFile = "/Users/debarghy/Desktop/UnityGames/AppResources/ScreenshotSpades/";
    //this is the name of the file
    string fileName = "16_9";
    //this is the file type
    string fileType = ".png";

    bool captureEnabled = false;

    private int CurrentScreenshot { get => PlayerPrefs.GetInt("ScreenShot"); set => PlayerPrefs.SetInt("ScreenShot", value); }

    private void Update()
    {
        if(captureEnabled) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //if(EditorApplication.isPlaying)
                {

                    UnityEngine.ScreenCapture.CaptureScreenshot(pathToYourFile + fileName + CurrentScreenshot + fileType);
                    CurrentScreenshot++;
                }
            }

        }
    }
}