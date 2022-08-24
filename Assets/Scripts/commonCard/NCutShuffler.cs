using System.Collections;

using System.Collections.Generic;

public class NCutShuffler {

    private static System.Random rng = new System.Random();  

   public void inlineShuffle(List<Card> list)  
    {  

     //return new CardDeck().fullDeck();
    
    int n = list.Count;  
    while (n > 1) {  
        n--;  
        int k = rng.Next(n + 1);  
        Card value = list[k];  
        list[k] = list[n];  
        list[n] = value;  
        }  
    
        
    } 



    public List<Card> shuffle(List<Card> cards, int n) {

        System.Random rnd = new System.Random();

        /*
        if(rnd.NextDouble() < .8) {
            return new CardDeck().fullDeck();

        } */
        

        List<Card> outputCards = cards, tmpCards = cards;
        for(int i=0;i<n;i++) {
            tmpCards = cut(outputCards);
            outputCards = tmpCards;
        }

        return outputCards;

        

    }

    private List<Card> cut(List<Card> cards) {

        System.Random rnd = new System.Random();
        int  num  = rnd.Next(5,50);

        List<Card> outputCards = new List<Card>();

        for(int i=num;i<cards.Count;i++) {
            outputCards.Add(cards[i]);
        }
        for(int i=0;i<num;i++) {
            outputCards.Add(cards[i]);
        }

        return outputCards;


    }

            static void Main() {
                  //  Console.WriteLine("Main Method");
                          //Console.WriteLine("Overloaded Main Method");


        }

}