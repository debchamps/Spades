using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using UnityEngine.Analytics;
using Newtonsoft.Json;
using System;
using TMPro;

using System.Collections;
using System.Collections.Generic;
public class GamePlay : MonoBehaviour {


    public static GameVariant GAME_VARIANT = GameVariant.BRAY;

    public AudioSource playCardAudio;
    public AudioSource roundWonAudio;
    public static string GAME_SAVED_JSON = "OfflineBrayGameSaved";

    public GameObject[] prefabs;

    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    static SettingsManager settingsManager = new SettingsManager();
    public static SpadeMatchState matchState;

    //static Player playerNorth = new RandomPlayer();
    //static Player playerEast = new RandomPlayer();
    //static Player playerWest = new RandomPlayer();

    static Player playerNorth = new CallbreakPlayer();
    static Player playerEast = new CallbreakPlayer();
    static Player playerWest = new CallbreakPlayer();
    static Player playerSouth = new HumanPlayer(PlayerPosition.SOUTH);

    static Sequence warningBkgSequence;
    static Sequence warningTxtSequence;

    public float deltaTime;

    private static string EVENT_PREFIX = "Bray";

    static ConfigManager configManager = new ConfigManager();

    static Dictionary<PlayerPosition, Player> playerPosMap = new Dictionary<PlayerPosition, Player>();


    public static bool IS_DEBUG = false;


    public  void increaseTo(GameObject txtObject, int number, float delay) {
        int diff = number - int.Parse(txtObject.GetComponent<Text>().text);
        if(diff == 0)
            return;
        StartCoroutine(increaseToIEnum(txtObject, number, delay/diff));
        
    }

