using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class SpadeGameState {

    System.Random RND = new System.Random();
    [JsonIgnore]
    public Dictionary<PlayerPosition, Player> playerMap = new Dictionary<PlayerPosition, Player>();
    public Dictionary<PlayerPosition, List<Card>> playerCardMap = new Dictionary<PlayerPosition, List<Card>>();

    public int gameNumber;

    public GameVariant gameVariant;

    public CardHelper cardHelper = new CardHelper();


    public int gameTarget = 500;
    public int mercyTarget = -200;

    public bool isMercyEnabled = true;
    public bool isSandBagEnabled = false;

    public bool isSandbagThisGame = false;


    public BrayCardHelper callbreakCardHelper =  new BrayCardHelper();
    NCutShuffler nCutShuffler = new NCutShuffler();

    public PlayerPosition matchStarter;

    Queue<SpadeMatchState.NotificationStatus> notifications = new Queue<SpadeMatchState.NotificationStatus>();

    public List<SpadeRound> rounds = new List<SpadeRound>();

    public Dictionary<PlayerPosition, int> tricksWinnerCount = new Dictionary<PlayerPosition, int>();


    public int team1Score = 0;
    public int team2Score = 0;

    public Dictionary<PlayerPosition, int> gameScore = new Dictionary<PlayerPosition, int>();


    //Team1 Score Team2 Score b 

    public GameStatus gameStatus;




    public enum GameStatus {
        BIDDING,
        PASS_CARD,
        IN_PROGRESS,
        COMPLETED
    }

    public bool isNoPassingRound() {
        if(gameVariant.Equals(GameVariant.HEARTS) && gameNumber % 4 != 0) {
            return false;
        }            
        else 
        return true;
    }

    public PlayerPosition playerToPass(PlayerPosition pos) {
        PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();
        if(gameVariant.Equals(GameVariant.HEARTS)) {
            if(gameNumber % 4 == 1) {
              return  playerPositionHelper.getNextPlayerPosition(pos);
            }
            else if(gameNumber % 4 == 2) {
               return playerPositionHelper.getPreviousPlayerPosition(pos);
                
            }
            else if(gameNumber % 4 == 3) {
                return playerPositionHelper.getOppositePlayerPosition(pos);                
            } else {
                return pos;
            }

        } else {
            return  playerPositionHelper.getNextPlayerPosition(pos);
   
        }

    }

     public PlayerPosition playerFromPass(PlayerPosition pos) {
        PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();
        if(gameVariant.Equals(GameVariant.HEARTS)) {
            if(gameNumber % 4 == 1) {
              return  playerPositionHelper.getPreviousPlayerPosition(pos);
            }
            else if(gameNumber % 4 == 2) {
               return playerPositionHelper.getNextPlayerPosition(pos);
                
            }
            else if(gameNumber % 4 == 3) {
                return playerPositionHelper.getOppositePlayerPosition(pos);                
            } else {
                return pos;
            }

        } else {
            return  playerPositionHelper.getPreviousPlayerPosition(pos);
   
        }

    }

    public void sortByRank(List<List<Card>> cards) {

        cards.Sort(delegate(List<Card> x, List<Card> y)
        {
            ComputerBidder computerBidder = new ComputerBidder();
            if(computerBidder.totalBid(x) < computerBidder.totalBid(y))
                return 1;
            else 
                return -1;    

        });
            
    }


    public void randomRest(List<Card> cards1, List<Card> cards2, List<Card> cards3) {
        double rndValue = RND.NextDouble();
        if(rndValue < .33) {
            playerCardMap[PlayerPosition.WEST] = cards1;
            playerCardMap[PlayerPosition.NORTH] = cards2;
            playerCardMap[PlayerPosition.EAST] = cards3;

        }
        else if(rndValue < .66) {
            playerCardMap[PlayerPosition.EAST] = cards1;
            playerCardMap[PlayerPosition.WEST] = cards2;
            playerCardMap[PlayerPosition.NORTH] = cards3;

        }
        else {
            playerCardMap[PlayerPosition.NORTH] = cards1;
            playerCardMap[PlayerPosition.EAST] = cards2;
            playerCardMap[PlayerPosition.WEST] = cards3;

        }

    }


    public void rearrange(List<Card> shuffledCards, Dictionary<PlayerPosition, int> aggregateScoreCard ) {

        List<Card> card1 = shuffledCards.GetRange(0,13);
        List<Card> card2 = shuffledCards.GetRange(13,13);
        List<Card> card3 = shuffledCards.GetRange(26,13);
        List<Card> card4 = shuffledCards.GetRange(39,13);

        List<List<Card>> cardAll = new List<List<Card>>();
        cardAll.Add(card1);
        cardAll.Add(card2);
        cardAll.Add(card3);
        cardAll.Add(card4);

        sortByRank(cardAll);

        float maxScore = 100f;
        PlayerPosition highest = PlayerPosition.SOUTH;

        if(maxScore > aggregateScoreCard[PlayerPosition.SOUTH]) {
            maxScore = aggregateScoreCard[PlayerPosition.SOUTH];
            highest = PlayerPosition.SOUTH;

        }
        if(maxScore > aggregateScoreCard[PlayerPosition.EAST]) {
            maxScore = aggregateScoreCard[PlayerPosition.EAST];
            highest = PlayerPosition.EAST;

        }
        if(maxScore > aggregateScoreCard[PlayerPosition.WEST]) {
            maxScore = aggregateScoreCard[PlayerPosition.WEST];
            highest = PlayerPosition.WEST;

        }
        if(maxScore > aggregateScoreCard[PlayerPosition.NORTH]) {
            maxScore = aggregateScoreCard[PlayerPosition.NORTH];
            highest = PlayerPosition.NORTH;        
        }

        //defauly allocation. Might get overridern to make the game interesting.
        playerCardMap[PlayerPosition.SOUTH] = callbreakCardHelper.rearrange(shuffledCards.GetRange(0,13));
        playerCardMap[PlayerPosition.WEST] = shuffledCards.GetRange(13,13);
        playerCardMap[PlayerPosition.NORTH] = shuffledCards.GetRange(26,13);
        playerCardMap[PlayerPosition.EAST]= shuffledCards.GetRange(39,13);


        /*
        if(highest.Equals(PlayerPosition.SOUTH) && aggregateScoreCard[PlayerPosition.SOUTH] > 6) {
            //50 % of the time, give a bad hand. 
            if(RND.NextDouble() < .75) {
                Debug.Log("Giving BAD hand to player SOUTH");
                playerCardMap[PlayerPosition.SOUTH] = callbreakCardHelper.rearrange(cardAll[3]);
                randomRest(cardAll[0], cardAll[1], cardAll[2]);
            }

        } else if(maxScore - aggregateScoreCard[PlayerPosition.SOUTH] > 3){
            if(RND.NextDouble() < .5) {
                Debug.Log("Giving GOOD hand to player SOUTH");
                playerCardMap[PlayerPosition.SOUTH] = callbreakCardHelper.rearrange(cardAll[0]);
                randomRest(cardAll[1], cardAll[2], cardAll[3]);
            }
        } else {
            Debug.Log("Giving DEFAULT hand to player SOUTH");
        }
        */

        playerMap[PlayerPosition.SOUTH].setCards(playerCardMap[PlayerPosition.SOUTH]);
        playerMap[PlayerPosition.WEST].setCards(playerCardMap[PlayerPosition.WEST]);
        playerMap[PlayerPosition.NORTH].setCards(playerCardMap[PlayerPosition.NORTH]);
        playerMap[PlayerPosition.EAST].setCards(playerCardMap[PlayerPosition.EAST]);

    }




    public List<PlayerPosition> findShootMoonPlayer() {
        List<PlayerPosition> shootMoonPlayers = new List<PlayerPosition>();
        List<PlayerPosition> pointWinners = new List<PlayerPosition>();
        foreach(SpadeRound round in rounds) {
            if(round.roundPoints > 0 ) {
                if(pointWinners.Count == 0) {
                    pointWinners.Add(round.winner);
                } else {
                    if(!pointWinners[0].Equals(round.winner)) {
                        //return pointWinners;
                        return shootMoonPlayers;
                    }

                }
            }

        }
        return pointWinners;
    }

    public List<PlayerPosition> getNilAchievedPlayers() {
        List<PlayerPosition> players = new List<PlayerPosition>();
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            if(isPlayerNIL(pos) && !isPlayerNILBusted(pos)) {
                players.Add(pos);
            }

        }
        return players;
    }


    public void startGame(int gameNo, List<Card> previousRoundCard, PlayerPosition start, Dictionary<PlayerPosition, int> aggregateScoreCard ) {

        Debug.Log("XXX Starting gameNumber " + gameNo);
        this.gameNumber =  gameNo;
        Debug.Log("XXX Set gameNumber " + gameNumber);


        List<Card> shuffledCards = nCutShuffler.shuffle(previousRoundCard, 20);
        nCutShuffler.inlineShuffle(shuffledCards);

        //Set the palyer cards.

        rearrange(shuffledCards, aggregateScoreCard);

        Debug.Log("Player Log1 is " + playerMap.Keys);
        Debug.Log("Player Log2 is " + playerMap.Values);
        Debug.Log("Player North  is " + playerMap[PlayerPosition.NORTH]);



        this.matchStarter = start;

        callBreakBidding.startBidding(start);

    }

    public CallBreakBidding callBreakBidding = new CallBreakBidding();

    public SpadeGameState() {

        gameStatus = GameStatus.BIDDING;
  
        foreach(PlayerPosition pos in System.Enum.GetValues(typeof(PlayerPosition))) {
            tricksWinnerCount[pos] = 0;
            gameScore[pos] = 0;
        }
    }

    public void setPlayers(Dictionary<PlayerPosition, Player> playerPosMap) {
        this.playerMap = playerPosMap;
    }

    public bool isHeartsBroken() {
        foreach(SpadeRound round in rounds) {
            foreach(SpadeMove move in round.moves) {
                if(move.card.suit.Equals("H"))
                    return true;
            }
        }
        return false;

    }

    public bool isSpadeBroken() {
        foreach(SpadeRound round in rounds) {
            foreach(SpadeMove move in round.moves) {
                if(move.card.suit.Equals("S"))
                    return true;
            }
        }
        return false;

    }


    public Card getSpadesBrokenCard() {
        foreach(SpadeRound round in rounds) {
            foreach(SpadeMove move in round.moves) {
                if(move.card.suit.Equals("S"))
                    return move.card;
            }
        }
        return null;

    }

    public void startNewRound() {

        this.gameStatus = GameStatus.IN_PROGRESS;
        int roundCompleted = rounds.Count;
        PlayerPosition roundStarter;
        Debug.Log("roundCompleted " + roundCompleted);
        if(roundCompleted <= 0) {
            roundStarter = matchStarter;
        } else {
            roundStarter = rounds[roundCompleted -1].winner;
            //callbreakRound.nextMovePlayer = PlayerPosition.WEST;

        }

        SpadeRound callbreakRound = new SpadeRound(roundCompleted + 1, roundStarter);
        rounds.Add(callbreakRound);
        Debug.Log("Round starter of round" + rounds.Count + " is " + roundStarter);

    }

    public void updateMove(SpadeMove move) {
        SpadeRound currRound = getCurrentRound();
        currRound.updateMove(move);

        playerMap[move.playerPosition].remainingCards = cardHelper.removeCard(playerMap[move.playerPosition].remainingCards, move.card);
        playerMap[move.playerPosition].playedCards.Add(move.card);

        if(currRound.roundState.Equals(SpadeRound.RoundState.COMPLETED)) {

            if(currRound.roundNumber == 13) {

                this.gameStatus = GameStatus.COMPLETED;
                onRoundComplete();
                calculateGameScore();
                //notify game complete

            } else {
                onRoundComplete();    
                //calculateGameScore();
                //notify round complete

            }
            
        }
    }

    public void updateBid(CallbreakBid callBreakBid) {
        callBreakBidding.updateBid(callBreakBid);

    }

    public void onGameComplete() {

        gameStatus = GameStatus.COMPLETED;
        //Let the notification go from MatchState.
    }

    public void onRoundComplete() {
        //Let the notification go from MatchState.

        SpadeRound currRound = getCurrentRound();
        gameScore[currRound.winner] += currRound.roundPoints;

        updateNilBustedRound();


        tricksWinnerCount[currRound.winner] = tricksWinnerCount[currRound.winner] + 1;

        //computeSandBag();

        //If team have more that target. Increment the sandbag.



        //startNewRound();

    }
    /* This has to be calculated before updating the score */


    private void updateNilBustedRound() {

        SpadeRound currRound = getCurrentRound();
        if(tricksWinnerCount[currRound.winner] == 0 && isNil(currRound.winner)) {
            //
            NilBustedEventController.InvokeEvent(currRound.winner);
            //NIL BUSTED FOR currRoundWinner
        }
        //Check the winner of the round. 
        //C

    }


    public SpadeRound getCurrentRound() {
        if(rounds.Count == 0)
            return null;
        return rounds[rounds.Count - 1];
    }



    public int getTarget(int teamNo) {
        if(teamNo == 1) {
            return callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.SOUTH] + callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.NORTH];

        } else {
        return callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.EAST] + callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.WEST];

        }

    }

    public int getTeamNo(PlayerPosition pos) {
        if(pos.Equals(PlayerPosition.SOUTH) || pos.Equals(PlayerPosition.NORTH)) {
            return 1;
        }
        return 2;

    }

     public int getAchieved(int teamNo) {
        if(teamNo == 1) {
            return tricksWinnerCount[PlayerPosition.SOUTH] + tricksWinnerCount[PlayerPosition.NORTH];

        } else {
        return tricksWinnerCount[PlayerPosition.EAST] + tricksWinnerCount[PlayerPosition.WEST];

        }

    }

    public int nilPenalty() {
        if(gameTarget == 250)
            return 50;

        return 100;
    }

    /// <summary>
    /// Sandbag penalty applied when a team crosses a bag threshold. Mirrors
    /// nilPenalty(): half-game (target 250) uses −50, standard (target 500)
    /// uses −100. Per standard Spades rules (Pagat / Hoyle).
    /// </summary>
    public int sandbagPenalty() {
        if (gameTarget == 250)
            return 50;
        return 100;
    }

    /// <summary>
    /// How many bags a team must accumulate before the next sandbag penalty
    /// fires. Half-game (target 250) → every 5 bags; standard (500) → every
    /// 10 bags. The 1:10 ratio with sandbagPenalty() is the standard rule.
    /// </summary>
    public int sandbagThreshold() {
        if (gameTarget == 250)
            return 5;
        return 10;
    }

    public void calculateGameScore() {
        
        /*
        int team1Bid = callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.SOUTH] + callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.NORTH];
        int team2Bid = callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.EAST] + callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.WEST];


        int team1Achieved = tricksWinnerCount[PlayerPosition.SOUTH] + tricksWinnerCount[PlayerPosition.NORTH];
        int team2Achieved = tricksWinnerCount[PlayerPosition.EAST] + tricksWinnerCount[PlayerPosition.WEST];
        */
        
        team1Score = getScore(1);
        team2Score = getScore(2);

        Debug.Log("team1Score is " + team1Score + " team2Score is " + team2Score);


    }

    //Indicated player have called for NIL. Does not indicate NIL finish or NIL busted
    public bool isNil(PlayerPosition pos) {
        if(callBreakBidding.callBreakBiddingData.playerBidAmount[pos] == 0)
            return true;

        return false;
    }


    public int getTricksWon(int teamNo) {
        PlayerPosition player1 = PlayerPosition.SOUTH, player2= PlayerPosition.NORTH;
        if(teamNo == 2) {
            player1 = PlayerPosition.EAST;
            player2 = PlayerPosition.WEST;
        }

        int achieved = tricksWinnerCount[player1] + tricksWinnerCount[player2];

        return achieved;
    }

    /// <summary>
    /// Returns the trick count that actually counts TOWARDS the team's bid
    /// contract — i.e. excludes tricks won by a partner who bid Nil. This
    /// matches the formula used in getScore() (lines 529-533): only tricks
    /// of partners who bid > 0 contribute to "achieved" for the contract.
    ///
    /// Added per P3 review feedback. Using getTricksWon() for sandbag-
    /// avoidance decisions can over-count when a partner bids Nil and
    /// busts — the busted nil's tricks inflate the team total but DON'T
    /// satisfy the team's contract, so the AI would falsely conclude
    /// "we've booked" and start ducking when we actually still need tricks.
    /// </summary>
    public int getEffectiveTricksWon(int teamNo) {
        PlayerPosition player1 = PlayerPosition.SOUTH, player2 = PlayerPosition.NORTH;
        if (teamNo == 2) {
            player1 = PlayerPosition.EAST;
            player2 = PlayerPosition.WEST;
        }

        int achieved = 0;
        if (callBreakBidding.callBreakBiddingData.playerBidAmount[player1] > 0)
            achieved += tricksWinnerCount[player1];
        if (callBreakBidding.callBreakBiddingData.playerBidAmount[player2] > 0)
            achieved += tricksWinnerCount[player2];
        return achieved;
    }


    public int getScore(int teamNo) {

        PlayerPosition player1 = PlayerPosition.SOUTH, player2= PlayerPosition.NORTH;
        if(teamNo == 2) {
            player1 = PlayerPosition.EAST;
            player2 = PlayerPosition.WEST;
        }

        int target = callBreakBidding.callBreakBiddingData.playerBidAmount[player1] + callBreakBidding.callBreakBiddingData.playerBidAmount[player2];
        int achieved = 0;

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player1] > 0)
            achieved = achieved + tricksWinnerCount[player1];

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player2] > 0)
            achieved = achieved + tricksWinnerCount[player2];

        int totalscore = 0;

        if(target == achieved) 
            totalscore =  target * 10;
        else if(target < achieved) 
            totalscore =  target * 10 + (achieved - target) ;
        else 
            totalscore =  -10 * target; 

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player1] == 0) {
            if(tricksWinnerCount[player1] == 0 ) {
                totalscore += nilPenalty();
            } else {
                totalscore -= nilPenalty();

            }
        }

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player2] == 0) {
            if(tricksWinnerCount[player2] == 0 ) {
                totalscore += nilPenalty();
            } else {
                totalscore -= nilPenalty();

            }
        }

        return totalscore;

    }

    public SpadeRound getLastCompletedRound() {
        int numOfRounds = rounds.Count;

        SpadeRound round = getCurrentRound();
        if(round.roundState.Equals(SpadeRound.RoundState.COMPLETED))
            return round;

        //Otherwise return the previous round

        if(rounds.Count >=2)
            return rounds[rounds.Count - 2];
        else
            return null;

    }

    public List<PlayerPosition> getNILPlayers() {
        List<PlayerPosition> playerPositions = new List<PlayerPosition>();
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            if(callBreakBidding.callBreakBiddingData.playerBidAmount[pos] == 0) 
                playerPositions.Add(pos);

        }
        return playerPositions;

    }



    public bool isPlayerNIL(PlayerPosition pos) {

        if(gameStatus.Equals(GameStatus.IN_PROGRESS) || gameStatus.Equals(GameStatus.COMPLETED)) {
            if(callBreakBidding.callBreakBiddingData.playerBidAmount[pos] == 0) 
                return true;
        }

        return false;

    }


    public bool isPlayerNILBusted(PlayerPosition pos) {
        if(gameStatus.Equals(GameStatus.IN_PROGRESS) || gameStatus.Equals(GameStatus.COMPLETED)) {
            if(callBreakBidding.callBreakBiddingData.playerBidAmount[pos] == 0 && tricksWinnerCount[pos] > 0) 
                return true;
        }
        return false;
    

    }


    public int maxAllowedBid(PlayerPosition pos) {
        PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();
        PlayerPosition partnerPos = playerPositionHelper.getOppositePlayerPosition(pos);
        if(callBreakBidding.callBreakBiddingData.playerBidAmount.ContainsKey(partnerPos)) {
            return 13 - callBreakBidding.callBreakBiddingData.playerBidAmount[partnerPos];
        }
        return 13;

    }

    public string getNILStatus(PlayerPosition pos) {
        if(callBreakBidding.callBreakBiddingData.playerBidAmount[pos] > 0)
            return "NA";
        
        if(isPlayerNILBusted(pos)) {
            return "NIL_BUSTED";

        } else {
            return "NIL_DONE";

        }


    }


    public string getNILStatus(int teamNo) {
        PlayerPosition player1 = PlayerPosition.SOUTH, player2= PlayerPosition.NORTH;
        if(teamNo == 2) {
            player1 = PlayerPosition.EAST;
            player2 = PlayerPosition.WEST;
        }

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player1] > 0 && callBreakBidding.callBreakBiddingData.playerBidAmount[player2] > 0) 
            return "NA";

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player1] == 0 ) {
            if(isPlayerNILBusted(player1)) {
            return "NIL_BUSTED";

            } else {
            return "NIL_DONE";

            }
        }

        if(callBreakBidding.callBreakBiddingData.playerBidAmount[player2] == 0 ) {
            if(isPlayerNILBusted(player2)) {
            return "NIL_BUSTED";

            } else {
            return "NIL_DONE";

            }
        }

        return "NA";    

    }



}