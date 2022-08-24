using System.Collections.Generic;
using UnityEngine;

public class CallBreakBiddingData {

    public enum Status {
        IN_PROGRESS,
        COMPLETE
    }

    public Status status;
    public CallBreakBiddingData(PlayerPosition bidstarter) {
        this.biddingStarter = bidstarter;
        nextBiddingPosition = bidstarter;
        status = Status.IN_PROGRESS;
    }

    public PlayerPosition biddingStarter;
    public PlayerPosition currentBiddingPosition;
    public PlayerPosition nextBiddingPosition;

    public Dictionary<PlayerPosition, int> playerBidAmount = new Dictionary<PlayerPosition, int>();

    public void setBidAmount(PlayerPosition playerPosition, int bid) {
        Debug.Log("Setting bid " + playerPosition + " Value " + bid);
        playerBidAmount[playerPosition] = bid;
        currentBiddingPosition = playerPosition;
    }

    public int getBidAmount(PlayerPosition playerPosition) {
        return playerBidAmount[playerPosition];
    }
}