using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareScript : MonoBehaviour  {

    public string game;
    public string desc;

    public void share() {
        new NativeShare().SetSubject(LocalizationManager.Instance.Get("share_subject")).SetText(description()).Share();

    }

    public string description() {
        //return "One of the best " + game + " game." + "\n\n Android : " +  getAndroidLink() + "\n\n IOS : " + getAndroidLink();
        return LocalizationManager.Instance.Get("share_description", game) + "\n\n Android : " +  getAndroidLink();
    }


    string getAndroidLink() {
        //return "tinyurl.com/callbreak";
        return "https://play.google.com/store/apps/details?id=com.datgames.spade";

    }




}