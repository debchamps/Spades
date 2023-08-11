using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class SingleButtonScript : MonoBehaviour {

    public int buttonNumber;

    public string CODE;
    public string EVENT_TYPE;


    Color32 ACTIVE_COLOR = new Color32(8,197,15,255);
    Color32 INACTIVE_COLOR = new Color32(200,170,170,255);


    public void activate() {
        deactivateOthers();
        gameObject.GetComponent<Image>().color =  gameObject.transform.parent.gameObject.GetComponent<MultiButtonScript>().ACTIVE_COLOR;      
        PlayerPrefs.SetString(EVENT_TYPE, CODE);
        PlayerPrefs.Save();

        //HACK: Only works for this case.
        GameObject.Find("settingsparent").GetComponent<SettingsScript>().onEndTargetChange();
        //Generate event.
    }



    public void activateOutside() {
        deactivateOthers();
        gameObject.GetComponent<Image>().color =  gameObject.transform.parent.gameObject.GetComponent<MultiButtonScript>().ACTIVE_COLOR;      
        PlayerPrefs.SetString(EVENT_TYPE, CODE);
        PlayerPrefs.Save();

        //HACK: Only works for this case.
        //GameObject.Find("settingsparent").GetComponent<SettingsScript>().onEndTargetChange();
        //Generate event.
    }

    public void deactivate() {
    
        gameObject.GetComponent<Image>().color =  gameObject.transform.parent.gameObject.GetComponent<MultiButtonScript>().INACTIVE_COLOR;

    }

    void deactivateOthers() {
        GameObject parent = gameObject.transform.parent.gameObject;
        foreach (Transform child in parent.transform) {  
            if(child.gameObject.GetComponent<SingleButtonScript>()!= null) {
                   int btnNo = child.gameObject.GetComponent<SingleButtonScript>().buttonNumber;
                   if(buttonNumber != btnNo) {
                        child.gameObject.GetComponent<SingleButtonScript>().deactivate();   
                   }

            }

        }

    }




}