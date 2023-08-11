public class CallBreakBidding {

    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    public CallBreakBiddingData callBreakBiddingData;
    public void startBidding(PlayerPosition playerPosition) {
        //If computer Player call it to get the bid.
        callBreakBiddingData = new CallBreakBiddingData(playerPosition);        

    }

    public void updateBid(CallbreakBid callBreakBid)  {
        callBreakBiddingData.setBidAmount(callBreakBid.playerPosition, callBreakBid.bidValue);
        bool biddingFinished = isBiddingFinished();
        if(biddingFinished) {
            callBreakBiddingData.status = CallBreakBiddingData.Status.COMPLETE;
        } else {
            callBreakBiddingData.currentBiddingPosition = callBreakBid.playerPosition;
            callBreakBiddingData.nextBiddingPosition = playerPositionHelper.getNextPlayerPosition(callBreakBid.playerPosition);
            
        }
    }

    bool isBiddingFinished() {
        if(callBreakBiddingData.playerBidAmount.Keys.Count == 4)
        return true;
        return false;
    }

}