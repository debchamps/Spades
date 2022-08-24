using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubsequentMove {
    CardHelper cardHelper = new CardHelper();
    BrayCardHelper callbreakCardHelper = new BrayCardHelper();

    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    MoveHelper moveHelper = new MoveHelper();

    string TAG = "SubsequentMove";
    public bool shouldLog = false;

    int getTeamNo(PlayerPosition pos) {
        if(pos.Equals(PlayerPosition.NORTH) || pos.Equals(PlayerPosition.SOUTH))
            return 1;
        return 2;
    }

    int getOtherTeamNo(int ourTeam) {
        if(ourTeam ==1)
            return 2;
        return 1;
    }


    public Card playToLose(SpadeGameState gameState, PlayerPosition pos) {

        //If I have passed 

        return null;        
    }


    public Card playMove(SpadeGameState callbreakGameState, PlayerPosition pos) {

            Card card = decideStrategy(callbreakGameState, pos);
            if(card != null) {
                return card;
            }
            return null;
    }

    public Card partnerNilBustSaver(SpadeGameState gameState, PlayerPosition pos) {

        SpadeMove winningMove = gameState.getCurrentRound().findWinningMove();

        return null;
    }


    public Card opponentNilBustEnforcer(SpadeGameState gameState, PlayerPosition pos) {
        //Play low card is oppnent is NIL and they have a winning card.
        SpadeMove winningMove = gameState.getCurrentRound().findWinningMove();



        

        return null;

    }


    public Card decideStrategy(SpadeGameState gameState, PlayerPosition pos) {
        

        if(gameState.isNil(pos)) {
            return nilStrategy(gameState, pos);
        }

        int teamNo = getTeamNo(pos);
        int otherTeamNo =  getOtherTeamNo(teamNo);

        int target = gameState.getTarget(teamNo);

        int achieved = gameState.getTricksWon(teamNo);


        int opponentTarget = gameState.getTarget(otherTeamNo);

        int opponentAchieved = gameState.getTarget(otherTeamNo);

        /*
        if(gameState.isNil(winningMove.playerPosition) && !gameState.isPlayerNILBusted(winningMove.playerPosition)) {
            if(!winningMove.playerPosition.Equals(playerPositionHelper.getOppositePlayerPosition(pos))) {
                Card opponentNilEnforcer = opponentNilBustEnforcer(gameState, pos);
                if(opponentNilEnforcer != null)
                    return opponentNilEnforcer;

    //Opponent is winning.
            } else {
                Card patnerBustSaverCard = partnerNilBustSaver(gameState, pos);
                if(patnerBustSaverCard != null)
                    return patnerBustSaverCard;
            }
        }
        */
        

        if(achieved < target) {
            return playToWin(gameState, pos);
        } else {

            SpadeMove winningMove = gameState.getCurrentRound().findWinningMove();

            //Incase winning move is 'Opponent' and potential nil bust.


            //Incase winning move is 'Partner' and potential nil bust.

            if(!gameState.isSandBagEnabled)
                return playToWin(gameState, pos);

            if(opponentTarget < opponentAchieved) {
                return playToLose(gameState, pos);
            }

            return playToWin(gameState, pos);
        }

        // If I have called  NIL Strategy.
        // ELSE
        // If OUR TEAM TOTOAL HAVE NOT CROSSED
        //return playToWin();
        //else 
        //return playToLose();

        return null;


    }

    public Card nilStrategy(SpadeGameState gameState, PlayerPosition pos) {

        List<Card> cards = callbreakCardHelper.validCards(gameState, pos);
        Dictionary<string, List<Card>>  groupedCard = cardHelper.groupBySuit(cards);
        List<Card> roundSuitCards = groupedCard[gameState.getCurrentRound().roundSuit];

        callbreakCardHelper.sortByRank(roundSuitCards);


        if(gameState.tricksWinnerCount[pos] > 0) {
            //May change logic
            return playToWin(gameState, pos);
        }

        SpadeMove winningMove = gameState.getCurrentRound().findWinningMove();

        if(winningMove.card.suit.Equals("S") && !gameState.getCurrentRound().roundSuit.Equals("S")) {
            if(roundSuitCards.Count > 0 ) {
                return roundSuitCards[0];
            }

            callbreakCardHelper.sortByRank(cards);
            for(int i=0;i<cards.Count;i++) {
                if(!cards[i].suit.Equals("S")) {
                    return cards[i];
                }
            }
            //Smallest spade.
            return cards[cards.Count -1];
            //Play the highest card which is not spade.

            //If I have round suits. Play the highest round suit.

        } else {
            callbreakCardHelper.sortByRank(cards);
            if(roundSuitCards.Count > 0) {
                List<Card> lowerCards = callbreakCardHelper.filterLowerCard(roundSuitCards, winningMove.card);
                if(lowerCards.Count > 0) {
                    
                    return lowerCards[0];
                    
                } else {
                    //Play the low card, chance is there someone will have a higher card
                    return roundSuitCards[roundSuitCards.Count -1];
                }

            } else {

                //If winning suit is 'S' and I have a low 'S' then play it.
                if(winningMove.card.suit.Equals("S")) {
                        List<Card> spades = cardHelper.filterBySuit(cards, "S");
                        if(spades != null && spades.Count > 0) {
                            List<Card> lowerSpades = callbreakCardHelper.filterLowerCard(spades, winningMove.card);
                            if(lowerSpades != null && lowerSpades.Count > 0) {
                                callbreakCardHelper.sortByRank(lowerSpades);
                                return lowerSpades[0];

                            }

                        }


                }
                
                for(int i=0;i<cards.Count;i++) {
                    if(!cards[i].suit.Equals("S")) {
                        //Play a high card.
                        return cards[i];
                    }
                }
                //Smallest spade.
                return cards[cards.Count -1];

            }


        }

        //If winning move is same as round suit.

        //Incase winning move is trump, Play the highest suit. 


        callbreakCardHelper.sortByRank(cards);
        return cards[0];
    }

    public Card playToWin(SpadeGameState callbreakGameState, PlayerPosition pos) {

        //If i have suited card. Check no of round played. Play 'A' if you have
        // If not 'A' , play the KQ strategy.

        //Play the lowest card.

        List<Card> validCards = callbreakCardHelper.validCards(callbreakGameState, pos);
        string roundSuit = callbreakGameState.getCurrentRound().roundSuit;
        int positionNo = callbreakGameState.getCurrentRound().moves.Count + 1;

        Dictionary<string, List<Card>>  groupedCard = cardHelper.groupBySuit(validCards);

        if(groupedCard[roundSuit].Count > 0) {

            Card suitedCardPlay = suitedStrategy(validCards, callbreakGameState, positionNo, pos);
            Debug.Log( "suitedStrategy: " + suitedCardPlay.getCardId());
            return suitedCardPlay;

            //Play suited card
        }

        //can trump.



        bool isTrumpMove = false;


        if(groupedCard.ContainsKey("S") && !roundSuit.Equals("S")) {
            //Play trump card
            Card trump = trumpMove(validCards, callbreakGameState, pos);
            //DebugLog.Log( "TrumpStrategy: " + trump.getCardId(), shouldLog);
            if(trump != null) {
                Debug.Log(TAG + "TrumpStrategy Card play is " + trump.getCardId());
                return trump;

            }

        }

        Card nonTrumpNonSuit =  nonTrumpNonSuitMove(validCards, callbreakGameState);
        //DebugLog.Log( "nonTrumpNonSuitMove : " + nonTrumpNonSuit.getCardId(), shouldLog);
        Debug.Log(TAG + "nonTrumpNonSuitMove Card play is " + nonTrumpNonSuit);
        return nonTrumpNonSuit;


        //If 

    }

    public Card suitedStrategy(List<Card> cards, SpadeGameState game, int position, PlayerPosition pos) {

       if(cards.Count == 1)  {
            Debug.Log(TAG + "SingleCardLeft play is " + cards[0].getCardId());
            return cards[0]; 

       }

        bool isPartnerTryingNil = false;
        bool isOpponentTryingNil = false;



        PlayerPosition winningPos = game.getCurrentRound().findWinningMove().playerPosition;

        if(playerPositionHelper.getOppositePlayerPosition(pos).Equals(winningPos)) {
            isPartnerTryingNil  = game.isPlayerNIL(winningPos) && !game.isPlayerNILBusted(winningPos);

        } else {
            isOpponentTryingNil =  game.isPlayerNIL(winningPos) && !game.isPlayerNILBusted(winningPos);
        }

        Card winningCard = game.getCurrentRound().findWinningMove().card;
        List<Card> higherCards = callbreakCardHelper.filterHigherCard(cards, winningCard);
        List<Card> lowerCards = callbreakCardHelper.filterLowerCard(cards, winningCard);

        
        callbreakCardHelper.sortByRank(cards);



        if(position == 4) {
            //Return lowest card.
            //Do I have a winning card. If my partner is winning.

            if(playerPositionHelper.getOppositePlayerPosition(pos).Equals(winningPos)) {

                //If Partner is trying NIL. Try to win the hand.

                if(isPartnerTryingNil) {
                    //Try to play the next biggest card.
                    //return cards[cards.Count -1];
                    if(higherCards.Count > 0)
                       return higherCards[higherCards.Count -1];
                    else 
                       return cards[cards.Count -1];

                } else {
                    Debug.Log(TAG + "4PosPartnerWinningLowCard" + cards[cards.Count -1].getCardId());
                    DebugLog.Log(TAG + "4PosPartnerWinningLowCard" + cards[cards.Count -1].getCardId());
                    return cards[cards.Count -1];

                }


            } else {
                if(higherCards.Count > 0 && !isOpponentTryingNil) {
                   Debug.Log(TAG + "4PosOpponentWinningLowestHigherCard" + higherCards[higherCards.Count -1].getCardId());
                    return higherCards[higherCards.Count -1];
                } else {
                    Debug.Log(TAG + "4PosOpponentWinningLowesCard" + cards[cards.Count -1].getCardId());
                    return cards[cards.Count -1];
                }

            }
             
            return cards[cards.Count -1];
        }

        Card highestAllCard = moveHelper.highestRemainingCard(game, cards[0].suit);
        Card myHighCard = cards[0];
        Card myLowCard = cards[cards.Count - 1];
        bool isMighHighCardBiggerThanWinningCard = callbreakCardHelper.compare(myHighCard, winningCard, myHighCard.suit);
        bool isPartnerHighCardBiggerThanRemainingCard = callbreakCardHelper.compare(winningCard, highestAllCard, myHighCard.suit);
        bool isMighHighCardBiggerThanRemainingCard = myHighCard.getCardId().Equals(highestAllCard.getCardId());
        
        Debug.Log(TAG + "myHighCard is " + myHighCard.getCardId() + " and isMighHighCardBiggerThanWinningCard " + isMighHighCardBiggerThanWinningCard);
        Debug.Log(TAG + "isMighHighCardBiggerThanRemainingCard " + isMighHighCardBiggerThanRemainingCard + " highestAllCard" + highestAllCard.getCardId());
        if(highestAllCard!= null)
        Debug.Log(TAG + "highestAllCard is " + highestAllCard.getCardId());
        if(myHighCard != null)
        Debug.Log(TAG + "myHighCard is " + myHighCard.getCardId() + " and isMighHighCardBiggerThanWinningCard " + isMighHighCardBiggerThanWinningCard);
        if(myLowCard != null)
        Debug.Log(TAG + "myLowCard is " + myLowCard.getCardId());

        Debug.Log(TAG + " isPartnerHighCardBiggerThanRemainingCard " + isPartnerHighCardBiggerThanRemainingCard);

        if(position == 3 || position == 2) {
            if(playerPositionHelper.getOppositePlayerPosition(pos).Equals(winningPos)) {

                //Incase partner is winning play the high card.
                if(higherCards.Count > 0 && isPartnerTryingNil) {
                    return higherCards[0];
                }

                if(isPartnerHighCardBiggerThanRemainingCard) {
                    Debug.Log(TAG + "23PosPartnerWinningPartnerHighestCard" + cards[cards.Count -1].getCardId());
                     return cards[cards.Count -1];
                }
                
                if(isMighHighCardBiggerThanRemainingCard && isMighHighCardBiggerThanWinningCard) {
                    Debug.Log(TAG + "23PosPartnerWinningMeHighestCard" + myHighCard.getCardId());
                    return myHighCard;
                }
                Debug.Log(TAG + "23PosPartnerWinningDefaultLowCard" + cards[cards.Count -1].getCardId());
 
                return cards[cards.Count -1];


                //CASEA: Partner is winnung and partner have played higher than All 
                //CASEB: Partner is winnung and partner have NOT played higher tha || Oponent is winning || any other case
                //CASE B1 I have the highest remainign card. Play it.
                //Case B2 I do not have the highest remaining card. Play yhe lowest card.

        
            } else {

                if(position == 3) {
                    //This is basically the 2nd player trying NIL. 
                    //TODO: Another case can be 4th player is trying NIL.(To be solved later)
                    if(isOpponentTryingNil) {
                        if(isMighHighCardBiggerThanRemainingCard && isMighHighCardBiggerThanWinningCard) {
                            //Still I will not throw High card. Check if I have a low card. To make them nil.                            
                            return myLowCard;
                        
                        }

                    }
                }

                List<Card> aCards = cardHelper.filterByNumber(cards, "A");
                if(aCards.Count > 0 && winningCard.suit.Equals(game.getCurrentRound().roundSuit)) {
                    Debug.Log(TAG + "23PosOpponentWinningPartnerHighestCard" + aCards[0].getCardId());
                    return aCards[0];

                }

                if(isMighHighCardBiggerThanRemainingCard && isMighHighCardBiggerThanWinningCard) {
                    Debug.Log(TAG + "23PosOpponentWinningMyHighestCard" + myHighCard.getCardId());
                    return myHighCard;
                }
                else {
                    Debug.Log(TAG + "23PoOpponentWinningDefaultLowCard" + cards[cards.Count -1].getCardId());
                    return cards[cards.Count -1];
                }

            }

        }


        Debug.Log(TAG + "DefaultLowCard" + myLowCard.getCardId());
        return myLowCard;

    }

    public Card trumpMove(List<Card> cards, SpadeGameState game, PlayerPosition pos) {

        bool isPartnerTryingNil = false;
        bool isOpponentTryingNil = false;



        PlayerPosition winningPos = game.getCurrentRound().findWinningMove().playerPosition;

        if(playerPositionHelper.getOppositePlayerPosition(pos).Equals(winningPos)) {
            isPartnerTryingNil  = game.isPlayerNIL(winningPos) && !game.isPlayerNILBusted(winningPos);

        } else {
            isOpponentTryingNil =  game.isPlayerNIL(winningPos) && !game.isPlayerNILBusted(winningPos);
        }

        Card winningCard = game.getCurrentRound().findWinningMove().card;

        if(playerPositionHelper.getOppositePlayerPosition(pos).Equals(winningPos)) {
            //Partner is winning. Do not trump. Except in the scenatiom where partner NIL might get busted.
            if(!isPartnerTryingNil)
                return null;
        } else {
            //Opponent is winning and trying NIL. Do not trump.
            if(isOpponentTryingNil)
                return null;

        }

        List<Card> spades = cardHelper.filterBySuit(cards, "S");

        if(spades.Count == 0)
            return null;

        callbreakCardHelper.sortByRank(spades);

        if(spades.Count > 0) {
            if(!winningCard.suit.Equals("S")) {
                //Play the small trump.
                return spades[spades.Count -1];
                //callbreakCardHelper.sortByRank(cards);
                //return cards[cards.Count -1];

            } else {
                //Currently some other Spade is winning. See if I have higher spades. Then overtrump with the lowest higher card.
                List<Card> higherSpades = callbreakCardHelper.filterHigherCard(spades, winningCard);
                if(higherSpades.Count > 0) {
                    callbreakCardHelper.sortByRank(higherSpades);
                    return higherSpades[higherSpades.Count -1];
                }

            }

            //I am not able to play Spades. Play the lowest possible card. 
            //TODO: May bring hard mode intelligance of passing single card. Later on might 

            return spades[spades.Count -1];

        }



        //Do the smallest trump that wins. 

        callbreakCardHelper.sortByRank(cards);
        return cards[cards.Count -1];

    }

    public Card nonTrumpNonSuitMove(List<Card> cards, SpadeGameState game) {
        //Just play the lowest rank card.\

        //Filter out r

        List<Card> trumps = cardHelper.filterBySuit(cards, "S");

        List<Card> nonTrumps = cardHelper.minus(cards, trumps);
        List<Card> cardsToConsider = nonTrumps;
        if(nonTrumps.Count == 0) {
            cardsToConsider = cards;
        }
        callbreakCardHelper.sortByRank(cardsToConsider);

        return cardsToConsider[cardsToConsider.Count -1];

    }


}