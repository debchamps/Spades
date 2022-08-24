public class SpadeMove {
    public Card card; 
    public PlayerPosition playerPosition;
    public bool autoPlay = false;

    public SpadeMove(PlayerPosition position, Card cardToPlay) {
        this.playerPosition = position;
        this.card = cardToPlay;
    }
}