using System.Collections.Generic;
using UnityEngine;

public class ComputerBidder {

    string TAG = "ComputerBidder";
    CardHelper cardHelper = new CardHelper();
    BrayCardHelper brayCardHelper = new BrayCardHelper();

    public int calculateBid(SpadeGameState game, List<Card> cards, PlayerPosition pos) {
        //Get set state.
        //Calculate number of spades.
        int maxAllowedBid = game.maxAllowedBid(pos);

        int initialBid = totalBid(cards);


        //if(initialBid <=2)
         //   return 0;


        
        if(initialBid ==0) {
            bool canDONIL = canProceedWithNIL(cards);
            if(canDONIL)
                return 0;
        }
        
        return min(max(1,totalBid(cards)),min(8,maxAllowedBid));

        
    }

    bool canProceedWithNIL(List<Card> cards) {

        Dictionary<string, List<Card>> cardGroups = cardHelper.groupBySuit(cards);

        foreach(string suit in cardGroups.Keys) {
            if(suit.Equals("S")) {
                if(cardGroups[suit].Count > 3)
                    return false;

                List<Card> highSpades = brayCardHelper.filterHigherCard(cardGroups[suit], new Card(suit, "J"));
                    if(highSpades.Count >=1)
                        return false;


            } else {
                if(cardGroups[suit].Count <= 3) { 
                    List<Card> highCards = brayCardHelper.filterHigherCard(cardGroups[suit], new Card(suit, "J"));
                    if(highCards.Count >=1)
                        return false;
                }
            }

        }
        return true;

    }




    public int bounusSpadePoints() {
        return 0;
    }

    public int max(int a, int b) {
        if(a>b) return a;
        return b;
    }


    public int totalBid(List<Card> cards) {
        double totalPoint = 0, nonSuit = 0;
        foreach(Card card in cards) {
            nonSuit = nonSuit + pointProbabilityNonsuitCard(card, cards);
        }

        Debug.Log(TAG + "pointProbabilityNonsuitCard " + nonSuit);

        int pointTotalTrump = pointTrump(cards);

        Debug.Log(TAG + "pointTotalTrump " + pointTotalTrump);

        double extraTrumpPoint = extraTrumpSuitPoint(cards);

        double bidDeduction = 0;

        if(totalPoint >= 8)
            bidDeduction = 2;
        else if(totalPoint >= 5)
            bidDeduction = 1;


        totalPoint = nonSuit + pointTotalTrump + extraTrumpPoint - bidDeduction; 



        //DebugLog.Log("totalPoint : " + totalPoint + "nonSuit: " + nonSuit + " pointTotalTrump: " + pointTotalTrump + "extraTrumpPoint " + extraTrumpPoint);

        int bid = (int)System.Math.Round(totalPoint);
        return bid;
    }


    public double extraTrumpSuitPoint(List<Card> cards) {

        List<Card> spades = cardHelper.filterBySuit(cards, "S");
        bool spadeAce = cardHelper.contains(spades, "S", "A");
        bool spadeK = cardHelper.contains(spades, "S", "K");

        double limit  = 5;
        if(spadeAce && spadeK)
            limit = 4;
        else if(spadeAce)
            limit = 4.5;
        return Mathf.Max (0.0f, (float)(spades.Count - limit));

    }

    public double pointProbabilityNonsuitCard(Card card, List<Card> allCard) {
        //If only a single 'K', return 0
        //If 'A' and 'K' return 1 if count = 2, .5 if count - 3, .1 if count =4  

        int suitedCount = cardHelper.filterBySuit(allCard, card.suit).Count;

        if(card.number.Equals("A"))
        return 1;


        if(card.number.Equals("K")){

            if(cardHelper.contains(allCard, card.suit, "A")) {
                if(suitedCount == 2) return 1;
                if(suitedCount == 3) return .8;
                if(suitedCount == 4) return .3;
                if(suitedCount >= 5) return .1;
            } else {
                if(suitedCount == 1) return 0;
                if(suitedCount == 2) return .5;
                if(suitedCount == 3) return .6;
                if(suitedCount == 4) return .3;
                if(suitedCount >= 5) return .1;
            }
            //If contains A, {if count <=4} then return .8, else .1}
            //else {if count=2 return 0 if count = 3 return .5 if count = 4 return .1 else return 0}
            return 1;

        }

        if(card.number.Equals("Q")){

            if(cardHelper.contains(allCard, card.suit, "A")) {
                if(suitedCount == 2) return .4;
                if(suitedCount == 3) return .4;
                if(suitedCount == 4) return .1;
                if(suitedCount >= 5) return 0;
            } else {
                if(suitedCount == 1) return 0;
                if(suitedCount == 2) return .4;
                if(suitedCount == 3) return .3;
                if(suitedCount == 4) return 0;
                if(suitedCount >= 5) return 0;
            }


        } 

        


        return 0.0;

    }

    public int nonSuitedPoint(List<Card> cards) {

        List<Card> hearts = cardHelper.filterBySuit(cards, "H");
        List<Card> spades = cardHelper.filterBySuit(cards, "S");
        List<Card> clubs = cardHelper.filterBySuit(cards, "C");
        List<Card> diamonds = cardHelper.filterBySuit(cards, "D");

        return 0;

    }

    public int pointTrump(List<Card> cards) {

        List<Card> hearts = cardHelper.filterBySuit(cards, "H");
        List<Card> spades = cardHelper.filterBySuit(cards, "S");
        List<Card> clubs = cardHelper.filterBySuit(cards, "C");
        List<Card> diamonds = cardHelper.filterBySuit(cards, "D");


        int trumpable = posOrZero(3 - hearts.Count) + posOrZero(3 - clubs.Count) + posOrZero(3 - diamonds.Count);

        Debug.Log(TAG + "trumpable " + trumpable);

        if(spades.Count == 0) return 0;

        if(spades.Count == 1) return 0;

        if(spades.Count == 2)  {
            return min(1, trumpable);

        }

        if(spades.Count == 3)  {
            return min(2, trumpable);

        }

        if(spades.Count > 3)  {
            return min(3, trumpable);
        }

        //If suit count is 1, then 
        //If suit count is 2, then 

        return 0;
    }

    int min(int x, int y) {
        if(x>y) return y;
        return x;
    }

    int posOrZero(int x) {
        if(x<0) 
        return 0;
        return x;    
    }

}