using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHelper {

        CardHelper cardHelper = new CardHelper();
        BrayCardHelper callbreakCardHelper = new BrayCardHelper();

        CardDeck cardDeck = new CardDeck();

        public Dictionary<string, int> suitPlayedMap(SpadeGameState callbreakGameState) {
        Dictionary<string, int> suitPlayedCountMap = new Dictionary<string, int>();
        foreach(string suit in CardHelper.SUITS)
        {
            suitPlayedCountMap[suit] = 0;
        }

        foreach(SpadeRound round in callbreakGameState.rounds)
        {
            if(round.roundSuit!=null) {
            suitPlayedCountMap[round.roundSuit] += 1; 

            }
        }

        return suitPlayedCountMap;
    }



    public Card highestRemainingCard(SpadeGameState callbreakGameState,  string suit) {

        //Everyone have atleast one suit.
        List<Card> remainingCards = allRemainingCard(callbreakGameState, suit);
        if(remainingCards.Count == 0)
            return null;

        callbreakCardHelper.sortByRank(remainingCards);
        return remainingCards[0];

    }


    public List<Card> allPlayedCard(SpadeGameState callbreakGameState,  string suit) {
        List<Card> cards = new List<Card>();
        foreach(SpadeRound round in callbreakGameState.rounds) {
            foreach(SpadeMove move in round.moves) {
                cards.Add(move.card);
            }
        }
        return cards;
    }


    public List<Card> allRemainingCard(SpadeGameState callbreakGameState,  string suit) {
        List<Card> allSuitedCards = cardDeck.getSuitAll(suit);

        List<Card>  allPlayed = allPlayedCard(callbreakGameState, suit);

        return cardHelper.minus(allSuitedCards, allPlayed);

    }

}