using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;


public class BrayCardHelper {

    Dictionary<string, int> numberToRankMap = new Dictionary<string, int>();

    CardHelper cardHelper = new CardHelper();


    public BrayCardHelper() {
        numberToRankMap.Add("A", 13);
        numberToRankMap.Add("K", 12);
        numberToRankMap.Add("Q", 11);
        numberToRankMap.Add("J", 10);
        numberToRankMap.Add("10", 9);
        numberToRankMap.Add("9", 8);
        numberToRankMap.Add("8", 7);
        numberToRankMap.Add("7", 6);
        numberToRankMap.Add("6", 5);
        numberToRankMap.Add("5", 4);
        numberToRankMap.Add("4", 3);
        numberToRankMap.Add("3", 2);
        numberToRankMap.Add("2", 1);
    }

    public int point(Card card) {

        if(card.suit.Equals("H"))
            return 1;
        if(GamePlay.GAME_VARIANT.Equals(GameVariant.HEARTS) && card.suit.Equals("S") && card.number.Equals("Q"))
            return 13;
        if(GamePlay.GAME_VARIANT.Equals(GameVariant.BRAY) && card.suit.Equals("S") && card.number.Equals("Q"))
            return 12;

        return 0;

    }

    public int rank(Card card, string roundSuit) {
        if (card.suit.Equals("S"))
        {
            return 100 + numberToRankMap[card.number];
        } else if (card.suit.Equals(roundSuit)) {
            return 50 + numberToRankMap[card.number];

        } else {
            return numberToRankMap[card.number];
        }
    }

    /**
    Return 1 if card1 > card2
    0 otherwise
    */
    public bool compare(Card card1, Card card2, string suit) {
        if (rank(card1, suit) > rank(card2, suit))
            return true;
        return false;

    }



    public List<Card> getAllRemainingCards(SpadeGameState brayGameState) {
        List<Card> allRemainingCards = new List<Card>();
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
            allRemainingCards.AddRange(brayGameState.playerMap[pos].remainingCards);
        return allRemainingCards;
    }

    public List<Card> getAllRemainingCardsBySuit(SpadeGameState brayGameState, string suit)
    {
        List<Card> allRemainingCards = getAllRemainingCards(brayGameState);
        List<Card> suitedCards = cardHelper.filterBySuit(allRemainingCards, suit);
        sortByRank(suitedCards);
        return suitedCards;
    }
    public void secondCardLogic(SpadeGameState brayGameState,PlayerPosition playerPosition) {
        
        List<Card> remainingCards = brayGameState.playerMap[playerPosition].remainingCards;

        //Do I have the highest remaining Card.
            //Is this color trumped before. Then Play the lowest card.
            //If it is not trumped before
            //  Check with probability if it should be trumped. If show play lowest card.
            //  Check if I have winning card. Play winning card.
            //Play the lowest card.
            


        
        if(cardHelper.contains(remainingCards, new Card(brayGameState.getCurrentRound().roundSuit, "A"))) {
            //Play suited 'A'
        }


        
        List<Card> suitedCardRemaining = getAllRemainingCardsBySuit(brayGameState, brayGameState.getCurrentRound().roundSuit);




        //Check if i have a winning card.

    }


    public float intelligenceLevel(SpadeMatchState callbreakMatchState) {
        //If set score is low. give more intelligence
        // If set score is high. Give less intelligence.
        return 0f;
    }

    bool playIntelligentMoveWithProbability(SpadeGameState brayGameState, PlayerPosition playerPosition) {

        float intelligence = 0.8f;

        System.Random rnd = new System.Random();
        double rndVal = rnd.NextDouble();
        if(rndVal < intelligence) {
            return true;
        }
        return false;
    }

    


 

    public Dictionary<string, int> suitPlayedMap(SpadeGameState brayGameState) {
        Dictionary<string, int> suitPlayedCountMap = new Dictionary<string, int>();
        foreach(string suit in CardHelper.SUITS)
        {
            suitPlayedCountMap[suit] = 0;
        }

        foreach(SpadeRound round in brayGameState.rounds)
        {
            suitPlayedCountMap[round.roundSuit] += 1; 
        }

        return suitPlayedCountMap;
    }

    bool isTrumpCard(Card card) {
        if(card.suit.Equals("H") || (card.suit.Equals("S") && card.number.Equals("Q")))
            return true;
        return false;
    }

