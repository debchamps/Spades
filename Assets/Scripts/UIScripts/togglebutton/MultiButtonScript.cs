using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class MultiButtonScript : MonoBehaviour {


    public int totalButtons;
    public string DEFAULT_CODE;
    public string EVENT_TYPE;


    //public Color32 ACTIVE_COLOR = new Color32(14,89,17,255);
    public Color32 ACTIVE_COLOR = new Color32(14,89,17,255);
    public Color32 INACTIVE_COLOR = new Color32(233,155,155,255);

    SettingsManager settingsManager;

    public void activateByCode() {

        if(!PlayerPrefs.HasKey(EVENT_TYPE)) {
            PlayerPrefs.SetString(EVENT_TYPE, DEFAULT_CODE);
            PlayerPrefs.Save();
        }

        string storedCode = PlayerPrefs.GetString(EVENT_TYPE, DEFAULT_CODE);

        foreach (Transform child in gameObject.transform) {  
            if(child.gameObject.GetComponent<SingleButtonScript>()!= null) {
                string btnCode = child.gameObject.GetComponent<SingleButtonScript>().CODE;
                if(btnCode == storedCode) {
                    child.gameObject.GetComponent<SingleButtonScript>().activateOutside();   
                }
            }
        }
    }
    


}