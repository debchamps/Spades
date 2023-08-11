using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PassBoxScript : MonoBehaviour
{   
    /*
    public static int cardsMovedToBox = 0;
    string TAG = "PassBoxScriptTag";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPosition() {
        GameObject obj = GameObject.Find("passcardparent");
        Rect cardRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("testCard1").transform);
        Rect dialgueRct = RectTransformExt.GetWorldRect((RectTransform)obj.transform);

        Debug.Log("dialgueRct.height" + dialgueRct.height + " dialgueRct.width" + dialgueRct.width);
        Vector3 card1Pos = GameObject.Find("ScriptEmpty").GetComponent<DeckDistribution>().getCardPositionInScreen(1);
        GameObject.Find("passcardparent").transform.position = new Vector3(Screen.width/2, card1Pos.y + cardRect.height  + .5f *dialgueRct.height  , 1f);

    }

    public bool isCardPassed(Card cardInput){

        for(int i=1;i<=3;i++) {
            
            if(GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().isFilled) {
                if(GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().card.getCardId().Equals(cardInput.getCardId())) {
                    return true;
                }
            }
        }
        return false;
    }



    public void show() {

        Rect cardRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("testCard1").transform);
        Rect dialgueRct = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("passcardparent").transform);
        Vector3 card1Pos = GameObject.Find("ScriptEmpty").GetComponent<DeckDistribution>().getCardPositionInScreen(1);
        transform.position = new Vector3(Screen.width/2, cardRect.y  , 1f);        

        Vector3 localScale = GameObject.Find("passcardparent").transform.localScale;
        //GameObject.Find("passcardparent").transform.DOScale(0, 0f);
        GameObject.Find("passcardparent").transform.position = new Vector3(Screen.width * 5, card1Pos.y + cardRect.height * 1f+ .5f * dialgueRct.height  , 1f);

        PlayerPosition pass = GamePlay.callbreakMatchState.getCurrentGameState().playerToPass(PlayerPosition.SOUTH);
        Debug.Log(TAG + " Player To Pass " + pass);
        GameObject btnObj = GameObject.Find("passcardbtn/buttonText");
        if(pass.Equals(PlayerPosition.EAST)) {
            btnObj.GetComponent<Text>().text = "Pass Right";

        }
        if(pass.Equals(PlayerPosition.SOUTH)) {
            //btnObj.GetComponent<Text>().text = "Pass Right";
            
        }
        if(pass.Equals(PlayerPosition.NORTH)) {
            btnObj.GetComponent<Text>().text = "Pass Opposite";
            
        }
        if(pass.Equals(PlayerPosition.WEST)) {
            btnObj.GetComponent<Text>().text = "Pass Left";
            
        }

        Sequence sequence=  DOTween.Sequence();
        sequence.AppendInterval(4f);
        sequence.AppendCallback(setPosition);
        sequence.AppendInterval(.5f);
        sequence.Append(GameObject.Find("passcardparent").transform.DOScale(localScale, .5f));
        sequence.Play();


    }


    public void close() {

        GameObject.Find("passcardparent").transform.position = new Vector3(Screen.width * 5, Screen.height/2, 1f);

        GameObject.Find("cardCombo1").GetComponent<PassCard>().unfill();
        GameObject.Find("cardCombo2").GetComponent<PassCard>().unfill();
        GameObject.Find("cardCombo3").GetComponent<PassCard>().unfill();

    }



    public bool canMoveCard() {
        if(getFirstEmptyBox() != null)
            return true;
        return false;
    }
    
    public void moveCardToBox(GameObject cardObject, int cardPos) {

        GameObject firstEmptyBox = getFirstEmptyBox();
        if(firstEmptyBox != null) {
            firstEmptyBox.GetComponent<PassCard>().fill(cardObject, cardPos);
        }

    }


    public void removeCardFromPassBox(Card cardInput) {

        for(int i=1;i<=3;i++) {
            
            if(GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().isFilled) {
                if(GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().card.getCardId().Equals(cardInput.getCardId())) {
                    GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().unfill();
                }
            }
        }


    }




    public GameObject getFirstEmptyBox() {
        bool isBox1Filled = GameObject.Find("cardCombo1").GetComponent<PassCard>().isBoxFilled();
        if(!isBox1Filled) {
            return GameObject.Find("cardCombo1");
        }
        bool isBox2Filled = GameObject.Find("cardCombo2").GetComponent<PassCard>().isBoxFilled();
        if(!isBox2Filled) {
            return GameObject.Find("cardCombo2");
        }
        bool isBox3Filled = GameObject.Find("cardCombo3").GetComponent<PassCard>().isBoxFilled();
        if(!isBox3Filled) {
            return GameObject.Find("cardCombo3");
        }

        return null;

    }


    */
}
