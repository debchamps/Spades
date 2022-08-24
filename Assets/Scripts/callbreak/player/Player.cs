using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player: MonoBehaviour {

    public PlayerPosition playerPosition;

    public List<Card> allCards;

    public List<Card> playedCards = new List<Card>();

    public List<Card> remainingCards;

    public void setCards(List<Card> cards) {
        this.allCards = cards;
        this.remainingCards = cards;
    }

    public void initFromGameState(SpadeGameState gameState) {
        allCards = gameState.playerCardMap[playerPosition];
        foreach(SpadeRound round in gameState.rounds) {
            foreach(SpadeMove move in round.moves) {
                if(move.playerPosition.Equals(playerPosition)) {
                    playedCards.Add(move.card);
                }
            }
        }
        remainingCards = new CardHelper().minus(allCards, playedCards);

    }

    CardHelper cardHelper = new CardHelper();
    BrayCardHelper callbreakCardHelper = new BrayCardHelper();

    public string getId() {return "";}

    public  abstract void updateMatchNotification(SpadeMatchState callbreakMatchState, SpadeMatchState.NotificationStatus notificationStatus);


    public bool isMyMove(SpadeGameState gameState) {

        
        Debug.Log("Current bidding position is " + gameState.callBreakBidding.callBreakBiddingData.nextBiddingPosition);
        if(gameState.gameStatus.Equals(SpadeGameState.GameStatus.BIDDING)) {
            if(gameState.callBreakBidding.callBreakBiddingData.nextBiddingPosition.Equals(playerPosition))
                return true;
            return false;

        }
        

        if(gameState.gameStatus.Equals(SpadeGameState.GameStatus.IN_PROGRESS)) {
            if(gameState.getCurrentRound().roundState.Equals(SpadeRound.RoundState.STARTED)) {
            if(gameState.getCurrentRound().nextMovePlayer.Equals(playerPosition))
                return true;
            }
        return false;   

        }
        return false;
    }



    public List<Card> validCards(SpadeGameState callbreakGameState) {
        return callbreakCardHelper.validCards(callbreakGameState , this.playerPosition);

    }


}