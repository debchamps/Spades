using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ScoreIconScript : MonoBehaviour, IPointerClickHandler
{ 
    public void OnPointerClick(PointerEventData eventData)
    {

        if(GamePlay.matchState != null && GamePlay.matchState.getCurrentGameState() != null && GamePlay.matchState.getCurrentGameState().getCurrentRound()!=null ) {
            SpadeGameState currGame = GamePlay.matchState.getCurrentGameState();
            Debug.Log("currGame.gameNumber " + currGame.gameNumber + "currGame.getCurrentRound().roundNumber" + currGame.getCurrentRound().roundNumber);
            if(GamePlay.matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
                return;
            //if(currGame.getCurrentRound().roundNumber == 13)
            //    return;
            if(currGame.gameNumber == 5 && currGame.getCurrentRound().roundNumber == 13)
                return;

        }

        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().enableScoreCardView(GamePlay.matchState);
        AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

    }
}
