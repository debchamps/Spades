using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AchievementManager{



    public static string GAME_ID = "SPADES_OFFLINE_NEW";

    string TAG = "AchievementManager";

    string lasNMatch = "";


    public static class AchievementKey
    {
        public static string NUMBER_WIN = "NUMBER_WIN";
        public static string NIL_WIN = "NIL_WIN";
        public static string NEGATIVE_SCORE_MATCH = "NEGATIVE_SCORE_MATCH";
        public static string DOUBLE_NIL = "DOUBLE_NIL";
        public static string ACHIEVE_10_PLUS_TARGET = "ACHIEVE_10_PLUS_TARGET";
        public static string WINNING_ALL_THIRTEEN_HAND = "WINNING_ALL_THIRTEEN_HAND";
        public static string BUST_OPPONENT_NIL = "BUST_OPPONENT_NIL";

        public static string CONSECUTIVE_WIN = "CONSECUTIVE_WIN";
    }


    AchievementDao achievementDao = new AchievementDao();
    StatisticsDao statisticsDao = new StatisticsDao();

    Achievement achievement1 = new Achievement(AchievementKey.NUMBER_WIN, "Win 500 Spade matches", 500, 0);
    Achievement achievement2 = new Achievement(AchievementKey.NIL_WIN, "Win 100 round after calling NIL", 100, 0);
    Achievement achievement3 = new Achievement(AchievementKey.BUST_OPPONENT_NIL, "Bust NIL of opponent 30 times", 30, 0);
    Achievement achievement4 = new Achievement(AchievementKey.ACHIEVE_10_PLUS_TARGET, "Achieve target of 10 or more 50 times", 50, 0);
    Achievement achievement5 = new Achievement(AchievementKey.DOUBLE_NIL, "Both Partner and Me acheiving NIL together 5 times", 30, 0);

    //Achievement achievement6 = new Achievement("CALLBREAK_WIN3", "Winnint with more than 5 point margin", 200, 0);

    public AchievementData getAchievementData() {
        //achievementDao.reset(GAME_ID);
        //initiate();
        return  achievementDao.getAchievementData(GAME_ID);

    }

    public void initiate() {

        AchievementData achievementData = new AchievementData();
        List<Achievement> achievementList = new List<Achievement>();
        achievementList.Add(achievement1);
        achievementList.Add(achievement2);
        achievementList.Add(achievement3);
        achievementList.Add(achievement4);
        //achievementList.Add(achievement5);
        //achievementList.Add(achievement5);
        //achievementList.Add(achievement6);

        achievementData.achievements = achievementList;
        achievementData.gameType = GAME_ID;
        achievementData.mode = "OFFLINE";

        achievementDao.initiateAchievement(achievementData);

    }


    public bool isNConsecutiveWin(string str, int n) {

        //Get the last 3
        
        if(str.Length < 3)
            return false;        

        string substr = str.Substring(str.Length - n);

        bool isLoss = false;

        for(int i=0;i<n;i++) {
            if(substr[i] != 'W') {
                return false;
            }

        }

        return true;

    }


    public bool isOpponentNilBusted(SpadeGameState gameState) {

         if(gameState.isPlayerNIL(PlayerPosition.EAST) && gameState.isPlayerNILBusted(PlayerPosition.EAST)) 
            return true;
         if(gameState.isPlayerNIL(PlayerPosition.WEST) && gameState.isPlayerNILBusted(PlayerPosition.WEST)) 
            return true;
         return false;

    }

    public bool isDoubleNILDefended(SpadeGameState gameState) {
         if(gameState.isPlayerNIL(PlayerPosition.SOUTH) && !gameState.isPlayerNILBusted(PlayerPosition.SOUTH) && gameState.isPlayerNIL(PlayerPosition.NORTH) && !gameState.isPlayerNILBusted(PlayerPosition.NORTH)) 
            return true;
        return false;

    }


    public bool isNilDefended(SpadeGameState gameState) {

         if(gameState.isPlayerNIL(PlayerPosition.SOUTH) && !gameState.isPlayerNILBusted(PlayerPosition.SOUTH)) 
            return true;
         return false;

    }

    public bool zeroPointMatch(SpadeGameState gameState) {
         if(gameState.gameScore[PlayerPosition.SOUTH] <= 0 ) 
            return true;
         return false;

    }

    public bool isNegativeScore(SpadeMatchState matchState) {
        if(matchState.aggregateScoreCardWithLatestRound[PlayerPosition.SOUTH] < 0) 
            return true;
         return false;

    }

    public bool lessThan20PercentScore(SpadeMatchState matchState) {
        if(matchState.aggregateScoreCardWithLatestRound[PlayerPosition.SOUTH] < .2f * matchState.gameTarget) 
            return true;
         return false;

    }

    public bool isWinner(SpadeMatchState matchState) {
        int southScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.SOUTH];
        int northScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.NORTH];
        int eastScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.EAST];
        int westScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.WEST];

        if(southScore <= Mathf.Min(Mathf.Min(northScore, westScore), eastScore)) {
            return true;
        }

        return false;
    }


    bool wonNoTrick(SpadeGameState gameState) {
        if(gameState.tricksWinnerCount[PlayerPosition.SOUTH] == 0)
            return true;

        return false;
    }


    public bool achievedTargetX(SpadeGameState gameState, int targetToAchieve) {
        int target = gameState.callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.SOUTH] + gameState.callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.NORTH];
        int achieved = gameState.tricksWinnerCount[PlayerPosition.SOUTH] + gameState.tricksWinnerCount[PlayerPosition.NORTH];

        if(target >= targetToAchieve && achieved >= target)
            return true;

        return false;

    }

    public void updateOnGameComplete(SpadeGameState gameState) {

        initiate();        

        Debug.Log(TAG + " updateOnGameComplete");


        bool isNilDefended = wonNoTrick(gameState);
        bool isDoubleNilDefended = wonNoTrick(gameState);
        bool achievedTenPlusTarget = achievedTargetX(gameState, 10);
        bool isOpponentNilBustedNow = isOpponentNilBusted(gameState);

        if(isNilDefended) {
            achievementDao.incrementAchievement(GAME_ID, AchievementKey.NIL_WIN);
            statisticsDao.increment(CallbreakStatisticsKey.NO_ROUND_WIN);

        }   

        if(isDoubleNilDefended) {
            achievementDao.incrementAchievement(GAME_ID, AchievementKey.DOUBLE_NIL);

        }

        if(achievedTenPlusTarget) {
            achievementDao.incrementAchievement(GAME_ID, AchievementKey.ACHIEVE_10_PLUS_TARGET);
        }

        if(isOpponentNilBustedNow) {

            achievementDao.incrementAchievement(GAME_ID, AchievementKey.BUST_OPPONENT_NIL);
        }


        /*
        if(achievedEightTarget) {
            achievementDao.incrementAchievement(GAME_ID, "CALLBREAK_8PLUS");
            statisticsDao.increment(CallbreakStatisticsKey.EIGHT_PLUS_TARGET_ACHIEVED);

        }   


        
        int target = gameState.callBreakBidding.callBreakBiddingData.playerBidAmount[PlayerPosition.SOUTH];
        float scoreRound = gameState.gameScore[PlayerPosition.SOUTH];

        if(scoreRound > 0)
            statisticsDao.increment(CallbreakStatisticsKey.getTargetAchievedKey(target, true));
        else 
            statisticsDao.increment(CallbreakStatisticsKey.getTargetAchievedKey(target, false));

        statisticsDao.add(CallbreakStatisticsKey.TOTAL_POINTS, scoreRound);
        statisticsDao.increment(CallbreakStatisticsKey.TOTAL_ROUNDS);
        */

    }

    void NCalculateWin(SpadeMatchState matchState) {
        string lastNResults = statisticsDao.getString(CallbreakStatisticsKey.LAST_N_MATCH_RESULT);

        if(lastNResults.Length > 50) {

             lastNResults = lastNResults.Substring(lastNResults.Length - 40);

        }

        bool matchWonBySouth = isGameWon(matchState, PlayerPosition.SOUTH);

        if(matchWonBySouth) {
            lastNResults = lastNResults + "W";
        } else {
            lastNResults = lastNResults + "L";
        }

        statisticsDao.setString(CallbreakStatisticsKey.LAST_N_MATCH_RESULT, lastNResults);

        bool isConsecutiveWin = isNConsecutiveWin(lastNResults, 2);

        if(isConsecutiveWin) {
            lastNResults = lastNResults + "X";
            statisticsDao.setString(CallbreakStatisticsKey.LAST_N_MATCH_RESULT, lastNResults);

            //achievementDao.incrementAchievement(GAME_ID, AchievementsDao.AchievementKey.CONSECUTIVE_WIN);
            statisticsDao.increment(CallbreakStatisticsKey.CONSECUTIVE_WIN);

        } else {
            //Nothing to do/
        }

    }

    public void updateOnMatchComplete(SpadeMatchState matchState) {

        NCalculateWin(matchState);

        bool isWinner = isGameWon(matchState, PlayerPosition.SOUTH);

        if(isWinner) {
            achievementDao.incrementAchievement(GAME_ID, AchievementKey.NUMBER_WIN);
        }

        bool scoreNegative = isNegativeScore(matchState);


        statisticsDao.increment(CallbreakStatisticsKey.getRankKey(getPlayerRankMatch(matchState)));
        

        //lastNResults = lastNResults + ""

        /*

        Debug.Log(TAG + " updateOnMatchComplete");
        
        statisticsDao.increment(CallbreakStatisticsKey.getRankKey(getPlayerRankMatch(matchState)));

        bool positiveInAllHands = scorePositiveInAllRound(matchState, PlayerPosition.SOUTH);

        Debug.Log(TAG + " positiveInAllHands is " + positiveInAllHands);

        if(positiveInAllHands) {
            achievementDao.incrementAchievement(GAME_ID, "CALLBREAK_ALLHAND_POSITIVE");

        }
        bool isWinner = isGameWon(matchState, PlayerPosition.SOUTH);
        Debug.Log(TAG + " isWinner is " + isWinner);

        if(isWinner) {
            achievementDao.incrementAchievement(GAME_ID, "CALLBREAK_WIN");
        }

        bool isTwentyPlusScore = totalScoreGreaterThanX(matchState, 15.0f, PlayerPosition.SOUTH);
        Debug.Log(TAG + " isTwentyPlusScore is " + isTwentyPlusScore);

        if(isTwentyPlusScore) {
            achievementDao.incrementAchievement(GAME_ID, "CALLBREAK_GAME_20POINT");

        }

        */

        /*

        PlayerPrefs.SetInt("positiveInAllHandCount", 50);
        PlayerPrefs.SetInt("positiveInAllHandCount", 50);
        PlayerPrefs.SetInt("positiveInAllHandCount", 50);
        PlayerPrefs.SetInt("positiveInAllHandCount", 50);
        PlayerPrefs.SetInt("positiveInAllHandCount", 50);

        */

        
    }


    int getPlayerRankMatch(SpadeMatchState matchState) {
        int rank = 1;

        if(matchState.winnerTeam == 1)
            return 1;
        else 
            return 2;

    }


    bool isGameWon(SpadeMatchState matchState, PlayerPosition pos) {
        Debug.Log(TAG + "matchState.winner " + matchState.winnerTeam);

        if(matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED) && matchState.winnerTeam == 1) {
            return true;
        }
        return false;
    }


}