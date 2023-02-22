using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SpadeMatchState {
    string matchId;

    public string appVersion;

    string TAG = "BrayMatchStateLog";
    public AchievementManager achievementManager = new AchievementManager();
    public int matchPlayed = 0;
    public int gamePlayed = 0;
    public SpadeGameState latestCallbreakGame;
    public PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();
    public GameVariant gameVariant;


    public int gameTarget = 500;
    public int mercyTarget = -200;
    public bool isMercyEnabled;
    public bool isSandBagEnabled;


    public int cumulativeSandbagTeam1 = 0;
    public int cumulativeSandbagTeam2 = 0;



    [JsonIgnore]
    public Dictionary<PlayerPosition, Player> playerMap;

    public enum MatchState {
        IN_PROGRESS,
        COMPLETED
    }
    private bool stopped = false;

    public MatchState matchState;
    public int winnerTeam;

    public Dictionary<int, int> team1ScoreCard = new Dictionary<int, int>();
    public Dictionary<int, int> team2ScoreCard = new Dictionary<int, int>();

    public Dictionary<int, Dictionary<PlayerPosition, int>> scoreCard = new Dictionary<int, Dictionary<PlayerPosition, int>>();
    public Dictionary<PlayerPosition, int> aggregateScoreCard = new Dictionary<PlayerPosition, int>();
    public Dictionary<PlayerPosition, int> aggregateScoreCardWithLatestRound = new Dictionary<PlayerPosition, int>();


    public Dictionary<int, Dictionary<PlayerPosition, string>> nilPlayerScoreCard = new Dictionary<int, Dictionary<PlayerPosition, string>>();
    public Dictionary<int, string> nilTeam1MatchScorecard = new Dictionary<int, string>();
    public Dictionary<int, string> nilTeam2MatchScorecard = new Dictionary<int, string>();
    public Dictionary<int, bool> sandbagTeam1MatchScorecard = new Dictionary<int, bool>();
    public Dictionary<int, bool> sandbagTeam2MatchScorecard = new Dictionary<int, bool>();


    public int team1MatchScore = 0;
    public int team2MatchScore = 0;

    public List<Player> getPlayers() {
        List<Player> players = new List<Player>();
        players.Add(playerMap[PlayerPosition.EAST]);
        players.Add(playerMap[PlayerPosition.WEST]);
        players.Add(playerMap[PlayerPosition.SOUTH]);
        players.Add(playerMap[PlayerPosition.NORTH]);

        return players;
    }

    public void setPlayers(Dictionary<PlayerPosition, Player> playerPosMap) {
        this.playerMap = playerPosMap;
    }

    public SpadeMatchState() {
        this.matchState = MatchState.IN_PROGRESS;
        this.notificationQueue = new Queue<NotificationStatus>();
    }

    [JsonIgnore]
    public Queue<NotificationStatus> notificationQueue  = new Queue<NotificationStatus>();

    public  enum NotificationStatus {
        BID_START,
        BID,
        PASS_CARD,
        BIDDING_COMPLETE,
        MOVE,
        ROUND_COMPLETE,
        GAME_COMPLETE,
        MATCH_COMPLETE,    
        CARD_DISTRIBUTED
    }

    public void startGame() {
        Debug.Log("XXX startGame ");

        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            if(!aggregateScoreCard.ContainsKey(pos))
            aggregateScoreCard[pos] = 0; 
            if(!aggregateScoreCardWithLatestRound.ContainsKey(pos))
            aggregateScoreCardWithLatestRound[pos] = 0; 
        }


        SpadeGameState gameState = new SpadeGameState();
        gameState.gameVariant = this.gameVariant;
        gameState.isSandBagEnabled = isSandBagEnabled;
        gameState.isMercyEnabled = isMercyEnabled;
        gameState.gameTarget = gameTarget;
        gameState.setPlayers(playerMap);
        Debug.Log("XXX GamePlayed " + gamePlayed);
        //TODO: Change to previous played round. Or start with some preplayed deck.
        gamePlayed = gamePlayed + 1;
        Debug.Log("XXX GamePlayed " + gamePlayed);

        PlayerPosition start = PlayerPositionHelper.getRandomPlayerPosition();
        if(gamePlayed > 1) {
            start  = playerPositionHelper.getNextPlayerPosition(latestCallbreakGame.matchStarter);
        }

        latestCallbreakGame = gameState;

        //Card Shuffled.
        gameState.startGame(gamePlayed, new CardDeck().fullDeck(),start, aggregateScoreCard);

        nilPlayerScoreCard[gamePlayed] = new Dictionary<PlayerPosition, string>();
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {

            nilPlayerScoreCard[gamePlayed][pos] = "NA";
        }

        nilTeam1MatchScorecard[gamePlayed] = "NA";
        nilTeam2MatchScorecard[gamePlayed] = "NA";
        sandbagTeam1MatchScorecard[gamePlayed] = false;
        sandbagTeam2MatchScorecard[gamePlayed] = false;


        GamePlay.processNotification(this, NotificationStatus.CARD_DISTRIBUTED);
        notifyPlayers(NotificationStatus.CARD_DISTRIBUTED);



    }



    public void updateBid(CallbreakBid callbreakBid) {

        NotificationStatus notificationTemp;
        Debug.Log("YYY for " + this.getCurrentGameState().callBreakBidding.callBreakBiddingData.currentBiddingPosition);

        Debug.Log("Made bid of " + callbreakBid.bidValue + " " + callbreakBid.playerPosition ); 
        SpadeGameState currGame = getCurrentGameState();
        currGame.updateBid(callbreakBid);
        Debug.Log("No. of bid " + currGame.callBreakBidding.callBreakBiddingData.playerBidAmount.Keys.Count); 

        if(currGame.callBreakBidding.callBreakBiddingData.status.Equals(CallBreakBiddingData.Status.COMPLETE)) {
            currGame.startNewRound();
            currGame.gameStatus = SpadeGameState.GameStatus.IN_PROGRESS;
            Debug.Log("Bid completed"); 
            //Bidding is completed
            notificationQueue.Enqueue(NotificationStatus.BIDDING_COMPLETE);
            notificationTemp = NotificationStatus.BIDDING_COMPLETE;

        } else {
            notificationQueue.Enqueue(NotificationStatus.BID);
            notificationTemp = NotificationStatus.BID;
        }
        
        NotificationStatus notification = notificationQueue.Dequeue();
        Debug.Log("Notifying " + notification + " for " + callbreakBid);
        Debug.Log("ZZZ for " + this.getCurrentGameState().callBreakBidding.callBreakBiddingData.currentBiddingPosition);
        GamePlay.processNotification(this, notificationTemp);
        Debug.Log("YYY for " + this.getCurrentGameState().callBreakBidding.callBreakBiddingData.currentBiddingPosition);
        notifyPlayers(notificationTemp);

    }
    
    public void stop() {
        stopped = true;        
    }

    public void updateMove(SpadeMove move) {
        if(stopped)
            return;
        NotificationStatus notificationTemp;

        Debug.Log("Updating move " + move.card.getCardId() + " by " + move.playerPosition);
        SpadeGameState currGame = getCurrentGameState();

        if(!currGame.getCurrentRound().nextMovePlayer.Equals(move.playerPosition)) {
            return;
        }

        currGame.updateMove(move);

        if(currGame.gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED)) {

            updateScoreCardRoundComplete(currGame.getCurrentRound());
            updateScoreCard();
            if(isMatchCompleted()) {

                //Time to start a new Match
                //notify game complete
                notificationQueue.Enqueue(NotificationStatus.MATCH_COMPLETE);
                notificationTemp = NotificationStatus.MATCH_COMPLETE;
                this.matchState = MatchState.COMPLETED;
                determineWinner();

            } else {
                // game complete.    
                //notify new  game
                notificationQueue.Enqueue(NotificationStatus.GAME_COMPLETE);
                notificationTemp = NotificationStatus.GAME_COMPLETE;

            }
            
        } else {
            //Game is not complete. Check if round complete.
            if(currGame.getCurrentRound().roundState.Equals(SpadeRound.RoundState.COMPLETED)) {
                //Round complete.
                updateScoreCardRoundComplete(currGame.getCurrentRound());
                getCurrentGameState().startNewRound();
                notificationQueue.Enqueue(NotificationStatus.ROUND_COMPLETE);
                notificationTemp = NotificationStatus.ROUND_COMPLETE;
                Debug.Log("Current game have rounds = " + getCurrentGameState().rounds.Count);
            } else {
                 notificationQueue.Enqueue(NotificationStatus.MOVE);
                 notificationTemp = NotificationStatus.MOVE;

            }
        }

        //Dequeue the notification and update the players and the GamePlay Scene.

        NotificationStatus notification = notificationQueue.Dequeue();

        Debug.Log("AAA Notification is " + notification);

        if(notificationTemp != null) {
            GamePlay.processNotification(this, notificationTemp);
            notifyPlayers(notificationTemp);

            if(notificationTemp.Equals(NotificationStatus.GAME_COMPLETE)) {
                achievementManager.updateOnGameComplete(currGame);
                //GamePlay.startNextMatchStatic();
            }
            if(notificationTemp.Equals(NotificationStatus.MATCH_COMPLETE)) {
                achievementManager.updateOnGameComplete(currGame);
                achievementManager.updateOnMatchComplete(this);

                //GamePlay.startNextMatchStatic();
            }
        }
    }

    private bool isMatchCompleted() {
        int maxAll = Mathf.Max(team1MatchScore, team2MatchScore);
        Debug.Log("maxALL " + maxAll);
        if(maxAll >= gameTarget)
            return true;

        if(isMercyEnabled) {
            int minAll = Mathf.Min(team1MatchScore, team2MatchScore);
            Debug.Log("maxALL " + maxAll);
            if(minAll <= mercyTarget)
                return true;
        }
        //If mercy rule is enabled.

        return false;

    }




    public void notifyPlayers(NotificationStatus notificationStatus) {
          List<Player> players = getPlayers();
          PlayerPosition turnPlayer = PlayerPosition.SOUTH;
          foreach(Player player in players) {
              if(player.isMyMove(this.getCurrentGameState())) {
                    turnPlayer = player.playerPosition;
              } 
          }

          foreach(Player player in players) {
              if(!player.playerPosition.Equals(turnPlayer))
                  player.updateMatchNotification(this, notificationStatus);
          }
          playerMap[turnPlayer].updateMatchNotification(this, notificationStatus);
    }

    public SpadeGameState getCurrentGameState() {
        return latestCallbreakGame;
        //return callbreakGames[callbreakGames.Count -1];
    }

    public void onRoundComplete() {

        SpadeGameState currGame = getCurrentGameState();

        
    }

    public void onGameComplete() {
        
    }



    public void onMatchComplete() {
        
    }

    public void determineWinner() {


        if(team1MatchScore > team2MatchScore) {
            winnerTeam = 1;
        } else {
            winnerTeam = 2;
        }


        //Debug.Log(TAG + " Winner is " +  winnerTeam + " and team1MatchScore is " + team1MatchScore + " and team2MatchScore is " + team2MatchScore);

    }

    public void updateScoreCardRoundComplete(SpadeRound round) {
        aggregateScoreCardWithLatestRound[round.winner]  = aggregateScoreCardWithLatestRound[round.winner] + round.roundPoints;
        computeSandBag(round);

    }

    private int sandBagTillNow(int teamNo) {
        var game = getCurrentGameState();
        int counter = 0;
        for(int i=1;i<game.gameNumber;i++) {
            if(teamNo ==1) {
                if(sandbagTeam1MatchScorecard[i]) {
                    counter = counter + 1;
                }
            } else {
                if(sandbagTeam2MatchScorecard[i]) {
                    counter = counter + 1;
                }
            }
        }
        return counter;
    }

    private void computeSandBag(SpadeRound currRound) {
        var game = getCurrentGameState();
        Debug.Log("In computeSandBag");
        int winningTeam = game.getTeamNo(currRound.winner);
        var sandBagScore = 0;
        if(isSandBagEnabled) {
        Debug.Log("In isSandBagEnabled");
            if(game.getAchieved(winningTeam) >= game.getTarget(winningTeam) + 1) {
                Debug.Log("In sandBag detected");
                if(winningTeam == 1) {
                    cumulativeSandbagTeam1 = cumulativeSandbagTeam1 +1;
                    sandBagScore =  cumulativeSandbagTeam1;                       

                    //Generate sandbag increase event. 

                    //Generate sandbag event. 

                }
                else {
                    cumulativeSandbagTeam2 = cumulativeSandbagTeam2 +1;                
                    sandBagScore =  cumulativeSandbagTeam2;                       
                }
                if(sandBagScore > 0 && sandBagScore %10 == 0 && sandBagScore == (sandBagTillNow(winningTeam) + 1) * 10) {
                    getCurrentGameState().isSandbagThisGame = true;
                    SandBagEventController.InvokeSandBagCompleteEvent(winningTeam);
                    if(winningTeam == 1) {
                        sandbagTeam1MatchScorecard[game.gameNumber] = true;
                    } else {
                        sandbagTeam2MatchScorecard[game.gameNumber] = true;
                    }

                } else {
                    SandBagEventController.InvokeSandBagScoreEvent(winningTeam);

                }
            }
        }

    }


    public void updateScoreCard() {
        SpadeGameState currGame = getCurrentGameState();



        team1MatchScore += currGame.team1Score;
        team2MatchScore += currGame.team2Score;


        team1ScoreCard[currGame.gameNumber] = currGame.team1Score;
        team2ScoreCard[currGame.gameNumber] = currGame.team2Score;

        Debug.Log(TAG + "Updating scorecard team1MatchScore " + team1MatchScore);

        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            nilPlayerScoreCard[currGame.gameNumber][pos] = currGame.getNILStatus(pos);
        }

        nilTeam1MatchScorecard[currGame.gameNumber] = currGame.getNILStatus(1);
        nilTeam2MatchScorecard[currGame.gameNumber] = currGame.getNILStatus(2);

        //nilMatchScorecard[currGame.gameNumber] 

        //update NIL of both team.
        //Update Sandbag of both team.

        
    }



}







 

