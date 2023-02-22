
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SettingsManager {


    public  void initiateBraySettings() {
        if(!PlayerPrefs.HasKey(BraySettingsKey.SOUND_ENABLED))
            setSettings(BraySettingsKey.SOUND_ENABLED, 1);
        if(!PlayerPrefs.HasKey(BraySettingsKey.MUSIC_ENABLED))
            setSettings(BraySettingsKey.MUSIC_ENABLED, 0);
        if(!PlayerPrefs.HasKey(BraySettingsKey.AUTOTHROW_ENABLED))
            setSettings(BraySettingsKey.AUTOTHROW_ENABLED, 0);
        if(!PlayerPrefs.HasKey(BraySettingsKey.SPADE_END_TARGET))
            setSettingsString(BraySettingsKey.SPADE_END_TARGET, "500");


        if(!PlayerPrefs.HasKey(BraySettingsKey.IS_MERCY_ENABLED))
            setSettings(BraySettingsKey.IS_MERCY_ENABLED, 0);

        if(!PlayerPrefs.HasKey(BraySettingsKey.IS_SANDBAG_ENABLED))
            setSettings(BraySettingsKey.IS_SANDBAG_ENABLED, 1);

    }

    public BraySettings getBraySettings(){
        BraySettings braySettings = new BraySettings();
        braySettings.soundEnabled = getSettings(BraySettingsKey.SOUND_ENABLED)!=0;
        braySettings.musicEnabled = getSettings(BraySettingsKey.MUSIC_ENABLED)!=0;
        braySettings.autothrowEnabled = getSettings(BraySettingsKey.AUTOTHROW_ENABLED)!=0;
        braySettings.endTarget = int.Parse(getSettingsString(BraySettingsKey.SPADE_END_TARGET));

        braySettings.isMercyEnabled = getSettings(BraySettingsKey.IS_MERCY_ENABLED)!=0;
        braySettings.isSandBagEnabled = getSettings(BraySettingsKey.IS_SANDBAG_ENABLED)!=0;

        return braySettings;

    }

    public string getSettingsString(string settingsKey) {
        return PlayerPrefs.GetString(settingsKey);
        
    }

    public int getSettings(string settingsKey) {
        return PlayerPrefs.GetInt(settingsKey);
        
    }
    public void setSettingsString(string settingsKey, string val) {
        
        PlayerPrefs.SetString(settingsKey, val);
        PlayerPrefs.Save();   
        
    }


    public void setSettings(string settingsKey, int val) {
        
        PlayerPrefs.SetInt(settingsKey, val);
        PlayerPrefs.Save();   
        
    }



}