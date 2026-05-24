using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FirstMove {

    MoveHelper moveHelper = new MoveHelper();

    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    CardDeck cardDeck = new CardDeck();
    BrayCardHelper callbreakCardHelper = new BrayCardHelper();
    CardHelper cardHelper = new CardHelper();

    public string TAG = "FirstMove ";
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


    public Card decideStrategy(SpadeGameState gameState, PlayerPosition pos) {
        

        if(gameState.isNil(pos)) {
            return nilStrategy(gameState, pos);
        }

        int teamNo = getTeamNo(pos);
        int otherTeamNo =  getOtherTeamNo(teamNo);

        int target = gameState.getTarget(teamNo);

        // P3 review fix: use the bid-aware effective trick count. The previous
        // raw `getTricksWon()` includes tricks won by a busted-nil partner —
        // those tricks DON'T count toward the team's contract per the scoring
        // formula in SpadeGameState.getScore(), so using the raw total would
        // make us falsely think we've booked and trigger sandbag-avoidance
        // while we actually still need tricks to make the contract.
        int achieved = gameState.getEffectiveTricksWon(teamNo);


        int opponentTarget = gameState.getTarget(otherTeamNo);

        // BUGFIX (S1 root cause): this previously read getTarget(otherTeamNo)
        // again, making the `opponentTarget < opponentAchieved` check below
        // always trivially false (target < target) and the sandbag-avoidance
        // branch unreachable. Now uses the same bid-aware trick count as
        // `achieved` above so both sides of the comparison are apples-to-apples.
        int opponentAchieved = gameState.getEffectiveTricksWon(otherTeamNo);



        if(achieved < target) {
            return playToWin(gameState, pos);
        } else {

            // Sandbag avoidance (S1): once we've booked our bid, every extra
            // trick we take is a bag. Each 10 bags costs −100 — devastating.
            // We duck (playToLose) whenever:
            //   • sandbag setting is on AND
            //   • opponents have ALSO booked (extra tricks don't hurt them) OR
            //   • we already only need to play out the hand.
            if(!gameState.isSandBagEnabled)
                return playToWin(gameState, pos);


            if(opponentAchieved >= opponentTarget) {
                return playToLose(gameState, pos);
            }

            // Opponents haven't booked yet: keep taking tricks to deny them
            // (set-the-opponent strategy).
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
        //If already won
        List<Card> cards = callbreakCardHelper.validCards(gameState, pos);
        if(gameState.tricksWinnerCount[pos] > 0) {
            //May change logic
            return playToWin(gameState, pos);
        }
        
        callbreakCardHelper.sortByRank(cards);
        return cards[cards.Count -1];
    }

    public Card playToWin(SpadeGameState callbreakGameState, PlayerPosition pos) {
        //Do I have the highest card. Return it,
        //Guess if partner have high card.
        //Play a low card.

        List<Card> cards = callbreakCardHelper.validCards(callbreakGameState, pos);

        Card cardToPlay = null;

        cardToPlay = aceMove(cards);

        if(cardToPlay!=null) {
            DebugLog.Log("Ace Strategy: "+ cardToPlay.getCardId(), shouldLog);
            Debug.Log(TAG + "Ace Strategy fired" + cardToPlay.getCardId());
            return cardToPlay;
        }

        cardToPlay = partnerCanTrumpCard(callbreakGameState, pos);

        if(cardToPlay != null) {
            DebugLog.Log("partnerCanTrumpCard: " + cardToPlay.getCardId(), shouldLog);
            return cardToPlay;
        }

        cardToPlay = playWinningCardSpade(callbreakGameState, pos);

        if(cardToPlay!=null) {
            DebugLog.Log("playWinningCardSpade: " + cardToPlay.getCardId(), shouldLog);
            Debug.Log(TAG + "playWinningCardSpade  fired" + cardToPlay.getCardId());
            return cardToPlay;
        }



        cardToPlay = playKQStrategy(callbreakGameState, cards);

        if(cardToPlay!=null) {
            DebugLog.Log("playKQStrategy: " + cardToPlay.getCardId(), shouldLog);
            Debug.Log(TAG + "playKQStrategy  fired" + cardToPlay.getCardId());
            return cardToPlay;
        }


        cardToPlay = playWinningCard(callbreakGameState, pos);

        if(cardToPlay != null) {
            DebugLog.Log("playWinningCard: " + cardToPlay.getCardId() , shouldLog);
            Debug.Log(TAG + "playWinningCard Strategy fired" + cardToPlay.getCardId() );
            return cardToPlay;
        }


        cardToPlay = playHighestSuitLowCard(callbreakGameState, pos);

        if(cardToPlay != null) {
            DebugLog.Log("playHighestSuitLowCard : " + cardToPlay.getCardId() , shouldLog);
            Debug.Log(TAG + "playHighestSuitLowCard " + cardToPlay.getCardId() );
            return cardToPlay;
        }

        
        DebugLog.Log("Default: " + cards[0].getCardId(), shouldLog);
        Debug.Log(TAG + "Default Strategy fired" + cards[0].getCardId());
        return cards[0];

    }

    /// <summary>
    /// Real implementation (S1). When LEADING and our team has already booked
    /// our bid, we want to NOT take more tricks (each extra trick is a bag,
    /// 10 bags = −100). Strategy:
    ///   1. Lead the LOWEST card of our SHORTEST non-spade suit — short suits
    ///      are likely to be voids for someone else, who will then win cheaply.
    ///   2. If we have no non-spades, lead our lowest spade.
    ///   3. Never lead a high card from a long suit (would win and book a bag).
    /// Returns the chosen card. Falls back to highest available if no cards.
    /// </summary>
    public Card playToLose(SpadeGameState gameState, PlayerPosition pos) {
        List<Card> cards = callbreakCardHelper.validCards(gameState, pos);
        if (cards == null || cards.Count == 0) return null;

        // Separate non-spades from spades. Leading spades when sandbagging is
        // bad because they win frequently.
        List<Card> spades    = cardHelper.filterBySuit(cards, "S");
        List<Card> nonSpades = cardHelper.minus(cards, spades);

        if (nonSpades.Count > 0) {
            // Group non-spades by suit, find the SHORTEST suit (most likely to
            // be void in someone's hand → they'll trump it for us → we lose).
            var bySuit = cardHelper.groupBySuit(nonSpades);
            List<Card> shortestSuit = null;
            int shortestLen = int.MaxValue;
            foreach (var kv in bySuit) {
                if (kv.Value.Count > 0 && kv.Value.Count < shortestLen) {
                    shortestLen   = kv.Value.Count;
                    shortestSuit  = kv.Value;
                }
            }
            if (shortestSuit != null && shortestSuit.Count > 0) {
                callbreakCardHelper.sortByRank(shortestSuit);
                // [Count-1] is lowest after sortByRank (descending)
                return shortestSuit[shortestSuit.Count - 1];
            }
        }

        // No non-spades — forced to lead a spade. Pick the lowest.
        if (spades.Count > 0) {
            callbreakCardHelper.sortByRank(spades);
            return spades[spades.Count - 1];
        }

        // Should be unreachable but stay safe.
        callbreakCardHelper.sortByRank(cards);
        return cards[cards.Count - 1];
    }

    public Card playMove(SpadeGameState callbreakGameState, PlayerPosition pos) {

            Card card = decideStrategy(callbreakGameState, pos);
            if(card != null) {
                return card;
            }
            return null;
    }

    public Card playHighestSuitLowCard(SpadeGameState callbreakGameState, PlayerPosition pos) {
        List<Card> cards = callbreakGameState.playerMap[pos].remainingCards;
        List<Card> spades = cardHelper.filterBySuit(cards, "S");
        List<Card> nonSuits = cardHelper.minus(cards, spades);
        PlayerPosition partnerPos = playerPositionHelper.getOppositePlayerPosition(pos);
        if(nonSuits.Count > 0) {
            callbreakCardHelper.sortByRank(nonSuits);
            if(callbreakGameState.isPlayerNIL(partnerPos) && !callbreakGameState.isPlayerNILBusted(partnerPos))
                return nonSuits[0];
            else 
                return nonSuits[nonSuits.Count -1];

        }
        return null;


    }



    public Card playWinningCardSpade(SpadeGameState callbreakGameState, PlayerPosition pos) {
        int currentRoundNo = callbreakGameState.getCurrentRound().roundNumber;
        Debug.Log(TAG + " playWinningCardSpade currentRoundNo" + currentRoundNo);
        if(currentRoundNo < 3)
            return null;
        List<Card> cards = callbreakCardHelper.validCards(callbreakGameState, pos);

        List<Card> spades = cardHelper.filterBySuit(cards, "S");
        if(spades.Count == 0)
            return null;

        Debug.Log(TAG + " playWinningCardSpade spades.Count" + spades.Count + " cards.Count " + cards.Count);
        if(spades.Count < cards.Count/2 - 1)
            return null;

        Card highestRemainingSpadeAll = moveHelper.highestRemainingCard(callbreakGameState, "S");
        Debug.Log(TAG + " playWinningCardSpade highestRemainingSpadeAll " + highestRemainingSpadeAll.getCardId());

        if(highestRemainingSpadeAll != null) {
            bool doIHaveHighestCard = cardHelper.contains(spades, highestRemainingSpadeAll);
            Debug.Log(TAG + " playWinningCardSpade doIHaveHighestCard " + doIHaveHighestCard);
            if(doIHaveHighestCard) {
                return highestRemainingSpadeAll;
            }
        }


        //Check if my spade is the highest spade.

        return null;
    }

    public Card partnerCanTrumpCard(SpadeGameState callbreakGameState, PlayerPosition pos) {
        Dictionary<string, List<PlayerPosition>> trumpDoneMap = callbreakCardHelper.trumpDoneMap(callbreakGameState);
        PlayerPosition partnerPos = playerPositionHelper.getOppositePlayerPosition(pos);
        //TODO: This strategy looks extremely hacky.
        if(callbreakGameState.isPlayerNIL(partnerPos) && !callbreakGameState.isPlayerNILBusted(partnerPos))
            return null;
        List<Card> cards = callbreakCardHelper.validCards(callbreakGameState, pos);

        foreach(string suit in CardHelper.SUITS) {
            if(!suit.Equals("S")) {
                if(trumpDoneMap[suit].Contains(playerPositionHelper.getOppositePlayerPosition(pos))) {
                        List<Card> suitedCards = cardHelper.filterBySuit(cards, suit);
                        if(suitedCards.Count > 0) {
                            callbreakCardHelper.sortByRank(suitedCards);
                            //Playing the highest of lowr card

                            Debug.Log(TAG + "partnerCanTrumpCard is " + suitedCards[0].getCardId());

                            return suitedCards[0];

                        }
                }
                
            }
        }
        return null;
    }

    public Card playWinningCard(SpadeGameState callbreakGameState, PlayerPosition pos) {


        List<Card> cards = callbreakGameState.playerMap[pos].remainingCards;

        Dictionary<string, List<Card>> groupedCards = cardHelper.groupBySuit(cards);


        //Card highestCard = highestRemainingCard(callbreakGameState);

        List<string> suits = new List<string>(CardHelper.SUITS);
        
        inlineShuffle(suits);

        Dictionary<string, List<PlayerPosition>> trumpDoneMap = callbreakCardHelper.trumpDoneMap(callbreakGameState);

        

        foreach(string suit in suits) {
            if(!suit.Equals("S")) {

                Debug.Log(TAG + "Suit is " + suit);
                if(groupedCards.ContainsKey(suit) && groupedCards[suit].Count > 0 ) {
                    List<Card> suitedCards = groupedCards[suit];
                    callbreakCardHelper.sortByRank(suitedCards);
                    Card highestSuitOfPlayer = suitedCards[0];
                    Debug.Log(TAG + "highestSuitOfPlayer is " + highestSuitOfPlayer.getCardId());
                    Card highestRemainingCardAll = moveHelper.highestRemainingCard(callbreakGameState, suit);
                    Debug.Log(TAG + "highestRemainingCard is " + highestRemainingCardAll.getCardId());

                    if(highestRemainingCardAll.getCardId().Equals(highestSuitOfPlayer.getCardId())) {

                        if(!trumpDoneMap.ContainsKey(highestSuitOfPlayer.suit)) {
                        Debug.Log(TAG + "ABC highestSuitOfPlayer is " + highestSuitOfPlayer.getCardId() + " is not trumped");
                            //This suit is not trumped.

                            if(!highestSuitOfPlayer.number.Equals("A"))
                                return highestSuitOfPlayer;
                        } else {
                            Debug.Log(TAG + "highestSuitOfPlayer is " + highestSuitOfPlayer.getCardId() + " is ctrumped");

                        }

                    }
                    //Check for trump.

                }
            }
        }

        return null;


        //Check if it is the highest suited remaining card.
        // Check if anyone is anyone have trump the suit before.
        // Check if anyone is going to trump.


    }





    Card aceMove(List<Card> remainingCards) {
        List<Card> aces = cardHelper.filterByNumber(remainingCards, "A");
        if(aces.Count>0)
        {

            foreach(Card card in aces) {
                {
                    //Q is present.
                    if(cardHelper.isCardPresent(remainingCards, card.suit, "Q")) {
                    //K is not present.
                        if(cardHelper.isCardPresent(remainingCards, card.suit, "K")) {
                            return card;
                        } else {
                            
                        }

                    } else {
                        return card;
                    }
                }
            }

            //Do not play 'A' If you have 'Q' as well as you can win it by 'Q' sometimelater.
            //Play the first Ace.
        }

        return null;

    }


    //Strategy is applicable if A> I have 'K'/'Q' and no 'A'  B> No hand of the suit is played.C> Play the next high card of that suit.
 
    public Card playKQStrategy(SpadeGameState game, List<Card> remainingCards) {
        List<Card> ks = cardHelper.filterByNumber(remainingCards, "K");
        List<Card> qs = cardHelper.filterByNumber(remainingCards, "Q");

        List<Card> kqs = new List<Card>();

        Dictionary<string, int> suitPlayed =  moveHelper.suitPlayedMap(game);

        kqs.AddRange(ks);
        kqs.AddRange(qs);
        if(kqs.Count > 0) {
            foreach(Card card in kqs) {
                if(suitPlayed[card.suit] == 0 && !cardHelper.contains(remainingCards, card.suit, "A")) {
                    List<Card> suitedCards = cardHelper.filterBySuit(remainingCards, card.suit);
                    if(suitedCards.Count >=2) {
                        List<Card> lowerCards = callbreakCardHelper.filterLowerCard(suitedCards, card);
                        if(lowerCards.Count>=1) {
                            callbreakCardHelper.sortByRank(lowerCards);
                            //Playing the highest of lowr card
                            return lowerCards[0];
                        }
                    }
                }
            }

        }

        //Play high card below q.



        return null;


        //


    }



     public void firstCardLogic(SpadeGameState callbreakGameState, PlayerPosition playerPosition) {

         /*

        //If i have 'A' play it.
        List<Card> remainingCards = new List<Card>();


        //PLay the k's strategy.
        List<Card> ks = cardHelper.filterByNumber(remainingCards, "K");
        foreach(Card kcard in ks)
        {

        }


        //PLay the q's strategy. Very similar to k's




        //Check if trump each other is true.
        //i.e If i help someone trump and they can help me trump go for it.

        Dictionary<PlayerPosition, List<string>> trumpCardMap = trumpMap(callbreakGameState);
        List<Card> spades = cardHelper.filterBySuit(remainingCards, "S");

        bool canITrump = trumpCardMap[playerPosition].Count != 0 && spades.Count !=0;

        if(canITrump)
        {
            //then only the next logic works, otherwise return.
        }

        PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();
        List<PlayerPosition> opponents = playerPositionHelper.getOtherPlayerPositions(playerPosition);

        foreach(PlayerPosition pos in opponents)
        {
            List<Card> opponentSpades = cardHelper.filterBySuit(callbreakGameState.playerMap[pos].remainingCards, "S");
            bool canOpponentTrump = trumpCardMap[pos].Count != 0 && spades.Count != 0;

            string helpOpponentTrumpSuit = trumpCardMap[pos][0];
            //Now check if I have the card, play the lowest possible card of that.
            cardHelper.filterBySuit(remainingCards, helpOpponentTrumpSuit);


        }
        */


    }


    public void inlineShuffle(List<string> list)  
    {  

        System.Random rng = new System.Random();
    int n = list.Count;  
        while (n > 1) {  
        n--;  
        int k = rng.Next(n + 1);  
        string value = list[k];  
        list[k] = list[n];  
        list[n] = value;  
        }  
    } 




    public Dictionary<string, int>  cardPlayedSuit(SpadeGameState callbreakGameState) {

        Dictionary<string, int> suitCountPlayed = new Dictionary<string, int>(); 
        suitCountPlayed["D"] = 0;
        suitCountPlayed["C"] = 0;
        suitCountPlayed["S"] = 0;
        suitCountPlayed["H"] = 0;

        foreach(SpadeRound round in callbreakGameState.rounds) {
            foreach(Card card in round.playedCards) {
                int existingCount = suitCountPlayed[card.suit];
                suitCountPlayed.Add(card.suit, existingCount + 1);
            }
        }

        return suitCountPlayed;

    }


    public Dictionary<string, int> cardRemainingSuit(SpadeGameState callbreakGameState) {

        Dictionary<string, int> suitCountRemaining = new Dictionary<string, int>(); 

        Dictionary<string, int>  played = cardPlayedSuit(callbreakGameState);


        suitCountRemaining["D"] = 13 - played["D"];
        suitCountRemaining["C"] = 13 - played["C"];
        suitCountRemaining["S"] = 13 - played["S"];
        suitCountRemaining["H"] = 13 - played["H"];

        return suitCountRemaining;

    }
}