using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class GamesScript : MonoBehaviour  {

    List<GameObject> panels = new List<GameObject>();

    public GamesConfig[] gamesCongigList;

    Vector3 originalPosition;
    bool open = false;

    private void HideIfClickedOutside(GameObject obj) {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(obj);
        HideIfClickedOutside(objects);
    }


    //Taken from https://forum.unity.com/threads/ui-detect-a-click-anywhere-except-on-the-ui.516546/
     private void HideIfClickedOutside(List<GameObject> panels) {

        bool isClickedInAnyPanel = false;
        foreach(GameObject obj in panels) {
             bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
                 obj.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 null);

            if(isInside) {
                isClickedInAnyPanel = true;
                break;
            }
                
        }



         if (Input.GetMouseButton(0)  &&
             !isClickedInAnyPanel) {
                     close();
         }
     }

     void Update() {
        if(open) {
            if(panels.Count == 0)
                fillPanels();
            
             if(AnimationUtil.isClickedOutside(panels)) {
                 close();
             }

        }
     }


    public void close() {
        Vector3 closingPosition = new Vector3(Screen.width/2, -Screen.height * 0.2f, 1f);
        GameObject.Find("OtherGames").transform.DOMove(closingPosition, .2f);
        open = false;

    }

    public void show() {


        if(!open) {
            open = true;    
            Vector3 positionToMove = getPositionToMove();
            float height = RectTransformExt.GetWorldRect(GameObject.Find("bottomBar").GetComponent<RectTransform>()).height;

            Sequence sequence=  DOTween.Sequence();
            sequence.Append(GameObject.Find("OtherGames").transform.DOMove(new Vector3(positionToMove.x, positionToMove.y + height/8, 1f), .2f));
            sequence.Append(GameObject.Find("OtherGames").transform.DOMove(positionToMove, .05f));
            sequence.Play();
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

            if(panels.Count == 0) {
                fillPanels();
            }

        } else {
            close();
        }


        //depending on name of 
        
    }


    void fillPanels() {

        panels.Add(GameObject.Find("OtherGames"));
        panels.Add(GameObject.Find("othergamesicon"));

    }

    Vector3 getPositionToMove() {
        originalPosition = GameObject.Find("bottomBar").transform.position;
        float height = RectTransformExt.GetWorldRect(GameObject.Find("bottomBar").GetComponent<RectTransform>()).height;
        float y = GameObject.Find("bottomBar").transform.position.y + height+ height * .2f;
        return new Vector3(Screen.width/2, y, 1f);
        //Formulat 
        //
        //x = Screen.width.2 y = bottomBar Top + Bottom bar height * .2f


    }



}