    public Dictionary<string, List<PlayerPosition>> trumpDoneMap(SpadeGameState brayGameState) {

        Dictionary<string, List<PlayerPosition>> trumpDoneMapPlayer = new Dictionary<string, List<PlayerPosition>>();
        trumpDoneMapPlayer["H"] = new List<PlayerPosition>();
        trumpDoneMapPlayer["C"] = new List<PlayerPosition>();
        trumpDoneMapPlayer["S"] = new List<PlayerPosition>();
        trumpDoneMapPlayer["D"] = new List<PlayerPosition>();

        foreach( string suit in CardHelper.SUITS) {
            foreach(SpadeRound round in brayGameState.rounds) {
                if(round.roundState.Equals(SpadeRound.RoundState.COMPLETED) ) {
                    SpadeMove firstMove = round.moves[0];
                    for(int i=1;i<4;i++) {
                        if(!round.moves[i].card.suit.Equals(round.roundSuit) && isTrumpCard(round.moves[i].card)) {
                            if(trumpDoneMapPlayer.ContainsKey(round.roundSuit)) {

                                trumpDoneMapPlayer[round.roundSuit].Add(round.moves[i].playerPosition);

                            } else {
                                List<PlayerPosition> players = new List<PlayerPosition>();
                                players.Add(round.moves[i].playerPosition);
                                trumpDoneMapPlayer[round.roundSuit] = players;
                            }
                        }
                    }
                }
            }

        }

        return trumpDoneMapPlayer;
    }    



    public Dictionary<PlayerPosition, List<string>> trumpMap(SpadeGameState brayGameState)
    {
        Dictionary<PlayerPosition, List<string>> trumpCardMap = new Dictionary<PlayerPosition, List<string>>();
        trumpCardMap.Add(PlayerPosition.EAST, trumpMap(brayGameState, PlayerPosition.EAST));
        trumpCardMap.Add(PlayerPosition.WEST, trumpMap(brayGameState, PlayerPosition.WEST));
        trumpCardMap.Add(PlayerPosition.SOUTH, trumpMap(brayGameState, PlayerPosition.SOUTH));
        trumpCardMap.Add(PlayerPosition.NORTH, trumpMap(brayGameState, PlayerPosition.NORTH));

        return trumpCardMap;
    }

    public List<string> trumpMap(SpadeGameState brayGameState, PlayerPosition pos)
    {
        List<string> trumpSuits = new List<string>();
        foreach(string suit in CardHelper.SUITS) {
            if(suit.Equals("S"))
            {
                List<Card> suitedCards = cardHelper.filterBySuit(brayGameState.playerMap[pos].remainingCards, suit);
                if (suitedCards.Count == 0)
                    trumpSuits.Add(suit);

            }
        }
        return trumpSuits;

    }


    public Card findWinningCard(List<Card> cards) {
        Card firstCard = cards[0];
        Card highCard = firstCard;

        if(cards.Count == 1)
        return highCard;    

        for(int i=1; i<cards.Count - 1;i++) {
            bool isHigher = compare(cards[i], highCard, firstCard.suit);
            if(isHigher) {
                highCard = cards[i];
            }
        }

        return highCard;

    }

    public List<Card> filterHigherCard(List<Card> cards, Card  baseCard) {
        List<Card> higherCards = new List<Card>();
        foreach(Card card in cards) {
            if(card.suit.Equals(baseCard.suit) && numberToRankMap[card.number] > numberToRankMap[baseCard.number]) {
                higherCards.Add(card);
            }
        }
        return higherCards;
    }

    //Inclusive of the start and endCard.
    public List<Card> between(List<Card> cards, Card  startCard, Card endCard) {
        List<Card> inBetweenCards = new List<Card>();
        foreach(Card card in cards) {
            if(card.suit.Equals(startCard.suit) && numberToRankMap[card.number] >= numberToRankMap[startCard.number] && numberToRankMap[card.number] <= numberToRankMap[endCard.number]) {
                inBetweenCards.Add(card);
            }
        }
        return inBetweenCards;
    }




    public List<Card> filterLowerCard(List<Card> cards, Card baseCard)
    {
        List<Card> outputCards = new List<Card>();
        foreach (Card card in cards)
        {
            if (card.suit.Equals(baseCard.suit) && numberToRankMap[card.number] < numberToRankMap[baseCard.number])
            {
                outputCards.Add(card);
            }
        }
        return outputCards;
    }

    public string suitToString(string suit) {
        if(suit.Equals("S"))
            return "spade";
        if(suit.Equals("D"))
            return "diamond";
        if(suit.Equals("H"))
            return "heart";
        return "club";

    }


