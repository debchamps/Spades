using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPlayStore : MonoBehaviour  {

    public string GAME_ID;
    public string IOS_GAME_ID;

    public GamesConfig[] gamesCongigList;

    public void open() {


        #if UNITY_IPHONE
                    openIOS();
        #endif

        #if UNITY_ANDROID
                openAndroid();
        #endif

        //should close the panel.

        GameObject.Find("othergamesicon").GetComponent<GamesScript>().close();


    }

    void openAndroid()
    {

        string url = "https://play.google.com/store/apps/details?id=" + GAME_ID;
        Application.OpenURL(url);

        //Application.OpenURL ("market://details?id=" + GAME_ID);

    }


    void openIOS()
    {
        Application.OpenURL("market://details?id=" + IOS_GAME_ID);

    }


}