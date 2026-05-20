using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoogleMobileAds.Api;
//using GoogleMobileAds.Placement;
using System;

public class ScoreboardScript : MonoBehaviour
{

    static  int gameNumber;
    public static bool isAdLoaded = false;
    Transform northTransform, southTransform, eastTransform, westTransform;

    void Start() {

    }


    public void setMatchScore(SpadeMatchState matchState) {
        GameObject.Find("matchscore1").GetComponent<Text>().text = matchState.team1MatchScore.ToString();
        GameObject.Find("matchscore2").GetComponent<Text>().text = matchState.team2MatchScore.ToString();
    }


    public void init() {
        rankPlayers();
    }




    bool displayed = false;

    string TAG = "ScoreboardScript";
    ConfigManager configManager = new ConfigManager();
    // Start is called before the first frame update


    //Color NEGATIVE_COLOR  = new Color(200f/255,0,0);
    //Color POSITIVE_COLOR  = new Color(0,100f/255,0);

    List<Sequence> sequences = new List<Sequence>();

    Color NEGATIVE_COLOR  = new Color(246f/255,183f/255,27f/255);
    Color POSITIVE_COLOR  = new Color(246f/255,183f/255,27f/255);

    public void enableScoreCardWithDelay(SpadeMatchState callbreakMatchState, bool shouldDisappear) {
        StartCoroutine(enableScoreCardWithDelayIEnum(callbreakMatchState, shouldDisappear));

    }

    public IEnumerator enableScoreCardWithDelayIEnum(SpadeMatchState callbreakMatchState, bool shouldDisappear) {
        float delay = getDelayTime(callbreakMatchState);
        yield return new WaitForSeconds(delay);

        enableScoreCard(callbreakMatchState, shouldDisappear);
    }



    public void enableScoreCard(SpadeMatchState callbreakMatchState, bool shouldDisappear) {
        if(displayed)
            return;

        if(callbreakMatchState == null)
        return;

        if(callbreakMatchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.BIDDING))
            return;

        displayed = true;

        closeOthers();
        gameNumber = callbreakMatchState.getCurrentGameState().gameNumber;