    string suitToSpriteNumMap(string suit) {
        if(suit.Equals("C"))
            return "<sprite=1>";
        if(suit.Equals("D"))
            return "<sprite=2>";
        if(suit.Equals("H"))
            return "<sprite=0>";
        if(suit.Equals("S"))
            return "<sprite=3>";

            return "<sprite=0>";
    }
    public WarningEntity getInvalidReason(SpadeGameState brayGameState, PlayerPosition playerPosition, Card card) {

        List<Card> playerRemainingCards = brayGameState.playerMap[playerPosition].remainingCards;
        SpadeRound currRound = brayGameState.getCurrentRound();


        if(currRound.moveNumber == 0 && brayGameState.gameVariant.Equals(GameVariant.BRAY)) {
            return null;
        }



        if(brayGameState.gameVariant.Equals(GameVariant.HEARTS)) {
            if(currRound.moveNumber == 0) {
                if(currRound.roundNumber == 1) {
                    return new WarningEntity("Start with '2' of <sprite=1>", "Cards/"  + "icon");

                } else if(card.suit.Equals("H")) {
                    return new WarningEntity("<sprite=0>  NOT broken yet", "Cards/"  + "icon");

                } else {
                    return null;
                }

            }

        }

        string roundSuit = currRound.moves[0].card.suit;

        List<Card> roundSuitCards = cardHelper.filterBySuit(playerRemainingCards, roundSuit);

        if(roundSuitCards.Count >=1 && !card.suit.Equals(roundSuit)) {

            return new WarningEntity("Player have to play  " + suitToSpriteNumMap(roundSuit), "Cards/" + suitToString(roundSuit) + "icon");
            //return new WarningEntity("   Player have to play", "Cards/" + suitToString(roundSuit) + "icon");
            //return new WarningEntity("Hearts Broken by " + playerPosition, "Cards/" + suitToString("H") + "icon");
            //YOu have to play suited
        }        
        if(roundSuitCards.Count >=1 && card.suit.Equals(roundSuit)) {
            //YOu have to play higher suit than card.
            return new WarningEntity("Player have to play higher", "Cards/" + suitToString(roundSuit) + "icon");
        }   

        if(roundSuitCards.Count == 0 && !card.suit.Equals("S")) {
            return new WarningEntity("Player have to trump with", "Cards/" + suitToString("S") + "icon");
            //Have to play a  spade
        }
     

        if(roundSuitCards.Count == 0 && card.suit.Equals("S")) {
             return new WarningEntity("Player have to play HIGHER ", "Cards/" + suitToString("S") + "icon");
           //Have to play a bigger spade
        }

        return null;

    }


    public List<Card> validCards(SpadeGameState brayGameState, PlayerPosition playerPosition) {

        List<Card> playerRemainingCards = brayGameState.playerMap[playerPosition].remainingCards;




        SpadeRound currRound = brayGameState.getCurrentRound();

        if(currRound.moveNumber == 0) {

            bool isSpadeBroken = brayGameState.isSpadeBroken();
            if(!isSpadeBroken) {
                    List<Card> spades = cardHelper.filterBySuit(playerRemainingCards, "S");
                    if(spades.Count == playerRemainingCards.Count) {
                        return spades;

                    } else {
                    List<Card> nonSpades = cardHelper.removeCard(playerRemainingCards, spades);
                    return nonSpades;

                    }
            } 

            return playerRemainingCards;
        }

        string roundSuit = currRound.moves[0].card.suit;


        //callbreakCardHelper.

        SpadeMove winningMove = currRound.findWinningMove();

        //Debug.Log("playerRemainingCards " + playerRemainingCards + " roundSuit " + roundSuit);

        List<Card> roundSuitCards = cardHelper.filterBySuit(playerRemainingCards, roundSuit);



        if(roundSuitCards.Count > 0) {
            return roundSuitCards;
        } 
        return playerRemainingCards;

    }

    public  List<Card> rearrange(List<Card> cards) {
        List<Card> allCards = new List<Card>();
        Dictionary<string, List<Card>> suitToCardMap =  cardHelper.groupBySuit(cards);
        List<Card> spades = suitToCardMap["S"];
       List<Card> clubs = suitToCardMap["C"];
       List<Card> diamonds = suitToCardMap["D"];
       List<Card> hearts = suitToCardMap["H"];

        sortByRank(spades);
        sortByRank(hearts);
        sortByRank(clubs);
        sortByRank(diamonds);

        allCards.AddRange(spades);
        allCards.AddRange(hearts);
        allCards.AddRange(clubs);
        allCards.AddRange(diamonds);
        return allCards;

    }

    public void sortByRank(List<Card> cards) {

        cards.Sort(delegate(Card x, Card y)
        {
            if(numberToRankMap[x.number] < numberToRankMap[y.number])
                return 1;
            else 
                return -1;    

        });
            
    }




}

