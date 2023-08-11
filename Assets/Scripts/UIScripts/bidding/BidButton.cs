using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using DG.Tweening;

public class BidButton : MonoBehaviour, IPointerClickHandler{
   public bool selected = false;

   public bool deactivated = false;

   public int bidValue;

   public void select() {
       if(deactivated) 
            return;
       selected = true;
       deselectOthers();
       gameObject.GetComponent<Image>().color = new Color32(0,240,0,255);

       //gameObject.transform.DOScale(1.2f, .5f);

        Sequence sequence=  DOTween.Sequence();
        sequence.Append(gameObject.transform.DOScale(1.2f, .25f));
        sequence.Append(gameObject.transform.DOScale(1f, .25f));
    
        sequence.Play();

        GameObject.Find("ScriptEmpty").GetComponent<BidButtonGo>().showContinueButton();       
   }

   public void deactivate() {
        gameObject.GetComponent<Image>().color = new Color32(100,100,100,255);
        deactivated = true;

   }


    public void deselectOthers() {
        List<GameObject> biddingButtons = GameObjectFinder.getBiddingButtons();

        foreach(GameObject btn in biddingButtons) {

            if(btn.GetComponent<BidButton>().bidValue != bidValue && !btn.GetComponent<BidButton>().deactivated) {
                btn.GetComponent<BidButton>().selected = false;
                btn.GetComponent<Image>().color = new Color32(255,255,255,255);
            }

        }


    }


    public void deselect() {
       selected = false;
       deactivated = false;
       gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        select();
        // OnClick code goes here ...
    }


}