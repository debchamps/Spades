public class CallbreakBid {
    public int bidId ;    
    public  PlayerPosition playerPosition;
    public int bidValue;
    
    public CallbreakBid( int bidId, PlayerPosition playerPosition , int bidValue) {
        this.bidId = bidId;
        this.bidValue = bidValue;
        this.playerPosition = playerPosition;
    }
}