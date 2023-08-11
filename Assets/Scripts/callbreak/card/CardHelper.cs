using System.Collections.Generic;

public class CardHelper {

    public static string[] SUITS = { "C", "S", "D", "H" };
    public List<Card> filterBySuit(List<Card> cards, string suit) {
        List<Card> outputCards = new List<Card>();
        foreach(Card card in cards) {
            if(card.suit.Equals(suit)) {
                outputCards.Add(card);
            }
        }
        return outputCards;
    }

    public  List<Card> filterByNumber(List<Card> cards, string number) {
        List<Card> outputCards = new List<Card>();
        foreach(Card card in cards) {
            if(card.number.Equals(number)) {
                outputCards.Add(card);
            }
        }
        return outputCards;
    }

   public Dictionary<string, List<Card>> groupBySuit(List<Card> cards) {
        Dictionary<string, List<Card>>suitedCards = new Dictionary<string, List<Card>>();
        suitedCards.Add("C" , new List<Card>());
        suitedCards.Add("H" , new List<Card>());
        suitedCards.Add("S" , new List<Card>());
        suitedCards.Add("D" , new List<Card>());
        foreach(Card card in cards) {
            List<Card> existingCards = suitedCards[card.suit];
            existingCards.Add(card);
            //countSuitMap.Add(card.suit, existingCards);
        }
        return suitedCards;
    }


    public Dictionary<string, int> countAllSuit(List<Card> cards) {
        Dictionary<string, int> countSuitMap = new Dictionary<string, int>();
        countSuitMap.Add("C" , 0);
        countSuitMap.Add("S" , 0);
        countSuitMap.Add("D" , 0);
        countSuitMap.Add("H" , 0);


        foreach(Card card in cards) {
            int existingCount = countSuitMap[card.suit];
            countSuitMap.Add(card.suit, existingCount + 1);
        }
        return countSuitMap;
    }


    public int countSuit(List<Card> cards, string suit) {
        int counter = 0;
        foreach(Card card in cards) {
            if(card.suit.Equals(suit)) {
                counter++;
            }
        }
        return counter;
    }


   public  bool isCardPresent(List<Card> cards, string suit, string number) {
        foreach(Card card in cards) {
            if(card.suit.Equals(suit) && card.number.Equals(number)) {
                return true;
            }
        }

        return false;
    }

    public bool isCardPresent(List<Card> cards, Card card) {

        return isCardPresent(cards, card.suit, card.number);
    }


    public List<Card> removeCard(List<Card> cards, Card cardToRemove) {
        List<Card> cardOutput = new List<Card>();
        foreach(Card card in cards) {
            if(card.suit.Equals(cardToRemove.suit) && card.number.Equals(cardToRemove.number)) {
                //to remove
            } else {
                cardOutput.Add(card);
            }
        }
        return cardOutput;
    }

    public List<Card> removeCard(List<Card> cards, List<Card> cardsToRemove) {
        List<Card> cardOutput = new List<Card>();
        Dictionary<string, bool> presentCardMap =  new Dictionary<string, bool>();
        foreach(Card card in cardsToRemove) {
            presentCardMap.Add(card.getCardId(), true);
        }

        foreach(Card card in cards) {
            if(presentCardMap.ContainsKey(card.getCardId())) {
                //to remove
            } else {
                cardOutput.Add(card);
            }
        }
        return cardOutput;
    }

    public bool contains(List<Card> cards, Card cardToFind) {
        foreach(Card card in cards) {
            if(card.getCardId().Equals(cardToFind.getCardId()))
                return true;
        }
        return false;
    }

    public bool contains(List<Card> cards, string suit, string number) {
        foreach(Card card in cards) {
            if(card.suit.Equals(suit) && card.number.Equals(number))
                return true;
        }
        return false;
    }

    public List<Card> minus(List<Card> a, List<Card> b) {
        List<Card> results = new List<Card>();
        foreach(Card card in a) {
            if(!contains(b,card)) {
                results.Add(card);
            }
        }
        return results;
    }


}