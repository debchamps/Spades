using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


using System.Collections;
using System.Collections.Generic;
public class AnimationUtil {
    public static void scaleUpAndDown(GameObject obj, float time, float initialDelay, float scaleFactor) {

        Sequence sequence=  DOTween.Sequence();

        sequence.AppendInterval(initialDelay);
        sequence.Append(obj.transform.DOScale(scaleFactor, time/2));
        sequence.Append(obj.transform.DOScale(1.0f/scaleFactor, time/2));
        sequence.Play();

    }


    public static void openDialogue(GameObject obj, float scaleTo, float animateTime) {
        
        var increaseScale = scaleTo * 1.05f;
        Sequence sequence=  DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(increaseScale, animateTime * .85f));
        sequence.Append(obj.transform.DOScale(scaleTo, animateTime * .15f));
        sequence.Play();
       
    }


     public static void moveUpAndDown(GameObject obj, float time, int count, float initialDelay, float amount) {

        Sequence sequence=  DOTween.Sequence();

        int noOfMovement = count * 4;

        float movementTime = time/noOfMovement;

        sequence.AppendInterval(initialDelay);

        float currentY = obj.transform.position.y;

        for(int i=0;i<noOfMovement;i++) {
            sequence.Append(obj.transform.DOMoveY(currentY + amount, movementTime));
            sequence.Append(obj.transform.DOMoveY(currentY, movementTime));
            sequence.Append(obj.transform.DOMoveY(currentY-amount, movementTime));
            sequence.Append(obj.transform.DOMoveY(currentY, movementTime));
            amount = amount/2;            
        }
        sequence.Play();
    }



        public static void moveLeftAnRight(GameObject obj, float time, int count, float initialDelay, float amount) {

        Sequence sequence=  DOTween.Sequence();

        int noOfMovement = count * 4;

        float movementTime = time/noOfMovement;

        sequence.AppendInterval(initialDelay);

        float currentY = obj.transform.position.x;

        for(int i=0;i<noOfMovement;i++) {
            sequence.Append(obj.transform.DOMoveX(currentY + amount, movementTime));
            sequence.Append(obj.transform.DOMoveX(currentY, movementTime));
            sequence.Append(obj.transform.DOMoveX(currentY-amount, movementTime));
            sequence.Append(obj.transform.DOMoveX(currentY, movementTime));
            amount = amount/2;            
        }
        sequence.Play();
    }

    public static bool  isClickedOutside(GameObject obj) {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(obj);
        return isClickedOutside(objects);
    }


     public static bool isClickedOutside(List<GameObject> panels) {

        bool isClickedInAnyPanel = false;
        foreach(GameObject obj in panels) {
             bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
                 obj.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 null);

            if(isInside) {
                isClickedInAnyPanel = true;
                return false;
                break;
            }
                
        }



         if (Input.GetMouseButton(0)  &&
             !isClickedInAnyPanel) {
                 return true;
         }
          return false;

     }



    public static void openDarkBkg(float animTime) {
        var darkObj = GameObject.Find("darkbkg").GetComponent<Image>();
        openBkg(darkObj, animTime);
    }

    public static void closeDarkBkg() {
        var darkObj = GameObject.Find("darkbkg").GetComponent<Image>();
        closeBkg(darkObj);
    }


    public static void openDarkBkg(int i, float animTime) {
        var darkObj = GameObject.Find("darkbkg" + i.ToString()).GetComponent<Image>();
        openBkg(darkObj, animTime);
    }

    public static void closeDarkBkg(int i) {
        var darkObj = GameObject.Find("darkbkg"+i.ToString()).GetComponent<Image>();
        closeBkg(darkObj);
    }



    public static void openBkg(Image darkObj, float animTime) {
        var tempColor = darkObj.color;
        tempColor.a = 0f;

        darkObj.enabled = true;

        darkObj.DOFade(150f/255, animTime);

    }

    public static void closeBkg(Image darkObj) {
        darkObj.DOFade(0f/255, 0f);
        darkObj.enabled = false;
    }






}