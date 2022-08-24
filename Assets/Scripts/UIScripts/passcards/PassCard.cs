using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PassCard : MonoBehaviour
{

    public  bool isFilled = false;

    public int boxNumber;

    public Card card;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isBoxFilled() {
        return isFilled;
    }

    public void fill(GameObject cardObject, int cardPosition) {
        Vector3 moveTo = this.gameObject.transform.Find("bkg").transform.position;
        cardObject.transform.DOMove(moveTo, 0.5f);       

        cardObject.transform.DOScale(this.gameObject.transform.Find("bkg").transform.localScale, 0.5f);       

        card = cardObject.GetComponent<CardScript>().card;

        isFilled = true;
    }

    public void unfill() {
        //Move back to original position

        isFilled = false;
        //card = null;



    }

}
