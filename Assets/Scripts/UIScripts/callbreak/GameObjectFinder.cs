
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectFinder : MonoBehaviour {
    public static List<GameObject> getCardObjects() {
        List<GameObject> gameObjects = new List<GameObject>();
        for(int i=1;i<=13;i++)
        gameObjects.Add(GameObject.Find("testCard" + i.ToString()) );
        return gameObjects;

    }


    public static List<GameObject> getBiddingButtons() {
        List<GameObject> gameObjects = new List<GameObject>();
        for(int i=1;i<=14;i++)
        gameObjects.Add(GameObject.Find("spadeBidButton" + i.ToString()) );
        return gameObjects;

    }


    public static GameObject findCardGameObject(Card card) {
           List<GameObject> gameObjects =  getCardObjects();

           foreach(GameObject go in gameObjects) {
                Card cardInObject = go.GetComponent<CardScript>().card;
                if(cardInObject.getCardId().Equals(card.getCardId())) {
                    return go;
                }

           }
        return null;
    }


    public static List<GameObject> playerAvatar() {
           List<GameObject> gameObjects =  new List<GameObject>();
           foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
               gameObjects.Add(GameObject.Find("player" + PlayerPositionHelper.getName(pos).ToLower() + "avatar"));
           }
           return gameObjects;
    }


    public static GameObject getPlayerScoreObject(PlayerPosition pos) {

        return GameObject.Find("player" + PlayerPositionHelper.getName(pos).ToLower() + "score");

    }
}