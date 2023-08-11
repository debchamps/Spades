using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CallbreakPlayer : Player {

    MoveManeger moveManager = new MoveManeger();

    ConfigManager configManager = new ConfigManager();
    public enum PlayerType {
        HUMAN,
        COMPUTER
    }

    int playerId;
    PlayerType playerType;



    public CardHelper cardHelper = new CardHelper();

    BrayCardHelper callbreakCardHelper = new BrayCardHelper();

    ComputerBidder computerBidder = new ComputerBidder();


    public void updateBid(SpadeMatchState callbreakMatchState)  {

        
        bool myMove = isMyMove(callbreakMatchState.getCurrentGameState());

        Debug.Log("Bidding isMyMove " + myMove + " for Player " + playerPosition);
        if(myMove) {

        
        CallbreakBid bid = bestBid(callbreakMatchState);
        CallbreakPlayerHelper.updateBid(callbreakMatchState, bid);
        }
        

    }

    public void updateMove(SpadeMatchState callbreakMatchState) {
        //CallbreakMove lastMove = callbreakMatchState.getCurrentGameState().getCurrentRound().getCurrentMove();
        //PlayerPosition nextMovePlayer = callbreakMatchState.getCurrentGameState().getCurrentRound().nextMovePlayer;
        PlayerPosition nextMovePlayer = callbreakMatchState.getCurrentGameState().getCurrentRound().nextMovePlayer;

        Debug.Log("Next player move is " + nextMovePlayer + " and playerPosition is " + playerPosition);
        if(nextMovePlayer.Equals(playerPosition)) {

            SpadeMove currentMove = bestMove(callbreakMatchState);

            Debug.Log("nextMovePlayer is " + nextMovePlayer + " and player position is " + playerPosition
            + " and playing move is " + currentMove.card.getCardId());

            //Play a random Move now.
            CallbreakPlayerHelper.updateMove(callbreakMatchState, currentMove);
                        
            //Play a card with some delay.
        } else {
            //Skip. Any intelligence can come later. Like someone trumping
        }
    }

    public void updateBidComplete(SpadeMatchState callbreakMatchState)  {
        updateMove(callbreakMatchState);

    }

    public async override void updateMatchNotification(SpadeMatchState callbreakMatchState, SpadeMatchState.NotificationStatus notificationStatus) {
        Debug.Log("Processing match notification " + notificationStatus + " at player " + playerPosition);

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
            //updateRoundComplete(callbreakMatchState.getCurrentGameState());
        }
        
    }


    public SpadeMove bestMove(SpadeMatchState callbreakMatchState) {

        List<Card> cards = validCards(callbreakMatchState.getCurrentGameState());
        Card playCard = moveManager.playMove(callbreakMatchState.getCurrentGameState(), playerPosition);
        //Card playCard = cards[0];
        SpadeMove callbreakMove = new SpadeMove(playerPosition, playCard); 
        Debug.Log("Best move is "+ playCard.getCardId() + " for player " + playerPosition + " by moveManager");
        return callbreakMove;

    }


    CallbreakBid bestBid(SpadeMatchState callbreakMatchState) {
        int  bid = 2;
        int calculatedBid = computerBidder.calculateBid(callbreakMatchState.getCurrentGameState(), this.remainingCards, playerPosition);
        CallbreakBid callbreakBid = new CallbreakBid(123, this.playerPosition, calculatedBid);
        return callbreakBid;
    }

    public void setHandCards(List<Card> cards) {
        this.allCards = cards;
        this.remainingCards = cards;
    }

    public void playCard(SpadeGameState callbreakGameState) {
        List<Card> validCardList = validCards(callbreakGameState);
        moveManager.playMove(callbreakGameState, playerPosition);
        Card cardToPlay = validCardList[0];
    }


}