    public static bool isBiddingHappening() {
        if(matchState != null && matchState.getCurrentGameState() != null && matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.BIDDING))
            return true;
        return false;

    }


    public IEnumerator increaseToIEnum(GameObject txtObject, int number,float delay) {
        Debug.Log("Delay is " + delay);
        yield return new WaitForSeconds(delay);
        if(number > int.Parse(txtObject.GetComponent<Text>().text)) {
            txtObject.GetComponent<Text>().text = (int.Parse(txtObject.GetComponent<Text>().text) + 1).ToString();
            StartCoroutine(increaseToIEnum(txtObject, number, delay));
        }
        else if(number < int.Parse(txtObject.GetComponent<Text>().text)) {
            txtObject.GetComponent<Text>().text = (int.Parse(txtObject.GetComponent<Text>().text) - 1).ToString();
            StartCoroutine(increaseToIEnum(txtObject, number, delay));
        }        

    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            GameObject.Find("ScriptEmpty").GetComponent<QuitGame>().show();
        }

         deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
         float fps = 1.0f / deltaTime;
         GameObject.Find("fps").GetComponent<Text>().text = Mathf.Ceil (fps).ToString() + "aspect : " + DeviceTypeChecker.getAspectRatio().ToString();
     
    }

    public void multiplyLocalScale(GameObject obj, Vector3 scale) {
            Vector3 currScale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(currScale.x * scale.x, currScale.y * scale.y, currScale.z * scale.z);

    }

    bool shouldResumeMatch() {
        //If new version updated. Definately play a new match.
        //If old match, resume.
        //If mismatch in game_variant resume. Only for test though.
        bool shouldResume = false;


        if(!PlayerPrefs.HasKey(GAME_SAVED_JSON))
            return false;
        

        SpadeMatchState savedGame  = null;
        try {
         string savedGameStr = PlayerPrefs.GetString(GAME_SAVED_JSON);
         savedGame = JsonConvert.DeserializeObject<SpadeMatchState>(savedGameStr);
         string currentAppVersion =  Application.version;
         if(savedGame.appVersion == null || !currentAppVersion.Equals(savedGame.appVersion)) {
             return false;
         }
         if(!savedGame.gameVariant.Equals(GAME_VARIANT))
            return false;

        //|| savedGame.getCurrentGameState().getCurrentRound() == null    

        if(savedGame == null || savedGame.getCurrentGameState() == null )
            return false;

        } catch(Exception e) {
            DebugLog.Log("\n Exception " + new System.Diagnostics.StackTrace(e).ToString());
            return false;
        }
        return true;

    }

    


    public IEnumerator resume() {
        yield return new WaitForSeconds(0f);
        infoTextAnimation("Game \nResuming");

        yield return new WaitForSeconds(4f);

        DebugLog.Log("\nResuming");
        //STEP1: Load GameState from PlayerPref.
        string savedGameStr = PlayerPrefs.GetString(GAME_SAVED_JSON);
        Debug.Log("\n Loaded JSON " + savedGameStr);
        SpadeMatchState savedGame  = null;
        try {
         savedGame = JsonConvert.DeserializeObject<SpadeMatchState>(savedGameStr);
        DebugLog.Log("\n Deserialized JSON ");
        } catch(Exception e) {
            DebugLog.Log("\n Exception " + new System.Diagnostics.StackTrace(e).ToString());

        }
       matchState  = savedGame;

        //STEP2: Set the non-serializable fields (player) in gamestate matchstate.
        savedGame.setPlayers(playerPosMap);
        savedGame.getCurrentGameState().setPlayers(playerPosMap);
        DebugLog.Log("\nSet Players");

        //STEP3: Initialize the platers with the card
        playerPosMap[PlayerPosition.SOUTH].initFromGameState(matchState.getCurrentGameState());
        playerPosMap[PlayerPosition.NORTH].initFromGameState(matchState.getCurrentGameState());
        playerPosMap[PlayerPosition.EAST].initFromGameState(matchState.getCurrentGameState());
        playerPosMap[PlayerPosition.WEST].initFromGameState(matchState.getCurrentGameState());

        DebugLog.Log("\n initFromGameState");
        if(matchState.getCurrentGameState() == null) {
        DebugLog.Log("\n initFromGameState2");

        }
        if(matchState.getCurrentGameState().getCurrentRound() == null) {
            DebugLog.Log("\n initFromGameState3");

        }

        //Initializes the scores
        GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().initAllScores();
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().setMatchScore(matchState);

        updateGameScreenOnSettings();

        if(matchState.getCurrentGameState().gameNumber > 1) {
            GameObject.Find("score").GetComponent<Image>().enabled = true;
        } else {
            GameObject.Find("score").GetComponent<Image>().enabled = false;
        }

        if(matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.BIDDING)) {

            processNotification(matchState, SpadeMatchState.NotificationStatus.CARD_DISTRIBUTED);
            matchState.notifyPlayers(SpadeMatchState.NotificationStatus.CARD_DISTRIBUTED);

            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().biddingInfoOnResume(matchState.getCurrentGameState());
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().initGameScore();

            //processNotification(callbreakMatchState, BrayMatchState.NotificationStatus.PASS_CARD);
            //callbreakMatchState.notifyPlayers(BrayMatchState.NotificationStatus.PASS_CARD);

        }   else if(matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.PASS_CARD)) {
            processNotification(matchState, SpadeMatchState.NotificationStatus.PASS_CARD);
            matchState.notifyPlayers(SpadeMatchState.NotificationStatus.PASS_CARD);

        } else {
            if(matchState.getCurrentGameState().getCurrentRound() != null && matchState.getCurrentGameState().getCurrentRound().roundNumber == 1) {
                GameObject.Find("showlasthandbutton").GetComponent<ShowLastHand>().disableButton();
            }

            //STEP4: Notify Player turn if its is SOUTH's turn

            //Call the scoreboard script
            GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().init();



            if(matchState.getCurrentGameState().getCurrentRound()!= null && matchState.getCurrentGameState().getCurrentRound().roundState.Equals(SpadeRound.RoundState.STARTED) &&
                matchState.getCurrentGameState().getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH)) {
                    GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setupAnimationAsync(matchState.getCurrentGameState().getCurrentRound().roundNumber);
            }





            //STEP5: MovePlayedCard script resume.

            GameObject.Find("ScriptEmpty").GetComponent<DeckDistribution>().resume(savedGame);

            GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().resume(savedGame);

            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().initGameScore();

            showNilBid(matchState.getCurrentGameState());

            var nextMovePlayer = matchState.getCurrentGameState().matchStarter;
            if(matchState.getCurrentGameState().getCurrentRound() != null)
                nextMovePlayer = matchState.getCurrentGameState().getCurrentRound().nextMovePlayer;
            DebugLog.Log("nextMovePlayer " + nextMovePlayer);

            //STEP6: Set the other parts.
            bool isGameComplete = matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.COMPLETED);
            if(nextMovePlayer.Equals(PlayerPosition.SOUTH) && !isGameComplete) {

            } else {
                try {
                    Debug.Log("Notifying next player to move" + JsonConvert.SerializeObject(matchState));
                    Debug.Log("LastMatchState " + matchState.getCurrentGameState().gameStatus);

                    if(!isGameComplete) {
                        playerPosMap[nextMovePlayer].updateMatchNotification(matchState, SpadeMatchState.NotificationStatus.MOVE);

                    } else if(isGameComplete) {
                        startNextMatch();  
                    }

                } catch(Exception e) {
                    Debug.Log("Failure in starting match. Restatging again");
                    StartCoroutine(startMatch());
                    
                }
                DebugLog.Log("Start Again with notify move" + nextMovePlayer);
            }

        }


    }

    private void resumeLogic() {

    }

    private void setMatchScore(SpadeMatchState matchState) {
        GameObject.Find("matchscore1").GetComponent<Text>().text = matchState.team1MatchScore.ToString();
        GameObject.Find("matchscore2").GetComponent<Text>().text = matchState.team2MatchScore.ToString();
    }

    public bool canRestartMatch() {
        bool canRestart = false;
        if(matchState !=null && matchState.matchState.Equals(SpadeMatchState.MatchState.IN_PROGRESS)) {
            if(matchState.getCurrentGameState() != null && matchState.getCurrentGameState().getCurrentRound() != null) {
                    canRestart = true;
            }
        } 
        return canRestart;
    }

    public void restartMatch() {
        bool canRestart = canRestartMatch();
        
        if(canRestart)
            StartCoroutine(restartMatchEnumerator());

    }

    public void preFabAction() {
        Debug.Log("In preFabAction. HAHA");
    }

    public void Start() {

        Application.targetFrameRate = 60;

        //All GameObject resolution adjustmentbased on screen size.
        AdjustGameObjectResolution.adjust();

        AnalyticsResult result = Analytics.CustomEvent(EVENT_PREFIX + "GameStart");
        new AchievementManager().initiate();

        GameObject.Find("ScriptEmpty").GetComponent<ScreenWidthAdjustUtil>().initialize();

        clearBid(0f, 0f);

        setPlayers();


        CallbreakAudioInit.Init();
        settingsManager.initiateBraySettings();
        BraySettings settings = settingsManager.getBraySettings();

        bool shouldResume = shouldResumeMatch();
         //shouldResume = false;

        if(!shouldResume) {
            
            matchState = new SpadeMatchState();
            matchState.appVersion = Application.version;

            matchState.setPlayers(playerPosMap);
            matchState.gameTarget = settings.endTarget;
            matchState.isMercyEnabled = settings.isMercyEnabled;
            matchState.isSandBagEnabled = settings.isSandBagEnabled;
            matchState.gameVariant = GAME_VARIANT;

            updateGameScreenOnSettings();


        }




        if (shouldResume) {
            try {
                StartCoroutine(resume());

            } catch(Exception e) {
                Debug.Log("Exception in resuming match");
            }

        } else {
            StartCoroutine(startMatch());

        }
        GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().initiate();
        GameObject.Find("settingsparent").transform.localScale = new Vector3(0f, 0f, 1f);


    }



    private void setPlayers() {
        playerNorth.playerPosition = PlayerPosition.NORTH;
        playerEast.playerPosition = PlayerPosition.EAST;
        playerWest.playerPosition = PlayerPosition.WEST;
        playerSouth.playerPosition = PlayerPosition.SOUTH;

        playerPosMap[PlayerPosition.EAST] =  playerEast;
        playerPosMap[PlayerPosition.WEST] = playerWest;
        playerPosMap[PlayerPosition.NORTH] = playerNorth;
        playerPosMap[PlayerPosition.SOUTH] = playerSouth;
    }

    public void clearBid(float initialDelay, float duration) {

        GameObject.Find("eastbubble").GetComponent<Image>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("westbubble").GetComponent<Image>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("northbubble").GetComponent<Image>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("southbubble").GetComponent<Image>().DOFade(0f, duration).SetDelay(initialDelay);

        GameObject.Find("westbidval").GetComponent<Text>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("eastbidval").GetComponent<Text>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("northbidval").GetComponent<Text>().DOFade(0f, duration).SetDelay(initialDelay);
        GameObject.Find("southbidval").GetComponent<Text>().DOFade(0f, duration).SetDelay(initialDelay);



    }

    public void scaleUpAndDown(GameObject obj, float time, float initialDelay, float scaleFactor) {

        Vector3 currScale = obj.transform.localScale;

        

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(initialDelay);
        sequence.Append(obj.transform.DOScale(currScale.x * 1.7f, time/2));
        sequence.Append(obj.transform.DOScale(currScale, time/2));
        sequence.Play();

    }

    public void setTextWithDelay(GameObject obj, string txt, float delay) {
        StartCoroutine(setTextWithDelayEnumerator(obj, txt, delay));
        
    }

    public void playAudioWithDelay(string clip, float delay) {
        StartCoroutine(AudioManagerScript.playWithDelay(clip, delay));
        
    }


    public  static IEnumerator setTextWithDelayEnumerator(GameObject obj, string txt, float delay) {
        yield return new WaitForSeconds(delay);
        obj.GetComponent<Text>().text = txt;

    }


    void stopPlayerWarningAnimation()
    {

        if (warningBkgSequence != null)
        {
            GameObject.Find("turnWarning").GetComponent<Image>().DOFade(0f, 0f);
            warningBkgSequence.Kill();
        }
        if (warningTxtSequence != null)
        {

            GameObject.Find("warningtxt").GetComponent<Text>().DOFade(0.0f, 0f);
            warningTxtSequence.Kill();
        }
    }


    public IEnumerator setupAnimationAsyncEnumerator(int roundNum) {
        stopPlayerWarningAnimation();

        yield return new WaitForSeconds(5f);

        if(matchState != null && matchState.getCurrentGameState() != null && matchState.getCurrentGameState().getCurrentRound()!= null) {
            if(roundNum == GamePlay.matchState.getCurrentGameState().getCurrentRound().roundNumber
            &&
            GamePlay.matchState.getCurrentGameState().getCurrentRound().roundState.Equals(SpadeRound.RoundState.STARTED) 
            && GamePlay.matchState.getCurrentGameState().getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH)
            ) {
            warningTxtSequence = DOTween.Sequence();
            warningBkgSequence = DOTween.Sequence();

            for (int i = 0; i < 1; i++)
            {

                float time = .7f;
                warningBkgSequence.Append(GameObject.Find("turnWarning").GetComponent<Image>().DOFade(0.2f, time));
                warningBkgSequence.Append(GameObject.Find("turnWarning").GetComponent<Image>().DOFade(0.0f, time));

                warningTxtSequence.Append(GameObject.Find("warningtxt").GetComponent<Text>().DOFade(1.0f, time));
                warningTxtSequence.Append(GameObject.Find("warningtxt").GetComponent<Text>().DOFade(0.0f, time));
            }
            
            warningBkgSequence.SetLoops(100,LoopType.Restart);
            warningTxtSequence.SetLoops(100,LoopType.Restart);
            warningBkgSequence.Play();
            warningTxtSequence.Play();

        }

        }

    }

    public  void setupAnimationAsync(int roundNum) {
        StartCoroutine(setupAnimationAsyncEnumerator(roundNum));
    }

    public static void setupAnimation()
    {
        if (warningBkgSequence != null)
            warningBkgSequence.Kill();
        if (warningTxtSequence != null)
            warningTxtSequence.Kill();

        //if(warningBkgSequence  == null) 
        {
            warningTxtSequence = DOTween.Sequence();
            warningBkgSequence = DOTween.Sequence();
            warningBkgSequence.AppendInterval(5f);
            warningTxtSequence.AppendInterval(5f);

            for (int i = 0; i < 1; i++)
            {

                float time = .7f;
                //wanrningTextAnimation.Append(GameObject.Find("turnWarning").GetComponent<CanvasGroup>().DOFade(1.0f, time));
                //wanrningTextAnimation.Append(GameObject.Find("turnWarning").GetComponent<CanvasGroup>().DOFade(0.0f, time));
                warningBkgSequence.Append(GameObject.Find("turnWarning").GetComponent<Image>().DOFade(0.2f, time));
                warningBkgSequence.Append(GameObject.Find("turnWarning").GetComponent<Image>().DOFade(0.0f, time));

                warningTxtSequence.Append(GameObject.Find("warningtxt").GetComponent<Text>().DOFade(1.0f, time));
                warningTxtSequence.Append(GameObject.Find("warningtxt").GetComponent<Text>().DOFade(0.0f, time));
            }

        }

        warningBkgSequence.SetLoops(100,LoopType.Restart);
        warningTxtSequence.SetLoops(100,LoopType.Restart);
        warningBkgSequence.Play();
        warningTxtSequence.Play();



    }

    public static void clearAnimation() {
        warningBkgSequence.Kill();
        warningTxtSequence.Kill();
        clearYourTurn();

    }


    public void clearAnimationAsync() {
        warningBkgSequence.Kill();
        warningTxtSequence.Kill();
        clearYourTurn();

    }

    public static void clearYourTurn() {
        Color txtColor = GameObject.Find("warningtxt").GetComponent<Text>().color;
        txtColor.a = 0;
        GameObject.Find("warningtxt").GetComponent<Text>().color = txtColor;

        Color imgColor = GameObject.Find("turnWarning").GetComponent<Image>().color;
        imgColor.a = 0;
        GameObject.Find("turnWarning").GetComponent<Image>().color = imgColor;

    }

    public static void  startNextMatchStatic() {
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().startNextMatch();

    }

    public void startNextMatch() {
        StartCoroutine(startNextMatchIEnumerator());

    }

    IEnumerator  startNextMatchIEnumerator() {
        //infoTextAnimation("Game Starts");
        yield return new WaitForSeconds(1f);
        StartCoroutine(startMatch());
    }


    public static void startNewMatch() {


        Debug.Log("Starting new match");

        GameObject.Find("score").GetComponent<Image>().enabled = false;

        playerNorth.playerPosition = PlayerPosition.NORTH;
        playerEast.playerPosition = PlayerPosition.EAST;
        playerWest.playerPosition = PlayerPosition.WEST;
        playerSouth.playerPosition = PlayerPosition.SOUTH;

        playerPosMap[PlayerPosition.EAST] =  playerEast;
        playerPosMap[PlayerPosition.WEST] = playerWest;
        playerPosMap[PlayerPosition.NORTH] = playerNorth;
        playerPosMap[PlayerPosition.SOUTH] = playerSouth;

        matchState = new SpadeMatchState();
        matchState.appVersion = Application.version;

        matchState.setPlayers(playerPosMap);
        matchState.gameVariant = GAME_VARIANT;
        BraySettings settings = settingsManager.getBraySettings();
        matchState.gameTarget = settings.endTarget;
        matchState.isMercyEnabled = settings.isMercyEnabled;
        matchState.isSandBagEnabled = settings.isSandBagEnabled;

        Debug.Log("Match Target is " + matchState.gameTarget);

        matchState.startGame();


        if(matchState.getCurrentGameState().getCurrentRound() != null && matchState.getCurrentGameState().getCurrentRound().roundNumber == 1) {
            GameObject.Find("showlasthandbutton").GetComponent<ShowLastHand>().disableButton();
        }

        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().setMatchScore(matchState);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().initGameScore();

        updateGameScreenOnSettings();

        resetNilBid();

        

        

    }

    private static void updateGameScreenOnSettings() {
        if(!matchState.isSandBagEnabled) {
            GameObject.Find("sandbag1").GetComponent<Text>().text = "";                 
            GameObject.Find("sandbag2").GetComponent<Text>().text = "";                 
        } else {
            GameObject.Find("sandbag1").GetComponent<Text>().text = matchState.cumulativeSandbagTeam1.ToString();                 
            GameObject.Find("sandbag2").GetComponent<Text>().text = matchState.cumulativeSandbagTeam2.ToString();                 

        }

         GameObject.Find("endscore").GetComponent<Text>().text = matchState.gameTarget.ToString();

    }

    private IEnumerator  restartMatchEnumerator() {

        //Stop the current match. No more move.
        matchState.stop();
        yield return new WaitForSeconds(.5f);
         
         //WarningEntity entity = new WarningEntity("Starting a New Bray Match", "");
         //GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(entity);

        yield return new WaitForSeconds(.6f);


        infoTextAnimation("Starting \nNew Match");


        //Few cleanup needs to be done. Score and Move played card.
        matchState = null;
        PlayerPrefs.SetString(GAME_SAVED_JSON, null);
        PlayerPrefs.Save();  
       
        GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().restart();
        stopPlayerWarningAnimation();

        startNewMatch();
 

    }

    public IEnumerator  startMatch() {

        yield return new WaitForSeconds(0.05f);

        if(matchState == null || matchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED)) {
            startNewMatch();
        } else {
            matchState.startGame();
        }

        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().initGameScore();
        GamePlay.resetNilBid();

    }

    public static void processNotification(SpadeMatchState callbreakMatchState, SpadeMatchState.NotificationStatus notificationStatus) {

        Debug.Log("Processing notification " + notificationStatus);

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BID_START)) {
            updateBid(callbreakMatchState.getCurrentGameState());
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BID)) {
            updateBid(callbreakMatchState.getCurrentGameState());
        }
        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.BIDDING_COMPLETE)) {

            updateBidComplete(callbreakMatchState.getCurrentGameState());


        }
        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.PASS_CARD)) {
            /*
            distribute(callbreakMatchState);
            updatePassCard(callbreakMatchState.getCurrentGameState());
            */
            //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().disablePlayerInvalidCards(callbreakMatchState);

        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.CARD_DISTRIBUTED)) {
            distribute(callbreakMatchState);
            updateBidStart(callbreakMatchState.getCurrentGameState());

        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.GAME_COMPLETE)) {
            updateGameComplete(callbreakMatchState);
            Analytics.CustomEvent(EVENT_PREFIX + "GameComplete");
            //updateBiddingComplete(callbreakMatchState.getCurrentGameState());
        }
        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.MATCH_COMPLETE)) {
            updateGameComplete(callbreakMatchState);
            Analytics.CustomEvent(EVENT_PREFIX + "MatchComplete");

            //updateBiddingComplete(callbreakMatchState.getCurrentGameState());
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.MOVE)) {
            updateMove(callbreakMatchState);
        }

        if(notificationStatus.Equals(SpadeMatchState.NotificationStatus.ROUND_COMPLETE)) {
            updateRoundComplete(callbreakMatchState);
            Analytics.CustomEvent(EVENT_PREFIX + "RoundComplete");
        }

        if(callbreakMatchState != null) {
        string currentGameStateStr = JsonConvert.SerializeObject(callbreakMatchState);
        PlayerPrefs.SetString(GAME_SAVED_JSON, currentGameStateStr);
         PlayerPrefs.Save();  
        //string currentGameStateStr = JsonUtility.ToJson(callbreakMatchState);
        //Debug.Log("Serialized string is " + currentGameStateStr);            
        }


        
    }

    public static void distribute(SpadeMatchState callbreakMatchState) {
        GameObject.Find("ScriptEmpty").GetComponent<DeckDistribution>().distribute(callbreakMatchState.getCurrentGameState());
    }


    public static void setScore(SpadeMatchState callbreakMatchState) {

        PlayerPosition winnerPosition = callbreakMatchState.getCurrentGameState().getLastCompletedRound().winner;


        //Get winning team.

        if(winnerPosition.Equals(PlayerPosition.SOUTH) || winnerPosition.Equals(PlayerPosition.NORTH)) {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(GameObject.Find("team1score/scoretxt") , .6f, 1.5f, 1.5f);
           GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team1score/scoretxt")  , getScoreString(callbreakMatchState,winnerPosition), 1.3f);

        } else {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(GameObject.Find("team2score/scoretxt")  , .6f, 1.5f, 1.5f);
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team2score/scoretxt")  , getScoreString(callbreakMatchState,winnerPosition), 1.3f);
        }

        //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(GameObjectFinder.getPlayerScoreObject(winnerPosition) , .6f, 1.5f, 1.5f);
        //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObjectFinder.getPlayerScoreObject(winnerPosition) , getScoreString(callbreakMatchState,winnerPosition), 1.3f);

    }


    static string getAggregateScoreString(SpadeMatchState callbreakMatchState, PlayerPosition playerPosition){

        return callbreakMatchState.aggregateScoreCardWithLatestRound[playerPosition].ToString();
        //+ " / " + callbreakMatchState.getCurrentGameState().callBreakBidding.callBreakBiddingData.getBidAmount(playerPosition).ToString();

    }

    public void initGameScore() {

        Debug.Log("In initGameScore with status: " + matchState.getCurrentGameState().gameStatus);
        if(matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.BIDDING)) {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team1score/scoretxt")  , "Bidding", 0f);
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team2score/scoretxt")  , "Bidding", 0f);

        } else {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team1score/scoretxt")  , getScoreString(matchState,PlayerPosition.SOUTH), 0.2f);
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team2score/scoretxt")  , getScoreString(matchState,PlayerPosition.WEST), 0.2f);
        }

        //Also init showing NIL.
        
    }

    static string getScoreString(SpadeMatchState callbreakMatchState, PlayerPosition playerPosition){
        
        var gameState = callbreakMatchState.getCurrentGameState();

        
        if(playerPosition.Equals(PlayerPosition.SOUTH) || playerPosition.Equals(PlayerPosition.NORTH)) {

            return gameState.getAchieved(1) + " of " + gameState.getTarget(1);

            //return callbreakMatchState.getCurrentGameState().team1Score.ToString();


        } else {
            return gameState.getAchieved(2) + " of " + gameState.getTarget(2);

            //return callbreakMatchState.getCurrentGameState().team2Score.ToString();
        }

        //+ " / " + callbreakMatchState.getCurrentGameState().callBreakBidding.callBreakBiddingData.getBidAmount(playerPosition).ToString();

    }

    public static void updateRoundComplete(SpadeMatchState callbreakMatchState) {

        clearAnimation();


        if(callbreakMatchState.getCurrentGameState().getLastCompletedRound().roundNumber >= 1) {
            GameObject.Find("showlasthandbutton").GetComponent<ShowLastHand>().enableButton();
        }


        //Move the card to played card after a delay.

        //1. update the move of the last round.
        //System.Threading.Thread.Sleep(configManager.callbreakTimingConfig.delayBetweenMoves);

        updateMove(callbreakMatchState);

        Debug.Log("Movign cards to player");
        float height = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthSeat").transform).height;

        //moveUpAndDown(GameObject.Find("playerSouthSeat"),1f, 4, 1f, height/8);


        GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().moveCardToWinner(callbreakMatchState);

        setScore(callbreakMatchState);

        //2. move all the played card to winner hand.

    }

    public static void infoTextAnimation(string content) {
        GameObject obj = GameObject.Find("infoText");
        obj.GetComponent<Text>().text = content;
        Sequence sequence = DOTween.Sequence();
        Vector3 currLoc  = obj.transform.position;
        float scaleFactor = 1.5f;
        sequence.AppendInterval(1.2f);           
        sequence.Append(obj.transform.DOScale(new Vector3(scaleFactor, scaleFactor, 1f), .2f));
        sequence.Append(obj.transform.DOScale(new Vector3(1f, 1f, 1f), .15f));
        sequence.AppendInterval(1.5f);           
        sequence.Append(obj.transform.DOScale(new Vector3(0f, 0f, 1f), .25f));
        sequence.Play();
    }

    public static void updateMatchComplete(SpadeMatchState callbreakMatchState)
    {
        PlayerPrefs.SetString(GAME_SAVED_JSON, null);
        PlayerPrefs.Save();
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().enableScoreCardWithDelay(callbreakMatchState, false);
        if (callbreakMatchState.winnerTeam == 1)
            GameObject.Find("ratingparent").GetComponent<RatingManager>().increment();

    }

    public static void updateGameComplete(SpadeMatchState callbreakMatchState) {
        clearAnimation();

        GameObject.Find("score").GetComponent<Image>().enabled = true;

        updateRoundComplete(callbreakMatchState);

        if(callbreakMatchState.matchState.Equals(SpadeMatchState.MatchState.COMPLETED)){
            updateMatchComplete(callbreakMatchState);

        }
        else {
            GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().enableScoreCardAndDisappear(callbreakMatchState);
            //infoTextAnimation("Round 2");
        }


        List<PlayerPosition> nilAchievedPlayers = callbreakMatchState.getCurrentGameState().getNilAchievedPlayers();
        if(nilAchievedPlayers != null && nilAchievedPlayers.Count  > 0) {
                GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().noTrickOrShootingMoonAnimation(callbreakMatchState, nilAchievedPlayers);

        }


        /*
        if(callbreakMatchState.gameVariant.Equals(GameVariant.BRAY)) {
            List<PlayerPosition> zeroTricksPlayers = callbreakMatchState.getCurrentGameState().getZeroTrickWonPlayers();
            if(zeroTricksPlayers != null && zeroTricksPlayers.Count > 0)
                GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().noTrickOrShootingMoonAnimation(callbreakMatchState, zeroTricksPlayers);
        }
        else if(callbreakMatchState.gameVariant.Equals(GameVariant.HEARTS)) {
            List<PlayerPosition> moonShotPlayer = callbreakMatchState.getCurrentGameState().findShootMoonPlayer();
            if(moonShotPlayer != null && moonShotPlayer.Count  > 0) {
                GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().noTrickOrShootingMoonAnimation(callbreakMatchState, moonShotPlayer);
            }

        }*/



        Debug.Log("Game is complete. Enabling all cards");


        //infoTextAnimation("Excellent Round");

        //GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().enableAllCards();

        Debug.Log("Game is complete. Card enabled");


        //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().startNextMatch();



        //Move the card to played card after a delay.

        //1. update Round Complete.

        //2. move all the played card to winner hand.


    }




    public static void updateMove(SpadeMatchState callbreakMatchState) {


        //GameObject.Find("biddingparent").transform.position = new Vector3(Screen.width/2, Screen.height/2, 1f);



        SpadeGameState gameState = callbreakMatchState.getCurrentGameState();
        //1.Move the card to played card after a delay.

        // find gameObject with the corresponding card.
        enableAllCards(callbreakMatchState);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().disablePlayerInvalidCards(callbreakMatchState);

        //disablePlayerInvalidCards(callbreakMatchState);
        SpadeMove callbreakMove = null;
        SpadeMove winningMove = null;


        if (gameState.getCurrentRound().moveNumber == 0 && gameState.getCurrentRound().roundNumber>=2) {
            callbreakMove = gameState.rounds[gameState.rounds.Count -2].moves[3];
            winningMove = gameState.rounds[gameState.rounds.Count -2].findWinningMove();
            GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().deactivatePreviousLosingCard(gameState.rounds[gameState.rounds.Count -2]);
        } else {
          callbreakMove = gameState.getCurrentRound().getCurrentMove();
          winningMove = gameState.getCurrentRound().findWinningMove();
          GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().deactivatePreviousLosingCard(gameState.getCurrentRound());

        }
        Debug.Log("winningMove " + winningMove.card.getCardId() + " Played Move " + callbreakMove.card.getCardId());
            //GameObject cobj = GameObjectFinder.findCardGameObject(callbreakMove.card);
            //cobj.GetComponent<CardScript>().inactivate();   

        //if(callbreakMove.playerPosition.Equals(PlayerPosition.SOUTH))


        if(callbreakMove.autoPlay) {
            GameObject cobj = GameObjectFinder.findCardGameObject(callbreakMove.card);
            cobj.GetComponent<CardScript>().autoThrow();

        }

        GameObject.Find("ScriptEmpty").GetComponent<MovePlayedCard>().playCard(callbreakMove);

        if(gameState.getCurrentRound().roundState.Equals(SpadeRound.RoundState.STARTED) && gameState.getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH))
        {

            if (gameState.getCurrentRound().roundNumber == 1)
            {
                if (GameObject.Find("ratingparent").GetComponent<RatingManager>().shouldOpen())
                {

                    GameObject.Find("ratingparent").GetComponent<RatingManager>().open();
                }

            }
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setupAnimationAsync(gameState.getCurrentRound().roundNumber);
        }
        else
        {
             GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().clearAnimationAsync();

        }

        if(gameState!= null && gameState.isSpadeBroken() && gameState.getSpadesBrokenCard().getCardId().Equals(callbreakMove.card.getCardId())) {
                GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(new WarningEntity("<sprite=3> Spades broken by : " + callbreakMove.playerPosition, "C"));

        }

        

    }

    public static void resetNilBid() {
        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
                GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "nilBubble").transform.localScale = new Vector3(0f,0f,1f);
        }

    }

    public void showNilBid(SpadeGameState gameState) {


        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            if(gameState.isNil(pos)) {
                if(gameState.isPlayerNILBusted(pos)) {
                    GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "nilBubble").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");

                } else {
                    GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "nilBubble").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_green");
                }
                GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "nilBubble").transform.localScale = new Vector3(1f,1f,1f);

            }
        }
        //isNIL BID



        //GameObject.Find("playerNorthSeat/nilBubble").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_blue");
    }

    public static void updateBidComplete(SpadeGameState gameState) {
        //update all bid.    
        updateBid(gameState);
        GameObject.Find("showlasthandbutton").GetComponent<ShowLastHand>().disableButton();
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team1score/scoretxt")  , getScoreString(matchState,PlayerPosition.SOUTH), 2.3f);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("team2score/scoretxt")  , getScoreString(matchState,PlayerPosition.WEST), 2.3f);

        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GetGameObject(pos,  "score") , getScoreString(matchState,pos), 2.3f);
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GetGameObject(pos,  "totalscore") , getAggregateScoreString(matchState,pos), 2.3f);

        }


        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().clearBid(3f, .5f);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().showNilBid(gameState);

        //GameObject.Find("ScriptEmpty").GetComponent<BidButtonGo>().disableBidding();

        //Debug.Log("gameState.getCurrentRound().roundNumber" + gameState.getCurrentRound().roundNumber + " gameState.getCurrentRound().nextMovePlayer " + gameState.getCurrentRound().nextMovePlayer);
        //Debug.Log("gameState.getCurrentRound().roundNumber" + gameState.getCurrentRound().roundNumber + " gameState.getCurrentRound().nextMovePlayer " + gameState.getCurrentRound().nextMovePlayer);
        if(gameState.getCurrentRound().roundState.Equals(SpadeRound.RoundState.STARTED) && gameState.getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH))
        {

            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setupAnimationAsync(gameState.getCurrentRound().roundNumber);

        }

        if(gameState.gameVariant.Equals(GameVariant.HEARTS) && gameState.isNoPassingRound()) {
            GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(new WarningEntity("NO Passing in this Round", "C"));

        }

        //Reset score order.
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().init();

    }

    /*

    public static void updatePassCard(SpadeGameState gameState) {
        //update all bid.    
        //updateBid(gameState);
        GameObject.Find("showlasthandbutton").GetComponent<ShowLastHand>().disableButton();
        GameObject.Find("passcardparent/cardCombo1/bkg").transform.DOScale(GameObject.Find("testCard1").transform.localScale, 0f);

        GameObject.Find("ScriptEmpty").GetComponent<PassBoxScript>().show();

        //GameObject.Find("passcardparent").transform.position = new Vector3(Screen.width/2, Screen.height/2, 1f);


    }
    */

    public static void updateBidStart(SpadeGameState gameState) {

        GameObject.Find("playernorthscore").GetComponent<Text>().text = "";
        GameObject.Find("playersouthscore").GetComponent<Text>().text = "";
        GameObject.Find("playerwestscore").GetComponent<Text>().text = "";
        GameObject.Find("playereastscore").GetComponent<Text>().text = "";
        
        
        if(gameState.callBreakBidding.callBreakBiddingData.nextBiddingPosition.Equals(PlayerPosition.SOUTH)) {
            Debug.Log("XXX Enablig bidding " );
            GameObject.Find("ScriptEmpty").GetComponent<BidButtonGo>().enableBidding();
        }
        

    }

    private void biddingInfoOnResume(SpadeGameState gameState) {
        Debug.Log(" In biddingInfoOnResume");

        CallBreakBiddingData biddingData = gameState.callBreakBidding.callBreakBiddingData;

        foreach(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {

            if(biddingData.playerBidAmount.ContainsKey(pos)) {
                GameObject bidBubbleObj = GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "bubble");        
                GameObject bidTextObj = GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "bidval");

                int bid = biddingData.getBidAmount(pos);        
                bidTextObj.GetComponent<Text>().text = "BID " + bid.ToString();
                float initialDelay = 0.6f;
                GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(bidTextObj , .3f, 0f, 1.5f);

                Sequence sequence=  DOTween.Sequence();

                sequence.AppendInterval(initialDelay);
                sequence.Append(bidBubbleObj.GetComponent<Image>().DOFade(1.0f, 0));
                sequence.Append(bidTextObj.GetComponent<Text>().DOFade(1.0f, 0));
                sequence.Append(bidTextObj.GetComponent<Text>().transform.DOScale(1.4f, .25f));
                sequence.Append(bidTextObj.GetComponent<Text>().transform.DOScale(1f, .25f));

                sequence.Play();

            }

        }

    }

    public void bubbleAnimationWithDelay(SpadeGameState gameState) {
        CallBreakBiddingData biddingData = gameState.callBreakBidding.callBreakBiddingData;
        GameObject bidBubbleObj = GameObject.Find(PlayerPositionHelper.getName(biddingData.currentBiddingPosition).ToLower() + "bubble");        
        GameObject bidTextObj = GameObject.Find(PlayerPositionHelper.getName(biddingData.currentBiddingPosition).ToLower() + "bidval");

        int bid = biddingData.getBidAmount(biddingData.currentBiddingPosition);        
        bidTextObj.GetComponent<Text>().text = "BID " + bid.ToString();

        //bidBubbleObj.GetComponent<Image>().enabled = true;
        //bidTextObj.GetComponent<Text>().enabled = true;

        float initialDelay = 0.6f;

        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(bidTextObj , .3f, initialDelay, 1.5f);


        Sequence sequence=  DOTween.Sequence();

        sequence.AppendInterval(initialDelay);
        sequence.Append(bidBubbleObj.GetComponent<Image>().DOFade(1.0f, 0));
        sequence.Append(bidTextObj.GetComponent<Text>().DOFade(1.0f, 0));
        sequence.Append(bidTextObj.GetComponent<Text>().transform.DOScale(1.4f, .25f));
        sequence.Append(bidTextObj.GetComponent<Text>().transform.DOScale(1f, .25f));

        sequence.Play();

    }

    public static void updateBid(SpadeGameState gameState) {
        CallBreakBiddingData biddingData = gameState.callBreakBidding.callBreakBiddingData;
        Debug.Log("XXX bidding position is " + gameState.callBreakBidding.callBreakBiddingData.currentBiddingPosition);
        int bid = biddingData.getBidAmount(biddingData.currentBiddingPosition);        
        
        //GetGameObject(gameState.callBreakBidding.callBreakBiddingData.currentBiddingPosition, "score").GetComponent<UnityEngine.UI.Text>().text = bid.ToString();

        GameObject bidObj = GetGameObject(biddingData.currentBiddingPosition, "score");

        float waitBeforeAnimation = configManager.callbreakTimingConfig.waitBeforeBiddingAnimation;

        if(biddingData.currentBiddingPosition.Equals(PlayerPosition.SOUTH)) {
            waitBeforeAnimation = waitBeforeAnimation/3;
        }

        


        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().bubbleAnimationWithDelay(gameState);

        

        //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(bidObj , .6f, waitBeforeAnimation, 1.5f);
        //GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(bidObj , bid.ToString(), waitBeforeAnimation);
        
        if(!biddingData.currentBiddingPosition.Equals(PlayerPosition.SOUTH)) {
            GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().playAudioWithDelay(AudioClipType.DEFAULT_NOTIFICATION, configManager.callbreakTimingConfig.waitBeforeBiddingAnimation);
        }

        if(biddingData.currentBiddingPosition.Equals(PlayerPosition.SOUTH)) {
            GameObject.Find("ScriptEmpty").GetComponent<BidButtonGo>().disableBidding();
        }

        if(biddingData.status.Equals(CallBreakBiddingData.Status.IN_PROGRESS) && biddingData.nextBiddingPosition.Equals(PlayerPosition.SOUTH)) {
            Debug.Log("XXX Enablig bidding " );
            GameObject.Find("ScriptEmpty").GetComponent<BidButtonGo>().enableBidding();
        }

    }

    public static void highlightCard(Card card) {
        List<GameObject> gameObjects = GameObjectFinder.getCardObjects();
        foreach(GameObject obj in gameObjects) {
            if(obj.GetComponent<CardScript>().card.getCardId().Equals(card.getCardId())) {
                obj.GetComponent<CardScript>().highlight();
            }
        }

    }

    public static GameObject GetGameObject(PlayerPosition playerPosition, string objectType) {
        string playerPosObjName = "player" + PlayerPositionHelper.getName(playerPosition).ToLower() + objectType;
        Debug.Log("playerPosObjName " + playerPosObjName);
        return GameObject.Find(playerPosObjName);
    }

    public static void enableAllCards(SpadeMatchState matchState) {
        List<GameObject> gameObjects = GameObjectFinder.getCardObjects();
        bool shouldNotActivateHearts = false;
        if(matchState.gameVariant.Equals(GameVariant.HEARTS)) {
            shouldNotActivateHearts = !matchState.getCurrentGameState().isHeartsBroken();
        }
        foreach(GameObject gameObject in gameObjects) {
            if(shouldNotActivateHearts && gameObject.GetComponent<CardScript>().card.suit.Equals("H")) {

            } else {
                gameObject.GetComponent<CardScript>().activate();
            }
        }

    }

    public void disablePlayerInvalidCards(SpadeMatchState callbreakMatchState) {
            StartCoroutine(disablePlayerInvalidCardsEnumerator(callbreakMatchState));

    }

    public IEnumerator disablePlayerInvalidCardsEnumerator(SpadeMatchState callbreakMatchState) {



        //Debug.Log("disablePlayerInvalidCards Start");
        SpadeRound callbreakRound = callbreakMatchState.getCurrentGameState().getCurrentRound();
        if(callbreakRound.moves.Count >=0 && callbreakRound.nextMovePlayer.Equals(PlayerPosition.SOUTH)) {
            //Debug.Log("disablePlayerInvalidCards Criteria satisfied");
            List<Card> validCards = new BrayCardHelper().validCards(callbreakMatchState.getCurrentGameState(), PlayerPosition.SOUTH);
            List<Card> remainingCards = callbreakMatchState.playerMap[PlayerPosition.SOUTH].remainingCards;

            List<GameObject> gameObjects = GameObjectFinder.getCardObjects();
            //Debug.Log("disablePlayerInvalidCards validCards count " + validCards.Count + " remainingCards count" + remainingCards.Count);

            //Debug.Log("disablePlayerInvalidCards gameObjects count " + gameObjects.Count);

            //Get all the gameObjects.
            foreach(GameObject gameObject in gameObjects) {
                Card cardInObject = gameObject.GetComponent<CardScript>().card;
                Debug.Log("Card in object is " + cardInObject.getCardId());
                bool isCardRemaining = new CardHelper().contains(remainingCards, cardInObject);
                bool isCardValid = new CardHelper().contains(validCards, cardInObject);
                //Debug.Log("disablePlayerInvalidCards Card " + cardInObject.getCardId() + " isCardValid " + isCardValid + ",isCardRemaining "  + isCardRemaining);

                if(!isCardValid && isCardRemaining) {
                    //Debug.Log("disablePlayerInvalidCards inactivating");

                    gameObject.GetComponent<CardScript>().inactivate();
                }
                if(isCardValid && isCardRemaining) {
                    //Debug.Log("disablePlayerInvalidCards inactivating");

                    gameObject.GetComponent<CardScript>().activate();
                }

            }

        }
        yield return new WaitForSeconds(0f);


    }

}
