using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsScript : MonoBehaviour
{
    // Start is called before the first frame update

    SettingsManager settingsManager = new SettingsManager();
    static bool open = false;

    void Update() {
        if(open) {
            
             if(AnimationUtil.isClickedOutside(GameObject.Find("settingsparent")) && AnimationUtil.isClickedOutside(GameObject.Find("settingsiconbtn"))) {
                 close();
             }

        }
     }


     public void closeOthers() {
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().close();
        //GameObject.Find("ScriptEmpty").GetComponent<SettingsScript>().close();
        GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().close();
    }


    public void enable() {
        if(GamePlay.matchState == null || GamePlay.matchState.getCurrentGameState()== null || GamePlay.isBiddingHappening())
            return;

        if(!open) {
            open = true;
            closeOthers();
            var settings = settingsManager.getBraySettings();

            

            GameObject.Find("SoundEnabledToggle").GetComponent<Toggle>().isOn = settings.soundEnabled;
            GameObject.Find("MusicEnabledToggle").GetComponent<Toggle>().isOn = settings.musicEnabled;
            GameObject.Find("MercyEnabledToggle").GetComponent<Toggle>().isOn = settings.isMercyEnabled;
            GameObject.Find("SandBagEnabledToggle").GetComponent<Toggle>().isOn = settings.isSandBagEnabled;


            
            GameObject.Find("brayendScoreButton").GetComponent<MultiButtonScript>().activateByCode();
            GameObject.Find("settingsoverride").transform.localScale = new Vector3(0, 0, 1f);

            GameObject.Find("settingsparent").transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
            //GameObject.Find("settingsparent").transform.DOScale(new Vector3(0.8f,0.8f, 1f),.25f);


            AnimationUtil.openDarkBkg(0.25f);
            AnimationUtil.openDialogue(GameObject.Find("settingsparent"), 0.8f, 0.25f);
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);


        } else {
            close();
        }

    }

    public void close() {

        open = false;

        //Save the settings.
        settingsManager.setSettings(BraySettingsKey.SOUND_ENABLED, GameObject.Find("SoundEnabledToggle").GetComponent<Toggle>().isOn?1:0);
        settingsManager.setSettings(BraySettingsKey.MUSIC_ENABLED, GameObject.Find("MusicEnabledToggle").GetComponent<Toggle>().isOn?1:0);
        //settingsManager.setSettings(BraySettingsKey.AUTOTHROW_ENABLED, GameObject.Find("AutoThrowEnabledToggle").GetComponent<Toggle>().isOn?1:0);


        //GameObject.Find("settingsparent").transform.position = new Vector3(Screen.width* 6, Screen.height/2, 0);

        AnimationUtil.closeDialogue(GameObject.Find("settingsparent"));
        //GameObject.Find("settingsparent").transform.DOScale(new Vector3(0.0f,0.0f, 1f),.25f).SetEase(Ease.InBack);

    }



    public void onMercyChange() {

            settingsManager.setSettings(BraySettingsKey.IS_MERCY_ENABLED, GameObject.Find("MercyEnabledToggle").GetComponent<Toggle>().isOn?1:0);
            bool currGameVal = GamePlay.matchState.isMercyEnabled;
            bool settingsVal= settingsManager.getBraySettings().isMercyEnabled;

            Debug.Log("onMercyChange" + " currGameVal " + currGameVal + " settingsVal : " + settingsVal);

            if(!currGameVal.Equals(settingsVal)) {
                GameObject.Find("settingsoverride").transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
                AnimationUtil.openDarkBkg(2, 0.25f);
                AnimationUtil.openDialogue(GameObject.Find("settingsoverride"), 0.8f, 0.25f);

            }
    }

    public void onEndTargetChange() {
            //First check, if things are really changing.
            int currGameVal = GamePlay.matchState.gameTarget;
            int settingsVal= settingsManager.getBraySettings().endTarget;

            if(!currGameVal.Equals(settingsVal)) {

                GameObject.Find("settingsoverride").transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
                AnimationUtil.openDarkBkg(2, 0.25f);
                AnimationUtil.openDialogue(GameObject.Find("settingsoverride"), 0.8f, 0.25f);
            }
        
    }


    public void onMusicEnabled()
    {

        bool isMusicEnabled = GameObject.Find("MusicEnabledToggle").GetComponent<Toggle>().isOn;
        if(isMusicEnabled)
        {   
            //Start the Music.
        } else
        {
            //Stop the music
        }
        //First check, if things are really changing.
        //First persist the change.

    }

    public void onSandbagChange() {
            //First check, if things are really changing.
            settingsManager.setSettings(BraySettingsKey.IS_SANDBAG_ENABLED, GameObject.Find("SandBagEnabledToggle").GetComponent<Toggle>().isOn?1:0);
            bool currGameVal = GamePlay.matchState.isSandBagEnabled;
            bool settingsVal= settingsManager.getBraySettings().isSandBagEnabled;

            if(!currGameVal.Equals(settingsVal)) {
                GameObject.Find("settingsoverride").transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
                AnimationUtil.openDarkBkg(2, 0.25f);
                AnimationUtil.openDialogue(GameObject.Find("settingsoverride"), 0.8f, 0.25f);
            }
        
    }

    public void closeInfo() {
        GameObject.Find("settingsoverride").transform.DOScale(new Vector3(0.0f,0.0f, 1f),.25f);
        AnimationUtil.closeDarkBkg(2);
    }


}
