
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HowToPlayScript : MonoBehaviour {
    static int pageNumber = 1;

    static bool open = false;

    public int totalPage;

    public static Dictionary<int, string> pageNumberToHeader = new Dictionary<int, string>();


    void Update() {
        totalPage = 7;

        if (open) {
            
             if(AnimationUtil.isClickedOutside(GameObject.Find("howtoplay")) && AnimationUtil.isClickedOutside(GameObject.Find("bottomBar/help"))) {
                 close();
             }

        }
     }

    public void show() {
        if(!open) {
            loadHeader();
            open = true;
            GameObject.Find("howtoplay").transform.position = new Vector3(Screen.width/2, -Screen.height/2, 1f);
            AnimationUtil.openDarkBkg(0.5f);
            GameObject.Find("howtoplay").transform.DOMove(new Vector3(Screen.width/2, Screen.height/2, 1f), .5f);
            GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(0, 0f);
            GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(1, 0f);
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);
            setPage();
        } else {
            close();
        }

    }

    public void close() {
        open = false;
        GameObject.Find("howtoplay").transform.DOMove(new Vector3(Screen.width/2, -Screen.height/2, 1f), .25f);
        AnimationUtil.closeDarkBkg();
        pageNumber = 1;

    }


    public void setPageAnimate() {
        GameObject obj = GameObject.Find("howtoplay/helpimage");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.GetComponent<Image>().DOFade(0, .2f));
        sequence.AppendCallback(setPage);
        sequence.Append(obj.GetComponent<Image>().DOFade(1, .2f));
        sequence.Play();
                //Change page content.
    }

    public void setPage() {
        GameObject obj = GameObject.Find("howtoplay/helpimage");
        GameObject.Find("howtoplay/header").GetComponent<Text>().text = pageNumberToHeader[pageNumber];
        obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("howtoplay/help" + pageNumber.ToString());

    }

    public void next() {
        if(pageNumber < totalPage) {
            pageNumber++;
            setPageAnimate();
            if(pageNumber >= 2) {
                GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(1, .1f);
        
            }

            if(pageNumber == totalPage) {
                GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(0, .1f);
        
            }


        }

    }
    public void previous() {

        if(pageNumber > 1) {
            pageNumber--;
            setPageAnimate();

            if(pageNumber == 1) {
                GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(0, .1f);
        
            }

            if(pageNumber <= totalPage -1) {
                GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(1, .1f);        
            }

        }


    }


    void loadHeader() {
        pageNumberToHeader[1] = "Objective";
        pageNumberToHeader[2] = "Bidding";
        pageNumberToHeader[3] = "NIL Bid";
        pageNumberToHeader[4] = "Scoring";
        pageNumberToHeader[5] = "GamePlay";
        pageNumberToHeader[6] = "GamePlay (Trump)";
        pageNumberToHeader[7] = "Sandbag Penalty";
    }


}