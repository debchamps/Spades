
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallbreakPlayerHelper : MonoBehaviour {

    static ConfigManager configManager = new ConfigManager();

    public static void updateMove(SpadeMatchState callbreakMatchState, SpadeMove move) {
        GameObject.Find("ScriptEmpty").GetComponent<CallbreakPlayerHelper>().updateMoveExecute(callbreakMatchState, move);
    }


    public void updateMoveExecute(SpadeMatchState callbreakMatchState, SpadeMove move) {
        StartCoroutine(updateMoveEnumerator(callbreakMatchState, move));
    }


    public  IEnumerator updateMoveEnumerator(SpadeMatchState callbreakMatchState, SpadeMove callbreakMove) {
        //CallbreakMove lastMove = callbreakMatchState.getCurrentGameState().getCurrentRound().getCurrentMove();
        //PlayerPosition nextMovePlayer = callbreakMatchState.getCurrentGameState().getCurrentRound().nextMovePlayer;

            if(callbreakMatchState.getCurrentGameState().getCurrentRound().moves.Count == 0) {

                if(callbreakMatchState.getCurrentGameState().rounds.Count <=1) {
                    if(callbreakMatchState.gameVariant.Equals(GameVariant.HEARTS)) {
                        yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayBetweenPassingFirstRoundFirstMove/1000);

                    } else {
                        yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayBetweenFirstRoundFirstMove/1000);

                    }

                } else {
                    yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayBetweenRoundFirstMove/1000);
                }
                

            } else {
                yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayBetweenMoves/1000);

            }

            //Play a random Move now.
            if(callbreakMatchState.getCurrentGameState().getCurrentRound().nextMovePlayer.Equals(callbreakMove.playerPosition))
                callbreakMatchState.updateMove(callbreakMove);
            else 
                Debug.Log("FATAL : Duplicate call coming for " + callbreakMove.card.getCardId() + " for player " + callbreakMove.playerPosition);
            
            
            //Play a card with some delay.
    }

    public static void updateBid(SpadeMatchState callbreakMatchState, CallbreakBid bid) {
        GameObject.Find("ScriptEmpty").GetComponent<CallbreakPlayerHelper>().updateBidExecute(callbreakMatchState, bid);

    }

    public  void updateBidExecute(SpadeMatchState callbreakMatchState, CallbreakBid bid) {
        StartCoroutine(updateBidEnumerator(callbreakMatchState, bid));

    }



        public static IEnumerator updateBidEnumerator(SpadeMatchState callbreakMatchState, CallbreakBid bid)  {
        

        if(callbreakMatchState.getCurrentGameState().callBreakBidding.callBreakBiddingData.biddingStarter.Equals(bid.playerPosition))
            yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayFirstBid/1000);
        else
            yield return new WaitForSeconds(configManager.callbreakTimingConfig.delayBetweenBid/1000);
        
        callbreakMatchState.updateBid(bid);

        }
        

    


}