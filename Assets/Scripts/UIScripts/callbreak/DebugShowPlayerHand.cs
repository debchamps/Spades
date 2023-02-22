using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.EventSystems;

public class DebugShowPlayerHand : MonoBehaviour  , IPointerClickHandler {

    public static bool open = false;

    public PlayerPosition pos;

    public GameObject dialogue;

    public void close() {
        open = false;

        dialogue.transform.position = new Vector3(Screen.width * 2,Screen.height*4f/5,0);


    }

    public void showHand(SpadeMatchState callbreakMatchState) {

        dialogue.transform.position = new Vector3(Screen.width/2,Screen.height/2,0);

        open = true;


        List<Card> remainingCards = callbreakMatchState.getCurrentGameState().playerMap[pos].remainingCards;
        List<Card> playedCards = callbreakMatchState.getCurrentGameState().playerMap[pos].playedCards;

        int indx= 1;
        if(playedCards != null) {
            for(int i=0;i<playedCards.Count;i++) {
                string path =   "Cards/" +  playedCards[i].getCardId().ToLower();

                    if(indx <=13) {
                        GameObject.Find("card"+ indx.ToString());
                        GameObject.Find("card"+ indx.ToString()).GetComponent<Image>();
                        GameObject.Find("card"+ indx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
                        indx++;

                    }
            }
        }

        if(remainingCards != null) {
            for(int i=0;i<remainingCards.Count;i++) {
                string path =   "Cards/" +  remainingCards[i].getCardId().ToLower();
                if(indx <=13) {
                    GameObject.Find("card"+ indx.ToString());
                    GameObject.Find("card"+ indx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
                    indx++;
                }
            }
        }


    }


    public void OnPointerClick(PointerEventData eventData)
    {

        Debug.Log("Pointer clicked. YAY");

        if(!open)
        showHand(GamePlay.matchState);
        else
        close();

        //Move it by 2* Screen width.
        // OnClick code goes here ...
    }

    


    

}