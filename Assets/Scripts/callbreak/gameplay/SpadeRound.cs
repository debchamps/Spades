using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpadeRound 
{
    public enum RoundState {
        STARTED,
        COMPLETED
    }
    public int roundId;
    public int roundNumber { get; set; }
    public string roundSuit;

    public int moveNumber = 0;
    

    public List<SpadeMove> moves = new List<SpadeMove>();
    public List<Card> playedCards = new List<Card>();


    public RoundState roundState;
    public PlayerPosition winner;
    public int roundPoints = 0;
    public PlayerPosition starter;
    public PlayerPosition nextMovePlayer; 

    public PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    public Dictionary<PlayerPosition, Card> roundCards = new Dictionary<PlayerPosition, Card>();

    BrayCardHelper brayCardHelper = new BrayCardHelper();

    public SpadeMove getCurrentMove() {
        return moves[moves.Count - 1];
    }

    public SpadeRound() {
        
    }

    public SpadeRound(int rnum, PlayerPosition playerPosition) {
        roundState = RoundState.STARTED;
        this.roundNumber = rnum;
        this.nextMovePlayer = playerPosition;
    }



    public void updateMove(SpadeMove move) {


        roundCards[move.playerPosition] = move.card;
        int lastIndex = moves.Count;
        moves.Add(move);

        if(moves.Count == 1)
            roundSuit = move.card.suit;

        playedCards.Add(move.card);
        moveNumber = moveNumber + 1;
        Debug.Log("Round number " + roundNumber + " Move.Count " + moves.Count + " moveNumber: " + moveNumber);
        bool roundComplete = isRoundComplete();
        this.roundPoints += brayCardHelper.point(move.card);
        if(roundComplete) {
            onRoundComplete();
        } else {
        this.nextMovePlayer = playerPositionHelper.getNextPlayerPosition(move.playerPosition);
        }
        

    }

    public SpadeMove findWinningMove() {
        Card firstCard = moves[0].card;
        SpadeMove highMove = moves[0];

        if(moves.Count == 1)
        return moves[0];    

        for(int i=1; i<moves.Count;i++) {
            bool isHigher = brayCardHelper.compare(moves[i].card, highMove.card, firstCard.suit);
            if(isHigher) {
                highMove = moves[i];
            }
        }

        return highMove;

    }

    private void onRoundComplete() {
        calculateRoundWinner();
        this.roundState = RoundState.COMPLETED;
    }

    public bool isRoundComplete() {

        if(roundCards.Keys.Count == 4) {
            Debug.Log("Round completed" + roundNumber + " Played cards are " + playedCards);
            return true;            
        }

        return false;

    }

    PlayerPosition calculateRoundWinner() {

    int northRank = brayCardHelper.rank(roundCards[PlayerPosition.NORTH], roundSuit);
    int eastRank = brayCardHelper.rank(roundCards[PlayerPosition.EAST], roundSuit);
    int southRank = brayCardHelper.rank(roundCards[PlayerPosition.SOUTH], roundSuit);
    int westRank = brayCardHelper.rank(roundCards[PlayerPosition.WEST], roundSuit);


    int maxRank = northRank;
    PlayerPosition winnerPosition = PlayerPosition.NORTH;

    if(eastRank > northRank ) {

        winnerPosition = PlayerPosition.EAST;
        maxRank = eastRank;
    }

    if(southRank > maxRank ) {

        winnerPosition = PlayerPosition.SOUTH;
        maxRank = southRank;
    }
    

    if(westRank > maxRank ) {
        winnerPosition = PlayerPosition.WEST;
        maxRank = westRank;
    }
    
    this.winner = winnerPosition;

    //this.roundPoints = 0;
    foreach(Card card in playedCards) {
        //this.roundPoints += brayCardHelper.point(card);

    }


    Debug.Log("Winner of round " + roundNumber + " is " + this.winner + " and points is " + roundPoints);

    return winnerPosition;

    }


    
}
