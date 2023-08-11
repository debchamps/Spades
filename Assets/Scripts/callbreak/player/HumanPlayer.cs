using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player {
    public HumanPlayer(PlayerPosition playerPosition1) {
        this.playerPosition = playerPosition1;

    }

    SettingsManager settingsManager = new SettingsManager();


    ConfigManager configManager = new ConfigManager();

    BrayCardHelper callbreakCardHelper = new BrayCardHelper();

    public override void updateMatchNotification(SpadeMatchState callbreakMatchState, SpadeMatchState.NotificationStatus notificationStatus) {

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BID_START)) {
            updateBid(callbreakMatchState);
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BID)) {
            updateBid(callbreakMatchState);
        }
        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BIDDING_COMPLETE)) {
            updateMove(callbreakMatchState);
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.CARD_DISTRIBUTED)) {
            updateBid(callbreakMatchState);

        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.GAME_COMPLETE)) {
            //updateBiddingComplete(callbreakMatchState.getCurrentGameState());
        }
        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.MATCH_COMPLETE)) {
            //updateBiddingComplete(callbreakMatchState.getCurrentGameState());
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.MOVE)) {
            updateMove(callbreakMatchState);
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.ROUND_COMPLETE)) {
            updateMove(callbreakMatchState);
        }
        
    }



    public void highlightBestMove() {

        var matchState = GamePlay.matchState;
        PlayerPosition nextMovePlayer = GamePlay.matchState.getCurrentGameState().getCurrentRound().nextMovePlayer;
        MoveManeger moveManeger = new MoveManeger();

        Debug.Log("Next player move is " + nextMovePlayer + " and playerPosition is " + playerPosition);
        if(nextMovePlayer.Equals(playerPosition)) {
            Card card = moveManeger.playMove(matchState.getCurrentGameState(), this.getPlayerPosition());

            GamePlay.highlightCard(card);

        }

    }

    
    public void  updateTrumpRevealed(SpadeGameState gameState) {

    }

    public  void updateMove(SpadeMatchState matchState){
        PlayerPosition nextMovePlayer = matchState.getCurrentGameState().getCurrentRound().nextMovePlayer;
        MoveManeger moveManeger = new MoveManeger();
        Debug.Log("Next player move is " + nextMovePlayer + " and playerPosition is " + playerPosition);
        if(nextMovePlayer.Equals(playerPosition)) {
            Card card = moveManeger.playMove(matchState.getCurrentGameState(), this.getPlayerPosition());

            //GamePlay.highlightCard(card);

        }

        if(nextMovePlayer.Equals(playerPosition)) {

            List<Card> currentValidCards = callbreakCardHelper.validCards(matchState.getCurrentGameState(), this.playerPosition);

            if(currentValidCards.Count == 1) {
                BraySettings braySettings = settingsManager.getBraySettings();
                if(braySettings.autothrowEnabled) {
                    SpadeMove callbreakMove = new SpadeMove(playerPosition, currentValidCards[0]); 
                    callbreakMove.autoPlay = true;

                    GameObject cardObj = GameObjectFinder.findCardGameObject(currentValidCards[0]);
                    if(!cardObj.GetComponent<CardScript>().isDragging)
                        CallbreakPlayerHelper.updateMove(matchState, callbreakMove);

                }

            }            
            
            //Play a card with some delay.
        } else {
            //Skip. Any intelligence can come later. Like someone trumping
        }


    }

    public void updateBidComplete(SpadeGameState gameState){}
    public void setHand(List<Card> cards){}
    public PlayerPosition getPlayerPosition() {return playerPosition;}
    public void updateTrumpSet(SpadeGameState gameState){}

    public Card nextMove(SpadeGameState gameState){return null;}


    public void updateBid(SpadeMatchState callbreakMatchState)  {
        bool myMove = isMyMove(callbreakMatchState.getCurrentGameState());
        Debug.Log("Human Bid myMove " + myMove + " " + callbreakMatchState.getCurrentGameState().callBreakBidding.callBreakBiddingData.nextBiddingPosition);

        if(!myMove)
        return;

        CallbreakBid bid = bestBid(callbreakMatchState);

        //System.Threading.Thread.Sleep(configManager.callbreakTimingConfig.delayBetweenMoves);


        //Debug.Log("Human Bid is " + bid.playerPosition + " of " + bid.bidValue);
        //callbreakMatchState.updateBid(bid);

    }


    CallbreakBid bestBid(SpadeMatchState callbreakMatchState) {

        int  bid = 4;
        CallbreakBid callbreakBid = new CallbreakBid(123, PlayerPosition.SOUTH, bid);

        return callbreakBid;
    }


}