        if(!callbreakMatchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED)) {
            gameNumber = gameNumber -1;
        }

        GameObject.Find("nextroundicon").GetComponent<Image>().DOFade(0, .1f);
        if(shouldDisappear) {
            GameObject.Find("previousroundicon").GetComponent<Image>().DOFade(0, .1f);
        } else {
            GameObject.Find("previousroundicon").GetComponent<Image>().DOFade(1, .1f);

        }


        GameObject.Find("ScoreCardV2/footer").GetComponent<Text>().text = LocalizationManager.Instance.Get("scoreboard_footer", callbreakMatchState.gameTarget.ToString());


        updateScore(callbreakMatchState, gameNumber);

        GameObject parentObj = GameObject.Find("ScoreCardV2");

        bool isMatchCompleted = false;
        if(callbreakMatchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
            isMatchCompleted = true;
        bool isGameCompleted = false;
        if(callbreakMatchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED))
            isGameCompleted = true;

        if (isMatchCompleted || isGameCompleted)
        {
            loadAdOnly();
        }


        Debug.Log("Match Completed is isMatchCompleted" + isMatchCompleted);
        if(isMatchCompleted) {
            parentObj = GameObject.Find("ScoreCardV2");
            setMatchComplete(callbreakMatchState);
        } else {
            GameObject.Find("ScoreCardV2/winner").GetComponent<Image>().enabled = false;
        }

        float delay = 0f;
        
        if(callbreakMatchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED)) {
            delay = configManager.callbreakTimingConfig.waitBeforeShowingScoreboardAfterRoundsInSeconds;
            //delay = delay + configManager.callbreakTimingConfig.additionalDelayForShootingMoonAnimation;
        }  
        
        float  floatY = Screen.height/2;
        float moveTime = 0.4f;

        Sequence playSequence = DOTween.Sequence();

        playSequence.Append(parentObj.transform.DOMove(new Vector3(Screen.width/2,floatY),moveTime));

        playSequence.AppendCallback(rankPlayers);

        if(isMatchCompleted)
            playSequence.AppendCallback(postAnimation);


        playSequence.Play();

        /*
        if(isMatchCompleted)
        parentObj.transform.DOMove(new Vector3(Screen.width/2,floatY),moveTime).SetDelay(delay).OnComplete(postAnimation);
        else 
        parentObj.transform.DOMove(new Vector3(Screen.width/2,floatY),moveTime).SetDelay(delay).OnComplete(rankPlayers);
        */

    }

    public void rankPlayers() {
        Dictionary<PlayerPosition, int> playerRank =  scoreRank(GamePlay.matchState);

        int team1Score = GamePlay.matchState.team1MatchScore;
        int team2Score = GamePlay.matchState.team1MatchScore;

            Vector3 scorePos1 = GameObject.Find("ScoreArea1" ).transform.position;
            Vector3 scorePos2 = GameObject.Find("ScoreArea2" ).transform.position;
        if(team1Score >= team2Score) {
            GameObject.Find(PlayerPositionHelper.getName(PlayerPosition.SOUTH).ToLower() + "Score").transform.DOMoveY(scorePos1.y, .6f);
            GameObject.Find(PlayerPositionHelper.getName(PlayerPosition.EAST).ToLower() + "Score").transform.DOMoveY(scorePos2.y, .6f);

        } else {

           GameObject.Find(PlayerPositionHelper.getName(PlayerPosition.EAST).ToLower() + "Score").transform.DOMoveY(scorePos1.y, .6f);
            GameObject.Find(PlayerPositionHelper.getName(PlayerPosition.SOUTH).ToLower() + "Score").transform.DOMoveY(scorePos2.y, .6f);
        }

        foreach(PlayerPosition pos in  PlayerPositionHelper.PLAYER_POSITIONS) {

            //int rank = playerRank[pos];
            //Vector3 scorePos = GameObject.Find("ScoreArea" + rank.ToString()).transform.position;

            //GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "Score").transform.DOMoveY(scorePos.y, .6f);
        } 
    }


    public void previousRound() {

        if(gameNumber <=1) {
            gameNumber =1; 
            return;
        } else {
             gameNumber = gameNumber - 1;
             updateScore(GamePlay.matchState, gameNumber);
             if(gameNumber == 1) 
                GameObject.Find("previousroundicon").GetComponent<Image>().DOFade(0, .1f);    


        }

        if(gameNumber <= GamePlay.matchState.getCurrentGameState().gameNumber -1) {
                GameObject.Find("nextroundicon").GetComponent<Image>().DOFade(1, .1f);        
        }


    }


    public void nextRound() {

        int maxGameNumber =GamePlay.matchState.getCurrentGameState().gameNumber;
        if(!GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED)) {
            maxGameNumber = maxGameNumber -1;
        }

        if(gameNumber >=maxGameNumber) {
            gameNumber = maxGameNumber;
        } else {
            gameNumber = gameNumber + 1;        
            updateScore(GamePlay.matchState, gameNumber);
            if(gameNumber >=maxGameNumber -1 )
                GameObject.Find("nextroundicon").GetComponent<Image>().DOFade(0, .1f);        


        }
        if(gameNumber >= 2) {
            GameObject.Find("previousroundicon").GetComponent<Image>().DOFade(1, .1f);    
        }


    }

    public void postAnimation() {
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            getPrizeObj(pos).GetComponent<Image>().DOFade(1, .2f);    
        }

        //Check if any NIL or sandbags

        //StartCoroutine(winnerAnimation());
        //otherAnimation(GameObject.Find("southPrizeTest"));
        StartCoroutine(otherAnimation(GameObject.Find("southScore/prize")));

        StartCoroutine(otherAnimation(GameObject.Find("eastScore/prize")));
        StartCoroutine(otherAnimation(GameObject.Find("westScore/prize")));
        StartCoroutine(otherAnimation(GameObject.Find("northScore/prize")));

    }


    public void enableScoreCardAndDisappear(SpadeMatchState callbreakMatchState) {
        if(callbreakMatchState != null)
            StartCoroutine(enableScoreCardAndDisappearNumerator(callbreakMatchState, true));
    
    }

    public void enableScoreCardView(SpadeMatchState callbreakMatchState) {
        if(callbreakMatchState != null)
        StartCoroutine(enableScoreCardAndDisappearNumerator(callbreakMatchState, false));
    
    }

    public IEnumerator winnerAnimation() {




        float initialDelay = 1f;

        yield return new WaitForSeconds(initialDelay);


        GameObject obj = GameObject.Find("southScore/prize");
        Vector3 currScale = obj.transform.localScale;
        Vector3 enlargedScale = new Vector3(currScale.x * 1.25f, currScale.y*1.25f, 1);        

        Sequence winnerAnimationSequence=  DOTween.Sequence();
        winnerAnimationSequence.AppendInterval(initialDelay);
        winnerAnimationSequence.Append(obj.transform.DOScale(enlargedScale, .25f));
        winnerAnimationSequence.Append(obj.transform.DOScale(currScale, .25f));
        winnerAnimationSequence.SetLoops(100,LoopType.Restart);
        sequences.Add(winnerAnimationSequence);

        winnerAnimationSequence.Play();
    }



    void setScore(int roundNumber) {
        //If round Number is current.

        Dictionary<PlayerPosition , int> scoreTillNow = new Dictionary<PlayerPosition , int>();

        for(int i=1; i<=roundNumber;i++) {
            
        }
                    
    }

    public IEnumerator otherAnimation(GameObject obj) {

        float initialDelay = 1f;

        yield return new WaitForSeconds(initialDelay);

        Sequence sequence=  DOTween.Sequence();

        //GameObject obj = GameObject.Find("southPrizeTest");
        Vector3 currScale = obj.transform.localScale;
        float  currY = obj.transform.position.y;
        Vector3 enlargedScale = new Vector3(currScale.x * 1.25f, currScale.y*1.25f, 1);

        Rect objRect = RectTransformExt.GetWorldRect((RectTransform)obj.transform);
        
        float animTime = 0.125f;
        float moveDelta = 0.05f;

        sequence.AppendInterval(initialDelay);
        sequence.Append(obj.transform.DOMoveY(obj.transform.position.y + objRect.height* moveDelta, animTime));
        sequence.Append(obj.transform.DOMoveY(obj.transform.position.y - objRect.height*moveDelta, animTime));
        sequence.Append(obj.transform.DOMoveY(obj.transform.position.y + objRect.height*moveDelta, animTime));
        sequence.Append(obj.transform.DOMoveY(obj.transform.position.y - objRect.height*moveDelta, animTime));

        sequence.SetLoops(100,LoopType.Restart);
        sequences.Add(sequence);
        sequence.Play();

    }

    float getDelayTime(SpadeMatchState callbreakMatchState) {

        float delay = configManager.callbreakTimingConfig.waitBeforeShowingScoreboardAfterRoundsInSeconds;

        if(callbreakMatchState == null)
            return delay;
        List<PlayerPosition> nilPlayers = callbreakMatchState.getCurrentGameState().getNilAchievedPlayers();
        if(nilPlayers != null && nilPlayers.Count > 0)
            delay = delay + configManager.callbreakTimingConfig.additionalDelayForShootingMoonAnimation;
        return delay;

    }

    public IEnumerator enableScoreCardAndDisappearNumerator(SpadeMatchState callbreakMatchState, bool shouldDisappear) {

        float delay = getDelayTime(callbreakMatchState);

        if(shouldDisappear || callbreakMatchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED) || callbreakMatchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED))
            yield return new WaitForSeconds(delay);
        enableScoreCard(callbreakMatchState, shouldDisappear);
        if(shouldDisappear) {
            yield return new WaitForSeconds(configManager.callbreakTimingConfig.scoreboardAutoDisappearTime/1000);
            disableScoreCard();
        }

    }

    // Check if any problem.
    public void onlyClose() {
       if(!displayed)
        return;
       displayed = false;
       
    }


    public void close() {
       if(!displayed)
        return;
       displayed = false;

       resetPrize();

        GameObject parentObj = GameObject.Find("ScoreCardV2");

        bool isMatchCompleted = false;
        if(GamePlay.matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
            isMatchCompleted = true;


        if(isMatchCompleted)
            parentObj = GameObject.Find("ScoreCardV2");
        
        float  floatY = Screen.height/2;

        parentObj.transform.position = new Vector3(2 * Screen.width,floatY,0);

        if(GamePlay.matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
            GamePlay.startNextMatchStatic();
        else if(GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED))
            GamePlay.startNextMatchStatic();

         
    }

    private void loadAdOnly()
    {
        if (!AdManager.isInterstitialAdEnabled())
        {
            return;
        }

        if (!isAdLoaded)
        {
            GameObject.Find("ScriptEmpty").GetComponent<AdManager>().LoadInterstitialAd();

            //InterstitialAdGameObject interstitialAd = MobileAds.Instance.GetAd<InterstitialAdGameObject>("Interstitial Ad");
            //interstitialAd.LoadAd();
            isAdLoaded = true;
        }
        else
        {
            //Ad already loaded.
        }

    }

    private IEnumerator loadAd()
    {

        DebugLog.Log("Loading InterstitialGameEnd");
        //Wait till The score disappears.
        yield return new WaitForSeconds(0.5f);

        bool loadAdEnabled = AdManager.isInterstitialAdEnabled();
        if (loadAdEnabled)
        {
            try
            {

                DebugLog.Log("Fetched InterstitialGameEnd");


                GameObject.Find("ScriptEmpty").GetComponent<AdManager>().ShowInterstitialAd();

                //InterstitialAdGameObject interstitialAd = MobileAds.Instance.GetAd<InterstitialAdGameObject>("Interstitial Ad");
                //interstitialAd.ShowIfLoaded();
                //SceneManager.LoadScene("CommonCardScene");
                isAdLoaded = false;

                DebugLog.Log("Show InterstitialGameEnd");

                GamePlay.startNextMatchStatic();
            }
            catch (Exception e)
            {
                isAdLoaded = false;
                DebugLog.Log("Exception in Showing Ad" + e.ToString());
                Debug.LogException(e, this);
                GamePlay.startNextMatchStatic();
            }

        }
        else
        {
            GamePlay.startNextMatchStatic();
        }
    }


    private void cleanAnimations() {
        foreach(Sequence seq in sequences) {
            seq.Kill();
        }
    }

    private void resetPrize() {

        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            getPrizeObj(pos).GetComponent<Image>().DOFade(0, .2f);    
        }

    }

    public void disableScoreCard() {
        if(!displayed)
        return;
       displayed = false;
       cleanAnimations();
 
        GameObject parentObj = GameObject.Find("ScoreCardV2");

        bool isMatchCompleted = false;
        if(GamePlay.matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
            isMatchCompleted = true;


        if(isMatchCompleted)
            parentObj = GameObject.Find("ScoreCardV2");

        float floatY = Screen.height/2;

        parentObj.transform.DOMove(new Vector3(Screen.width * 2,floatY),0.4f).SetDelay(0f).OnComplete(resetPrize);


        if (GamePlay.matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED))
        {
            StartCoroutine(loadAd());

        }
        else if (GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED))
        {
            if(GamePlay.matchState.getCurrentGameState().gameNumber %3 == 0)
            {
                StartCoroutine(loadAd());

            }
            else
            {
                GamePlay.startNextMatchStatic();

            }


        }

    }

    public void closeOthers() {
        //GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().disableScoreCard();
        GameObject.Find("ScriptEmpty").GetComponent<SettingsScript>().close();
        GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().close();
    }


    void setPenaltyImage(GameObject obj, string penaltyType) {

            if(penaltyType.Equals("NIL_DONE")) { 
                obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");  
            } else {
                obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");  
            }
             obj.GetComponent<Image>().enabled = true;

    }

    public void setNILPenalty(SpadeMatchState matchState, int gameNumber,  int teamNo) {
        string teamPenalty ="NA", player1Penalty = "NA", player2Penalty = "NA";
        GameObject penalty = GameObject.Find("ScoreArea" + teamNo.ToString() + "/penalty");
        GameObject penalty1= GameObject.Find("ScoreArea" + teamNo.ToString() + "/penalty1");;
        GameObject penalty2 = GameObject.Find("ScoreArea" + teamNo.ToString() + "/penalty2");;
        
        if(teamNo == 1) {
             teamPenalty = matchState.nilTeam1MatchScorecard[gameNumber]; 
             player1Penalty = matchState.nilPlayerScoreCard[gameNumber][PlayerPosition.SOUTH];
             player2Penalty = matchState.nilPlayerScoreCard[gameNumber][PlayerPosition.NORTH];

        } else {
             teamPenalty = matchState.nilTeam2MatchScorecard[gameNumber]; 
             player1Penalty = matchState.nilPlayerScoreCard[gameNumber][PlayerPosition.EAST];
             player2Penalty = matchState.nilPlayerScoreCard[gameNumber][PlayerPosition.WEST];

        }

        Debug.Log("teamNo" + teamNo + "gameNumber " + gameNumber + "teamPenalty " + teamPenalty + " player1Penalty: " + player1Penalty + " player2Penalty: " + player2Penalty);

        if(teamPenalty.Equals("NA"))
            return;


        if(!player1Penalty.Equals("NA") && !player2Penalty.Equals("NA")) {
            setPenaltyImage(penalty1, player1Penalty);
            setPenaltyImage(penalty2, player2Penalty);
            //Both has penalty.

        } else {

            setPenaltyImage(penalty, teamPenalty);
        }


    }

    void disableAllPenalty() {
        GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().enabled = false;
        GameObject.Find("ScoreArea2/penalty1").GetComponent<Image>().enabled = false;
        GameObject.Find("ScoreArea2/penalty2").GetComponent<Image>().enabled = false;
        GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().enabled = false;
        GameObject.Find("ScoreArea1/penalty1").GetComponent<Image>().enabled = false;
        GameObject.Find("ScoreArea1/penalty2").GetComponent<Image>().enabled = false;

    }

    public void updateScore(SpadeMatchState matchState, int gameNumber) {

        disableAllPenalty();

        //Set the header
        GameObject.Find("roundNumberText").GetComponent<Text>().text = LocalizationManager.Instance.Get("round_number", gameNumber.ToString());
        // && gameNumber == matchState.getCurrentGameState().getLastCompletedRound().roundNumber
        if(matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED) ) {
            GameObject.Find("roundNumberText").GetComponent<Text>().text = LocalizationManager.Instance.Get("match_complete");

        }

        //Evaluate NIL Score. Even in extreme edge case show one NIL. 

        string team1NIL = matchState.nilTeam1MatchScorecard[gameNumber]; 
        bool team1SandBagPenalty = matchState.sandbagTeam1MatchScorecard[gameNumber];
        string team2NIL = matchState.nilTeam2MatchScorecard[gameNumber];
        bool team2SandBagPenalty = matchState.sandbagTeam1MatchScorecard[gameNumber];



        if((team1NIL.Equals("NIL_DONE") || team1NIL.Equals("NIL_BUSTED")) && team1SandBagPenalty) {
            GameObject.Find("ScoreArea1/penalty1").GetComponent<Image>().enabled = true;
            GameObject.Find("ScoreArea1/penalty2").GetComponent<Image>().enabled = true;
            if(team1NIL.Equals("NIL_DONE")) { 
                GameObject.Find("ScoreArea1/penalty1").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");  
            } else {
                GameObject.Find("ScoreArea1/penalty1").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");  
            }
            GameObject.Find("ScoreArea1/penalty2").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/sandbag_penalty");

        } else if(team1NIL.Equals("NA") && team1SandBagPenalty) {
            GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().enabled = true;
            GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/sandbag_penalty");

        } else if(!team1SandBagPenalty && (team1NIL.Equals("NIL_DONE") || team1NIL.Equals("NIL_BUSTED"))) {

            setNILPenalty(matchState, gameNumber, 1);
            /*

            if(!player1Penalty.Equals("NA") && !player2Penalty.Equals("NA")) {

            } else {
                GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().enabled = true;
                if(team1NIL.Equals("NIL_DONE")) { 
                    GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");  
                } else {
                    GameObject.Find("ScoreArea1/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");  
                }

            }*/


        } else {

        }

        if((team2NIL.Equals("NIL_DONE") || team2NIL.Equals("NIL_BUSTED")) && team2SandBagPenalty) {
            GameObject.Find("ScoreArea2/penalty1").GetComponent<Image>().enabled = true;
            GameObject.Find("ScoreArea2/penalty2").GetComponent<Image>().enabled = true;

            if(team2NIL.Equals("NIL_DONE")) { 
                GameObject.Find("ScoreArea2/penalty1").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");  
            } else {
                GameObject.Find("ScoreArea2/penalty1").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");  
            }
            GameObject.Find("ScoreArea2/penalty2").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/sandbag_penalty");

        } else if(team2NIL.Equals("NA") && team2SandBagPenalty) {
            GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().enabled = true;
            GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/sandbag_penalty");

        } else if(!team2SandBagPenalty && (team2NIL.Equals("NIL_DONE") || team2NIL.Equals("NIL_BUSTED"))) {
            setNILPenalty(matchState, gameNumber, 2);
            /*
            GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().enabled = true;
            if(team2NIL.Equals("NIL_DONE")) { 
                GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");  
            } else {
                GameObject.Find("ScoreArea2/penalty").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");  
            }
            */
        } else {
        }




        int team1LastRoundScore = 0,team2LastRoundScore =0;

        Debug.Log("matchState.team1ScoreCard " + PrintUtil.print(matchState.team1ScoreCard));

        if(matchState.getCurrentGameState().gameNumber.Equals(gameNumber)) {
            //Fetch the game score from game

            //team1LastRoundScore = matchState.team1ScoreCard[gameNumber];
            //team2LastRoundScore = matchState.team2ScoreCard[gameNumber];

            team1LastRoundScore = 0;
            team2LastRoundScore = 0;
            team1LastRoundScore = matchState.team1ScoreCard[gameNumber];
            team2LastRoundScore = matchState.team2ScoreCard[gameNumber];

            //team1LastRoundScore = matchState.getCurrentGameState().team1Score;
            //team2LastRoundScore = matchState.getCurrentGameState().team2Score;
        } else {

            team1LastRoundScore = matchState.team1ScoreCard[gameNumber];
            team2LastRoundScore = matchState.team2ScoreCard[gameNumber];

            //team1LastRoundScore = matchState.getCurrentGameState().team1Score;
            //team2LastRoundScore = matchState.getCurrentGameState().team2Score;

        }
       getScoreObj(PlayerPosition.SOUTH).GetComponent<Text>().text = team1LastRoundScore.ToString();
       getScoreObj(PlayerPosition.EAST).GetComponent<Text>().text = team2LastRoundScore.ToString();


        //Now set the match scores

    // 1800118887

        //Now set the cumulative score.

        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().increaseTo(GameObject.Find("matchscore1"), matchState.team1MatchScore, 1f);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().increaseTo(GameObject.Find("matchscore2"), matchState.team2MatchScore, 1f);

        //GameObject.Find("matchscore1").GetComponent<Text>().text = matchState.team1MatchScore.ToString();
        //GameObject.Find("matchscore2").GetComponent<Text>().text = matchState.team2MatchScore.ToString();

       getScoreTotalObj(PlayerPosition.SOUTH).GetComponent<Text>().text = matchState.team1MatchScore.ToString();
       getScoreTotalObj(PlayerPosition.EAST).GetComponent<Text>().text = matchState.team2MatchScore.ToString();



    }

    public Dictionary<PlayerPosition, int> scoreRank(SpadeMatchState matchState) {
        Dictionary<PlayerPosition, int> playerToRankMap = new Dictionary<PlayerPosition, int>();
        List<PlayerPosition> players = new List<PlayerPosition>();    
        PlayerPosition first = highest(matchState.aggregateScoreCardWithLatestRound, players);
        players.Add(first);
        PlayerPosition second = highest(matchState.aggregateScoreCardWithLatestRound, players);
        players.Add(second);
        PlayerPosition third = highest(matchState.aggregateScoreCardWithLatestRound, players);
        players.Add(third);
        PlayerPosition fourth = highest(matchState.aggregateScoreCardWithLatestRound, players);

        playerToRankMap[first] = 1;
        playerToRankMap[second] = 2;
        playerToRankMap[third] = 3;
        playerToRankMap[fourth] = 4;

        return playerToRankMap;


    }

    /*
                Sequence sequence = DOTween.Sequence();
                Vector3 currLoc  = obj.transform.position;
                sequence.Append(obj.transform.DOScale(new Vector3(scaleFactor, scaleFactor, 1f), .15f));
                sequence.Append(obj.transform.DOScale(new Vector3(endScaleFactor, endScaleFactor, 1f), .15f));
                sequence.AppendInterval(.4f);
        */

    void setMatchComplete(SpadeMatchState matchState) {

        int southScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.SOUTH];
        int westScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.WEST];
        int northScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.NORTH];
        int eastScore = matchState.aggregateScoreCardWithLatestRound[PlayerPosition.EAST];

        //TODO: Might need a better UX later.
        disableAllPenalty();


        List<PlayerPosition> players = new List<PlayerPosition>();    
        players.Add(PlayerPosition.NORTH);
        players.Add(PlayerPosition.WEST);
        
        PlayerPosition first = PlayerPosition.SOUTH, second = PlayerPosition.EAST;

        if(matchState.winnerTeam ==1) {
            GameObject.Find("ScoreCardV2/winner").GetComponent<Image>().enabled = true;
            first = PlayerPosition.SOUTH;
            second = PlayerPosition.EAST;

        } else {
            GameObject.Find("ScoreCardV2/winner").GetComponent<Image>().enabled = false;
            first = PlayerPosition.EAST;
            second = PlayerPosition.SOUTH;

        }

        /*
        PlayerPosition third = highest(matchState.aggregateScoreCardWithLatestRound, players);
        players.Add(third);
        PlayerPosition fourth = highest(matchState.aggregateScoreCardWithLatestRound, players);
        */
        Debug.Log(TAG + "1st : " + first + "2nd " + second); 

            
        getPrizeObj(first).GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/firstPrize");
        getPrizeObj(first).transform.localScale = new Vector3(1f,1f,1f);

        getPrizeObj(second).GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/secondPrize");
        getPrizeObj(second).transform.localScale = new Vector3(0.9f,0.9f,1f);

        /*
        getPrizeObj(third).GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/thirdPrize");
        getPrizeObj(third).transform.localScale = new Vector3(0.75f,0.75f,1f);

        getPrizeObj(fourth).GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/fourthPrize");
        getPrizeObj(fourth).transform.localScale = new Vector3(0.6f,0.6f,1f);
        */

    }


    PlayerPosition highest(Dictionary<PlayerPosition, int> scoreMap, List<PlayerPosition> omitted) {
        int highScore = -2000;
        PlayerPosition highestPos = PlayerPosition.SOUTH;

            foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
                if(!omitted.Contains(pos)) {
                    if(scoreMap[pos] > highScore) {
                    highestPos = pos;
                    highScore = scoreMap[pos];

                    }

                }
            }
            return highestPos;            
    }



    GameObject getScoreTotalObj(PlayerPosition pos) {
        //return obj.transform.Find(PlayerPositionHelper.getName(pos).ToLower()).gameObject.transform.Find(PlayerPositionHelper.getName(pos).ToLower()[0] +"total").gameObject;
        return GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "Score").gameObject.transform.Find("totalscore").gameObject;

    }

    GameObject getPrizeObj(PlayerPosition pos) {

        return GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "Score").gameObject.transform.Find("prize").gameObject;

    }
    

    GameObject getScoreObj(PlayerPosition pos) {

        return GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "Score").gameObject.transform.Find("roundscore").gameObject;

    }


    private float getPositionY() {
        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();

        float cardHeight = RectTransformExt.GetWorldRect((RectTransform)cardObjects[0].transform).height;
        float playerSouthCardBoundaryHeight = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthCardBoundary").transform).height;
        float  floatY = GameObject.Find("playerSouthCardBoundary").transform.position.y + playerSouthCardBoundaryHeight * 1.1f;
        return floatY;
 
   }


}
