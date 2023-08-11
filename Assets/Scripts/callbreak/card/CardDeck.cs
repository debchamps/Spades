using System.Collections;
using System.Collections.Generic;
public class CardDeck {

    public string[] suits = {"C", "S", "D", "H"};
    public string[] numbers = {"A", "K", "Q", "J", "2", "3", "4", "5", "6", "7", "8", "9", "10"};

    public List<Card> fullDeck() {
        List<Card> cardList = new List<Card>();
        foreach(string suit in suits) {
            foreach (string number in numbers ) {
                Card card = new Card(suit, number);
                cardList.Add(card);
            }
        }
        return cardList;
    }

    public List<Card> getSuitAll(string suit) {
        List<Card> cardList = new List<Card>();

        foreach (string number in numbers ) {
            Card card = new Card(suit, number);
            cardList.Add(card);


        }
        return cardList;
    }

    

}

