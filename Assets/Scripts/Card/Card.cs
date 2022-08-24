
public class Card 
{

    public Card(string ssuit, string nnumber) {
        this.suit = ssuit;
        this.number = nnumber;

    }
    public string suit { get; set; }
    public string number { get; set; }

    public string getCardId() {
        return suit + number;
    }
}
