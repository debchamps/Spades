
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

using UnityEngine.UI;

public class WarningScript : MonoBehaviour {

    bool isWarning = false;
    int latestWarningId =1 ;

    static System.Random RND = new System.Random(); 

    void Start() {
        Rect warningRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playwarningv2").transform);
        Vector3 warnpos = GameObject.Find("playwarningv2").transform.position;
        float width = RectTransformExt.GetWorldRect(GameObject.Find("playwarningv2").GetComponent<RectTransform>()).width;
        float height = RectTransformExt.GetWorldRect(GameObject.Find("playwarningv2").GetComponent<RectTransform>()).height;
        StartCoroutine(MoveTo.MoveOverSeconds(GameObject.Find("playwarningv2"), new Vector3(warnpos.x,Screen.height + height/2, warnpos.z ), 0f));
    }

    public  void showWarningAndDisappear( WarningEntity warningEntity) {
        showWarningAndDisappear(warningEntity , 1.5f);
    }

    public  void showWarningAndDisappearLong( WarningEntity warningEntity) {
        showWarningAndDisappear(warningEntity , 2.5f);
    }


    public  void showWarningAndDisappear( WarningEntity warningEntity, float stayTime ) {
        float appearanceTime = 0.1f;

        if(isWarning) {
            //DebugLog.Log("\niFtrueWarning : " + isWarning);
            StartCoroutine(disappearAndShowWarning(warningEntity));
        } else {

            int newWarningId = RND.Next();
            latestWarningId = newWarningId;

            isWarning = true;

            GameObject parent = GameObject.Find("playwarningv2");
            GameObject.Find("playwarningv2/playwarningtxtv2").GetComponent<TMP_Text>().SetText(warningEntity.text);
            //GameObject.Find("playwarningv2/icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(warningEntity.path);
            
            float width = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).width;
            Rect warningRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playwarningv2").transform);
            float height = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).height;


            float moveX = Screen.width/3 - parent.transform.position.x + warningRect.width/2;


            Vector3 currPos = parent.transform.position;

            Vector3 finalLocation1 = new Vector3(currPos.x,Screen.height - height*1.1f/2, currPos.z );
            Vector3 finalLocation2 = new Vector3(currPos.x,Screen.height - height/2, currPos.z );
            //parent.transform.DOMoveX(width, .1f);

            Sequence sequence=  DOTween.Sequence();
            sequence.Append(parent.transform.DOMove(finalLocation1, appearanceTime * .8f));
            sequence.Append(parent.transform.DOMove(finalLocation2, appearanceTime * .2f));
            sequence.Play();

            //StartCoroutine(MoveTo.MoveOverSeconds(parent, new Vector3(currPos.x,Screen.height - height/2, currPos.z ), .1f));

            if(isWarning) {
                StartCoroutine(disappearEnumerator(.3f, stayTime, newWarningId));
                //parent.transform.DOMoveX(width, .5f).SetDelay(2f);
                //isWarning = false;

            }

        }

    //SetDelay(initialDelay)

    }

    public IEnumerator disappearEnumerator(float animTime, float delay, int warningId) {
        //DebugLog.Log("\n     warning disEnum : " + latestWarningId + " isWarning : " + isWarning + " delay " + delay);
       if(!isWarning) {
            yield break;
        } else {
            yield return new WaitForSeconds(delay);
        //DebugLog.Log("\n     warning disappearEnumerator : " + latestWarningId + " isWarning : " + isWarning + " warningId " + warningId);
        if(isWarning && warningId.Equals(latestWarningId)) {
                GameObject parent = GameObject.Find("playwarningv2");
                DebugLog.Log("\n     warning disaapear : " + latestWarningId + " success : ");

                float width = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).width;
               float height = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).height;
                //parent.transform.DOMoveX(-width, animTime);
                Vector3 currPos = GameObject.Find("playwarningv2").transform.position;
                StartCoroutine(MoveTo.MoveOverSeconds(parent, new Vector3(currPos.x,Screen.height + height/2, currPos.z ), animTime));
                isWarning = false;
            }

        }


    }

    public IEnumerator disappearAndShowWarning(WarningEntity warningEntity) {
        disappearBeforeShowing();
        yield return new WaitForSeconds(0.05f);
        showWarningAndDisappear(warningEntity);

    }


    public  void disappearBeforeShowing() {
        //DebugLog.Log("\n     warning disappear Start : " + latestWarningId );
         GameObject parent = GameObject.Find("playwarningv2");
       float width = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).width;
        if(isWarning) { 
            StartCoroutine(disappearEnumerator(.1f, 0f, latestWarningId));
        }

    }

    public  void disappear() {
        //DebugLog.Log("\n     warning disappear Start : " + latestWarningId );
         GameObject parent = GameObject.Find("playwarningv2");
       float width = RectTransformExt.GetWorldRect(parent.GetComponent<RectTransform>()).width;
        if(isWarning) { 
            //StartCoroutine(disappearEnumerator(.1f, 0f, latestWarningId));
            StartCoroutine(disappearEnumerator(.1f, .15f, latestWarningId));
            //StartCoroutine(disappearEnumerator(.1f, .8f, latestWarningId));
        }

    }


}