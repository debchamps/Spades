using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


using System.Collections;
using System.Collections.Generic;
public class ScreenWidthAdjustUtil : MonoBehaviour {
    

    public void initialize() {

        float width = Screen.width;
        if(width <1500) {
            //GameObject obj = GameObject.Find("BiddingPanel");
            //obj.transform.localScale = new Vector3(1f,0.8f,1f);
        }

    }

}