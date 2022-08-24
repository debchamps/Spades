using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class DubugShowPlayerCards : MonoBehaviour  {

    public PlayerPosition pos;

    public void abc(PlayerPosition pos) {

    }

    public void showPlayerCards() {
        
        if(!GamePlay.IS_DEBUG)
            return;

        var game = GamePlay.matchState.getCurrentGameState();

        if(!game.gameStatus.Equals(SpadeGameState.GameStatus.IN_PROGRESS))
            return;

        //First fill the cards which are played by the player.
        int counter = 1;
        List<string> cardIds = new List<string>();
        foreach(SpadeRound round in game.rounds) {
            if(round.roundCards.ContainsKey(pos)) {
                string path =   "Cards/" +  round.roundCards[pos].getCardId().ToLower();
                GameObject.Find("debugshowhand/card" + counter.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
                cardIds.Add(round.roundCards[pos].getCardId());
                counter = counter + 1;
            }
        }

        List<Card> allPlayerCards = game.playerCardMap[pos];

        foreach(Card card in allPlayerCards) {
            if(!cardIds.Contains(card.getCardId())) {
                string path =   "Cards/" +  card.getCardId().ToLower();
                GameObject.Find("debugshowhand/card" + counter.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
                counter = counter + 1;
           }
        }


        GameObject.Find("debugshowhand").transform.position = new Vector2(Screen.width/2, Screen.height/2);



    }

    public void close() {
                GameObject.Find("debugshowhand").transform.position = new Vector2(Screen.width* 5, Screen.height * 5);

    }


}