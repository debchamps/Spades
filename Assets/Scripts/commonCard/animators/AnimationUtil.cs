using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


using System.Collections;
using System.Collections.Generic;
public class AnimationUtil {
    private static Image _darkBkg;
    private static Dictionary<int, Image> _darkBkgNumbered = new Dictionary<int, Image>();
    public static void scaleUpAndDown(GameObject obj, float time, float initialDelay, float scaleFactor) {

        Sequence sequence=  DOTween.Sequence();

        sequence.AppendInterval(initialDelay);
        sequence.Append(obj.transform.DOScale(scaleFactor, time/2).SetEase(Ease.OutCubic));
        sequence.Append(obj.transform.DOScale(1.0f/scaleFactor, time/2).SetEase(Ease.InCubic));
        sequence.Play();

    }


    public static void openDialogue(GameObject obj, float scaleTo, float animateTime) {
        obj.transform.DOKill();
        var increaseScale = scaleTo * 1.05f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(increaseScale, animateTime * .75f).SetEase(Ease.OutQuad));
        sequence.Append(obj.transform.DOScale(scaleTo,       animateTime * .25f).SetEase(Ease.InOutSine));
        sequence.Play();
    }

    public static void closeDialogue(GameObject obj)
    {
        obj.transform.DOKill();
        obj.transform.DOScale(new Vector3(0f, 0f, 1f), .25f).SetEase(Ease.InBack);
        AnimationUtil.closeDarkBkg();

    }

    public static void closeDialogue(GameObject obj, float animateTime)
    {
        obj.transform.DOKill();
        obj.transform.DOScale(new Vector3(0f, 0f, 1f), animateTime).SetEase(Ease.InBack);
        AnimationUtil.closeDarkBkg();

    }



    public static void moveUpAndDown(GameObject obj, float time, int count, float initialDelay, float amount) {

        Sequence sequence=  DOTween.Sequence();

        int noOfMovement = count * 4;

        float movementTime = time/noOfMovement;

        sequence.AppendInterval(initialDelay);

        float currentY = obj.transform.position.y;

        for(int i=0;i<noOfMovement;i++) {
            sequence.Append(obj.transform.DOMoveY(currentY + amount, movementTime).SetEase(Ease.OutSine));
            sequence.Append(obj.transform.DOMoveY(currentY, movementTime).SetEase(Ease.InSine));
            sequence.Append(obj.transform.DOMoveY(currentY-amount, movementTime).SetEase(Ease.OutSine));
            sequence.Append(obj.transform.DOMoveY(currentY, movementTime).SetEase(Ease.InSine));
            amount = amount/2;
        }
        sequence.Play();
    }



        public static void moveLeftAnRight(GameObject obj, float time, int count, float initialDelay, float amount) {

        Sequence sequence=  DOTween.Sequence();

        int noOfMovement = count * 4;

        float movementTime = time/noOfMovement;

        sequence.AppendInterval(initialDelay);

        float currentX = obj.transform.position.x;

        for(int i=0;i<noOfMovement;i++) {
            sequence.Append(obj.transform.DOMoveX(currentX + amount, movementTime).SetEase(Ease.OutSine));
            sequence.Append(obj.transform.DOMoveX(currentX, movementTime).SetEase(Ease.InSine));
            sequence.Append(obj.transform.DOMoveX(currentX-amount, movementTime).SetEase(Ease.OutSine));
            sequence.Append(obj.transform.DOMoveX(currentX, movementTime).SetEase(Ease.InSine));
            amount = amount/2;
        }
        sequence.Play();
    }

    public static bool isClickedOutside(GameObject obj) {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(obj.GetComponent<RectTransform>(), Input.mousePosition, null);
        return Input.GetMouseButton(0) && !isInside;
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
        if (_darkBkg == null) { var go = GameObject.Find("darkbkg"); if (go != null) _darkBkg = go.GetComponent<Image>(); }
        if (_darkBkg != null) openBkg(_darkBkg, animTime);
    }

    public static void closeDarkBkg() {
        if (_darkBkg == null) { var go = GameObject.Find("darkbkg"); if (go != null) _darkBkg = go.GetComponent<Image>(); }
        if (_darkBkg != null) closeBkg(_darkBkg);
    }


    public static void openDarkBkg(int i, float animTime) {
        if (!_darkBkgNumbered.TryGetValue(i, out Image img) || img == null) {
            var go = GameObject.Find("darkbkg" + i.ToString());
            if (go != null) { img = go.GetComponent<Image>(); _darkBkgNumbered[i] = img; }
        }
        if (img != null) openBkg(img, animTime);
    }

    public static void closeDarkBkg(int i) {
        if (!_darkBkgNumbered.TryGetValue(i, out Image img) || img == null) {
            var go = GameObject.Find("darkbkg" + i.ToString());
            if (go != null) { img = go.GetComponent<Image>(); _darkBkgNumbered[i] = img; }
        }
        if (img != null) closeBkg(img);
    }



    public static void openBkg(Image darkObj, float animTime) {
        var tempColor = darkObj.color;
        tempColor.a = 0f;

        darkObj.enabled = true;

        darkObj.DOFade(150f/255, animTime);
        AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

    }

    public static void closeBkg(Image darkObj)
    {
        darkObj.DOFade(0f, 0.15f);
        AudioManagerScript.play(AudioClipType.CLOSE);
        darkObj.enabled = false;
    }

    public static void disable()
    {
        //darkObj.enabled = false;

    }




}