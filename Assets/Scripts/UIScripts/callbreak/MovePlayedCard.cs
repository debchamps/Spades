using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

public class MovePlayedCard : MonoBehaviour
{

    Camera camera;
    float moveTime = 0.3f;

    Vector3 OUT_OF_SCREEN_POS = new Vector3(5 * Screen.width, 0f, 0f);

    public static Dictionary<PlayerPosition, Vector3> playerCardStartingPositionMap = new Dictionary<PlayerPosition, Vector3>();
    static Dictionary<PlayerPosition, Vector3> playerCardStaringFinalPositionMap = new Dictionary<PlayerPosition, Vector3>();

    ConfigManager configManager = new ConfigManager();
    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();


    void initiate()
    {

        camera = GameObject.Find("CameraMain").GetComponent<Camera>();
        playerCardStartingPositionMap[PlayerPosition.EAST] = new Vector3(4.0f, 2.7f, -10f);
        playerCardStartingPositionMap[PlayerPosition.WEST] = new Vector3(-4.0f, 2.7f, -10f);
        playerCardStartingPositionMap[PlayerPosition.SOUTH] = new Vector3(0f, -1.3f, -10f);
        playerCardStartingPositionMap[PlayerPosition.NORTH] = new Vector3(0.0f, 6.8f, -10f);

        playerCardStaringFinalPositionMap[PlayerPosition.EAST] = new Vector3(1.4f, 4.4f, -10f);
        playerCardStaringFinalPositionMap[PlayerPosition.NORTH] = new Vector3(-0.1f, 5.0f, -10f);
        playerCardStaringFinalPositionMap[PlayerPosition.WEST] = new Vector3(-1.6f, 4.4f, -10f);

    }

    void Start()
    {
        initiate();

    }

    void Resume()
    {
        initiate();

    }

    public void restart()
    {
        GameObject.Find("playernorthcard").transform.position = new Vector2(Screen.width * 5, 0);
        GameObject.Find("playereastcard").transform.position = new Vector2(Screen.width * 5, 0);
        GameObject.Find("playerwestcard").transform.position = new Vector2(Screen.width * 5, 0);

    }

    public void resume(SpadeMatchState currMatchState)
    {

        SpadeRound currRound = currMatchState.getCurrentGameState().getCurrentRound();
        var currRoundJson = JsonConvert.SerializeObject(currRound);

        Debug.Log("CurrentRoundJson" + currRoundJson);
        //Move current currRound cards to position.
        if (currRound != null)
        {
            if (currRound.moves.Count > 0 && currRound.moves.Count <= 3)
            {
                foreach (SpadeMove move in currRound.moves)
                {
                    if (move.playerPosition.Equals(PlayerPosition.SOUTH))
                    {
                        GameObjectFinder.findCardGameObject(currRound.roundCards[PlayerPosition.SOUTH]).transform.DOMove(playPosition(move.playerPosition), 0f);

                    }
                    else
                    {
                        attachSpriteForCardObject(move);
                        getOtherPlayerCardGameObject(move.playerPosition).transform.DOMove(playPosition(move.playerPosition), 0f);

                    }

                }

            }

        }


        //Move played card to OOScreen position.
        foreach (SpadeRound round in currMatchState.getCurrentGameState().rounds)
        {
            //Debug.Log("Round status " + JsonConvert.SerializeObject(round));
            if (round.roundState.Equals(SpadeRound.RoundState.COMPLETED))
            {
                GameObjectFinder.findCardGameObject(round.roundCards[PlayerPosition.SOUTH]).transform.position = OUT_OF_SCREEN_POS;
            }
        }

        //for all played card of player south move to OUT_OF_SCREEN_POS

        // move out 

        //Move card to there position

    }


    private void attachSpriteForCardObject(SpadeMove move)
    {
        Debug.Log(" Move Pos " + move.playerPosition + " card: " + move.card.getCardId());
        GameObject cardGameObject = getOtherPlayerCardGameObject(move.playerPosition);
        string path = "Cards/" + move.card.getCardId().ToLower();
        cardGameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
        cardGameObject.GetComponent<Image>().enabled = true;

    }




    public void autoplayCard(SpadeMove callbreakMove)
    {
        StartCoroutine(playCardEnumerator(callbreakMove));
    }

    public void playCard(SpadeMove callbreakMove)
    {
        StartCoroutine(playCardEnumerator(callbreakMove));
    }

    public void deactivatePreviousLosingCard(SpadeRound currRound)
    {

        SpadeMove winningMove = currRound.findWinningMove();
        foreach (SpadeMove move in currRound.moves)
        {
            if (!winningMove.card.getCardId().Equals(move.card.getCardId()))
            {
                StartCoroutine(deactivateLosingCard(move));
            }

        }

    }

    public void activateAllCards()
    {
        GameObject.Find("playedCard/playernorthcard").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        GameObject.Find("playedCard/playereastcard").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        GameObject.Find("playedCard/playerwestcard").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }


    public IEnumerator deactivateLosingCard(SpadeMove move)
    {
        yield return new WaitForSeconds((moveTime + .2f) * 1.1f);

        if (move.playerPosition.Equals(PlayerPosition.SOUTH))
        {
            GameObject cobj = GameObjectFinder.findCardGameObject(move.card);
            cobj.GetComponent<CardScript>().inactivate();

        }
        else
        {
            inactivateOtherPlayerCard(move);
        }
    }

    public GameObject getOtherPlayerCardGameObject(PlayerPosition pos)
    {
        return GameObject.Find("playedCard/player" + PlayerPositionHelper.getName(pos).ToLower() + "card");

    }


    public IEnumerator playCardEnumerator(SpadeMove callbreakMove)
    {
        //Set the card Object.
        //Make the sprite visible
        //move it to correct position



        yield return new WaitForSeconds(0.2f);

        if (!callbreakMove.playerPosition.Equals(PlayerPosition.SOUTH))
        {

            GameObject cardGameObject = getOtherPlayerCardGameObject(callbreakMove.playerPosition);
            string path = "Cards/" + callbreakMove.card.getCardId().ToLower();
            cardGameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            cardGameObject.GetComponent<Image>().enabled = true;
            camera = GameObject.Find("CameraMain").GetComponent<Camera>();

            /*
            if(callbreakMove.card.suit.Equals("D") && callbreakMove.card.number.Equals("A"))
            {
                //var currScale = cardGameObject.transform.localScale;
                //cardGameObject.transform.localScale = new Vector2(currScale.x * .73f, currScale.y * .85f);

            }
            */
            //cardGameObject.transform.position = camera.WorldToScreenPoint(playerCardStartingPositionMap[callbreakMove.playerPosition]);
            cardGameObject.transform.position = GameObject.Find("player" + PlayerPositionHelper.getName(callbreakMove.playerPosition).ToLower() + "scoregroup").transform.position;

            //float moveTime = 0.3f;
            //StartCoroutine (MoveTo.MoveOverSeconds(cardGameObject, camera.WorldToScreenPoint(playerCardStaringFinalPositionMap[callbreakMove.playerPosition]), moveTime));
            if (callbreakMove.playerPosition.Equals(PlayerPosition.NORTH) || callbreakMove.playerPosition.Equals(PlayerPosition.WEST) || callbreakMove.playerPosition.Equals(PlayerPosition.EAST))
                cardGameObject.transform.DOMove(playPosition(callbreakMove.playerPosition), moveTime);
            else
                StartCoroutine(MoveTo.MoveOverSeconds(cardGameObject, playPosition(callbreakMove.playerPosition), moveTime));


            if (callbreakMove.card.suit.Equals("H") && callbreakMove.playerPosition.Equals(PlayerPosition.NORTH))
                AudioManagerScript.play(AudioClipType.PLAY_CARD);


            SpadeGameState gameState = GamePlay.matchState.getCurrentGameState();

            AudioManagerScript.play(AudioClipType.PLAY_CARD);

        }

    }

    public void inactivateOtherPlayerCard(SpadeMove move)
    {
        GameObject cardGameObject = GameObject.Find("playedCard/player" + PlayerPositionHelper.getName(move.playerPosition).ToLower() + "card");
        cardGameObject.GetComponent<Image>().color = new Color32(188, 177, 177, 255);

    }



    Vector3 playPosition(PlayerPosition playerPosition)
    {

        float height = UnityHelper.Get_Height(GameObject.Find("playedCard"));
        float width = UnityHelper.Get_Width(GameObject.Find("playedCard"));

        float cardWidth = UnityHelper.Get_Width(GameObject.Find("playedCard/playedsouthcard"));
        float cardHeight = UnityHelper.Get_Width(GameObject.Find("playedCard/playedsouthcard"));

        float posX = GameObject.Find("playedCard").transform.position.x;
        float posY = GameObject.Find("playedCard").transform.position.y + height / 2 - cardHeight / 2;

        if (playerPosition.Equals(PlayerPosition.WEST))
        {


            posX = GameObject.Find("playedCard").transform.position.x - width / 2 + cardWidth / 2;
            posY = GameObject.Find("playedCard").transform.position.y - height / 2 - cardHeight / 2;

            return GameObject.Find("playedCard/playerwestpos").transform.position;

        }

        if (playerPosition.Equals(PlayerPosition.EAST))
        {
            posX = GameObject.Find("playedCard").transform.position.x + width;
            posY = GameObject.Find("playedCard").transform.position.y - height / 2 - cardHeight / 2;

            return GameObject.Find("playereastpos").transform.position;

        }

        if (playerPosition.Equals(PlayerPosition.NORTH))
        {
            posX = GameObject.Find("playedCard").transform.position.x;
            posY = GameObject.Find("playedCard").transform.position.y - height - cardHeight;
            return GameObject.Find("playedCard/playernorthpos").transform.position;

        }

        return new Vector3(posX, posY, 0);

    }


    public void initAllScores()
    {
        SpadeMatchState currMatchState = GamePlay.matchState;
        foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
        {
            GetGameObject(pos, "totalscore").GetComponent<Text>().text = currMatchState.aggregateScoreCardWithLatestRound[pos].ToString();
            GetGameObject(pos, "score").GetComponent<Text>().text = currMatchState.getCurrentGameState().gameScore[pos].ToString();

        }
    }

    void setScoreStringNoTrickWinMoonShoot()
    {
        SpadeMatchState currMatchState = GamePlay.matchState;
        PlayerPosition playerPosition = currMatchState.getCurrentGameState().getLastCompletedRound().winner;

        Debug.Log("Updating score for winner " + playerPosition + " gameScore: " + currMatchState.getCurrentGameState().gameScore[playerPosition].ToString()
         + " aggregateScore: " + currMatchState.aggregateScoreCard[playerPosition].ToString());

        GetGameObject(playerPosition, "score").GetComponent<Text>().text = currMatchState.getCurrentGameState().gameScore[playerPosition].ToString();
        if (currMatchState.getCurrentGameState().getCurrentRound().roundNumber == 13 && currMatchState.getCurrentGameState().getCurrentRound().moves.Count >= 3)
        {
            //GetGameObject(playerPosition,  "totalscore").GetComponent<Text>().text = (currMatchState.getCurrentGameState().gameScore[playerPosition]).ToString();
            //GetGameObject(playerPosition,  "totalscore").GetComponent<Text>().text = (currMatchState.aggregateScoreCardWithLatestRound[playerPosition] + currMatchState.getCurrentGameState().gameScore[playerPosition]).ToString();
            GetGameObject(playerPosition, "totalscore").GetComponent<Text>().text = currMatchState.aggregateScoreCardWithLatestRound[playerPosition].ToString();

            foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
            {
                bool shouldUpdate = false;

                if (currMatchState.gameVariant.Equals(GameVariant.BRAY) && currMatchState.getCurrentGameState().tricksWinnerCount[pos] == 0)
                    shouldUpdate = true;
                if (currMatchState.gameVariant.Equals(GameVariant.HEARTS))
                {
                    //shouldUpdate = true;
                }

                if (currMatchState.getCurrentGameState().tricksWinnerCount[pos] == 0)
                {
                    Sequence sequence = DOTween.Sequence();
                    GetGameObject(pos, "totalscore").GetComponent<Text>().text = currMatchState.aggregateScoreCardWithLatestRound[pos].ToString();
                    GetGameObject(pos, "score").GetComponent<Text>().text = currMatchState.getCurrentGameState().gameScore[pos].ToString();
                    sequence.Append(GameObjectFinder.getPlayerScoreObject(pos).transform.DOScale(new Vector3(2f, 1.6f, 1f), .4f));
                    sequence.Append(GameObjectFinder.getPlayerScoreObject(pos).transform.DOScale(new Vector3(1f, 0.8f, 1f), .3f));
                    sequence.Play();
                }
            }


        }
        else
        {
            //GetGameObject(playerPosition,  "totalscore").GetComponent<Text>().text = currMatchState.aggregateScoreCardWithLatestRound[playerPosition].ToString();

        }

    }

    void setScoreString()
    {
        SpadeMatchState currMatchState = GamePlay.matchState;
        PlayerPosition playerPosition = currMatchState.getCurrentGameState().getLastCompletedRound().winner;

        Debug.Log("Updating score for winner " + playerPosition + " gameScore: " + currMatchState.getCurrentGameState().gameScore[playerPosition].ToString()
         + " aggregateScore: " + currMatchState.aggregateScoreCard[playerPosition].ToString());

        Debug.Log("setScoreString : roundNumber, " + currMatchState.getCurrentGameState().getCurrentRound().roundNumber + " moveMunber " + currMatchState.getCurrentGameState().getCurrentRound().moves.Count);
        bool isSpecialUpdate = false;
        if (currMatchState.getCurrentGameState().getCurrentRound().roundNumber == 13 && currMatchState.getCurrentGameState().getCurrentRound().moves.Count == 4)
        {
            if (currMatchState.gameVariant.Equals(GameVariant.HEARTS))
            {
                //Set the score of moon shot to 25 If it is not 25 already. 
                List<PlayerPosition> moonShotPlayers = currMatchState.getCurrentGameState().findShootMoonPlayer();
                Debug.Log("moonShotPlayers.Count" + moonShotPlayers.Count);
                if (moonShotPlayers != null && moonShotPlayers.Count > 0)
                {
                    Debug.Log("moonShotPlayers is" + moonShotPlayers[0]);
                    isSpecialUpdate = true;
                    GetGameObject(moonShotPlayers[0], "score").GetComponent<Text>().text = "26";
                    //Get points in final round

                    int roundPoints = currMatchState.getCurrentGameState().getCurrentRound().roundPoints;
                    int newPoints = int.Parse(GetGameObject(moonShotPlayers[0], "totalscore").GetComponent<Text>().text) + roundPoints;

                    Debug.Log("For " + moonShotPlayers[0] + " roundPoints is " + roundPoints + " and newPoints is " + newPoints);

                    GetGameObject(moonShotPlayers[0], "totalscore").GetComponent<Text>().text = newPoints.ToString();
                }

            }
            else if (currMatchState.gameVariant.Equals(GameVariant.BRAY))
            {

                //Nothing to do actually. Because 

            }

        }
        if (!isSpecialUpdate)
        {
            GetGameObject(playerPosition, "score").GetComponent<Text>().text = currMatchState.getCurrentGameState().gameScore[playerPosition].ToString();
            GetGameObject(playerPosition, "totalscore").GetComponent<Text>().text = currMatchState.aggregateScoreCardWithLatestRound[playerPosition].ToString();

        }
    }

    public GameObject GetGameObject(PlayerPosition playerPosition, string objectType)
    {
        string playerPosObjName = "player" + PlayerPositionHelper.getName(playerPosition).ToLower() + objectType;
        return GameObject.Find(playerPosObjName);
    }

    public void moveCardToWinner(SpadeMatchState callbreakMatchState)
    {
        StartCoroutine(moveCardToWinnerEnumerator(callbreakMatchState));
    }


    /*
       void lastTrickAnimation(BrayMatchState callbreakMatchState) {
           BrayGameState callbreakGameState = callbreakMatchState.getCurrentGameState();
           yield return new WaitForSeconds(configManager.callbreakTimingConfig.waitBeforeWinningMoveAnimationInSeconds);


           BrayRound currRound = callbreakGameState.getLastCompletedRound();
           Debug.Log("Determining  winner");


           if(currRound.roundNumber != 13) {
               return;
           }

           //We are in last round.

           for(PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS) {
               if(callbreakGameState.tricksWinnerCount[pos] == 0) {
                   //No trick won.   -12 animation plus update score


               }
           }


       }*/

    public IEnumerator moveCardToWinnerEnumerator(SpadeMatchState callbreakMatchState)
    {

        //Wait for the
        //initiate();

        SpadeGameState callbreakGameState = callbreakMatchState.getCurrentGameState();
        yield return new WaitForSeconds(configManager.callbreakTimingConfig.waitBeforeWinningMoveAnimationInSeconds);


        SpadeRound currRound = callbreakGameState.getLastCompletedRound();
        Debug.Log("Determining  winner");
        PlayerPosition winner = currRound.winner;
        Debug.Log("Winner is " + winner);


        //Move all card to winner
        //Disappear card.
        GameObject cardGameObject1 = GameObject.Find("playernorthcard");
        GameObject cardGameObject2 = GameObject.Find("playereastcard");
        GameObject cardGameObject3 = GameObject.Find("playerwestcard");
        GameObject cardGameObject4 = GameObjectFinder.findCardGameObject(currRound.roundCards[PlayerPosition.SOUTH]);


        Vector3 moveLocation = camera.WorldToScreenPoint(playerCardStartingPositionMap[winner]);

        moveLocation = GameObject.Find("player" + PlayerPositionHelper.getName(winner).ToLower() + "scoregroup").transform.position;


        float winningMoveAnimationTime = configManager.callbreakTimingConfig.winningMoveAnimationTimeInSeconds;

        if (winner.Equals(PlayerPosition.SOUTH))
            AudioManagerScript.play(AudioClipType.ROUND_WON);

        {

            bool shouldUpdatScore = true;

            //TODO: Delete the animations and the symbols. Blah. blah
            /*
            foreach(BrayMove move in currRound.moves) {
                if(move.card.suit.Equals("H") || move.card.getCardId().Equals("SQ")) {
                    GameObject obj = null;
                   if(move.playerPosition.Equals(PlayerPosition.SOUTH)) {
                          GameObject obj1 = GameObject.Find("suiticon" + PlayerPositionHelper.getName(move.playerPosition).ToLower());
                          obj = GameObject.Find("suiticon" + PlayerPositionHelper.getName(move.playerPosition).ToLower() + "Duplicate");
                          obj.transform.position = obj1.transform.position;

                   } else {
                      obj = GameObject.Find("suiticon" + PlayerPositionHelper.getName(move.playerPosition).ToLower());

                   }
                    float scaleFactor = 0f, endScaleFactor = 0f;
                    if(move.card.suit.Equals("H")) {
                        obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/heart_symbol");
                        //obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/heart_symbolv2");
                        endScaleFactor  = 1.1f;

                    } else {
                        //obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/spade_icon_v2");
                        //obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/spade_icon_v3");
                        obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/spade_points");
                        //obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/spade_symbol");
                        scaleFactor = 3f;
                        endScaleFactor = 1.4f;

                    }
                    Sequence sequence = DOTween.Sequence();
                    Vector3 currLoc  = obj.transform.position;
                    sequence.Append(obj.transform.DOScale(new Vector3(scaleFactor, scaleFactor, 1f), .15f));
                    sequence.Append(obj.transform.DOScale(new Vector3(endScaleFactor, endScaleFactor, 1f), .15f));
                    sequence.AppendInterval(.4f);

                    sequence.Append(obj.transform.DOMove(GameObject.Find("player"  + PlayerPositionHelper.getName(winner).ToLower() + "score").transform.position, .3f));
                    sequence.Append(obj.transform.DOScale(new Vector3(0f, 0f, 1f), .1f));


                    if(shouldUpdatScore) {
                        shouldUpdatScore = false;
                        sequence.AppendCallback(setScoreString);
                        sequence.Append(GameObjectFinder.getPlayerScoreObject(winner).transform.DOScale(new Vector3(2f, 1.6f, 1f), .4f));
                        sequence.Append(GameObjectFinder.getPlayerScoreObject(winner).transform.DOScale(new Vector3(1f, 0.8f, 1f), .3f));
                    }

                    sequence.Append(obj.transform.DOMove(currLoc, 0f));
                    sequence.Play();
                }

            }
            //GameObject obj = GameObject.Find("suiticon");

            */

        }



        yield return new WaitForSeconds(.2f);

        cardGameObject1.transform.DOMove(moveLocation, winningMoveAnimationTime).SetEase(Ease.InBack);
        cardGameObject1.transform.DOScale(Vector3.zero, winningMoveAnimationTime).SetEase(Ease.InBack);
        cardGameObject2.transform.DOMove(moveLocation, winningMoveAnimationTime).SetEase(Ease.InBack);
        cardGameObject3.transform.DOMove(moveLocation, winningMoveAnimationTime).SetEase(Ease.InBack);
        cardGameObject4.transform.DOMove(moveLocation, winningMoveAnimationTime).SetEase(Ease.InBack).
            OnComplete(activateAllCards);

        Vector3 initalScale = cardGameObject1.transform.localScale;


        yield return new WaitForSeconds(winningMoveAnimationTime /3);

        cardGameObject1.transform.DOScale(Vector3.zero, winningMoveAnimationTime*2/3).SetEase(Ease.InExpo);
        cardGameObject2.transform.DOScale(Vector3.zero, winningMoveAnimationTime * 2 / 3).SetEase(Ease.InExpo);
        cardGameObject3.transform.DOScale(Vector3.zero, winningMoveAnimationTime * 2 / 3).SetEase(Ease.InExpo);
        cardGameObject4.transform.DOScale(Vector3.zero, winningMoveAnimationTime * 2 / 3).SetEase(Ease.InExpo);


        yield return new WaitForSeconds(winningMoveAnimationTime );

        cardGameObject1.transform.position = OUT_OF_SCREEN_POS;
        cardGameObject2.transform.position = OUT_OF_SCREEN_POS;
        cardGameObject3.transform.position = OUT_OF_SCREEN_POS;
        cardGameObject4.transform.position = OUT_OF_SCREEN_POS;


        cardGameObject1.transform.localScale = initalScale;
        cardGameObject2.transform.localScale = initalScale;
        cardGameObject3.transform.localScale = initalScale;
        cardGameObject4.transform.localScale = initalScale;

    }

    public void noTrickOrShootingMoonAnimation(SpadeMatchState matchState, List<PlayerPosition> winnerPos)
    {
        StartCoroutine(nilSuccessAnimation(matchState, winnerPos));
    }

    public IEnumerator nilSuccessAnimation(SpadeMatchState matchState, List<PlayerPosition> winnerPos)
    {
        Debug.Log("In nilSuccessAnimation");
        string posStr = "";

        if (winnerPos.Count == 1)
        {
            posStr = winnerPos[0].ToString();
        }
        else
        {
            foreach (PlayerPosition pos in winnerPos)
            {
                posStr = posStr + pos.ToString() + " ";
            }
        }
        yield return new WaitForSeconds(1f);



        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(new WarningEntity("NIL Bid succeded by Player : " + posStr, "C"));


        yield return new WaitForSeconds(.1f);

        //GamePlay.infoTextAnimation("No Trick Won");

        List<PlayerPosition> otherPlayers = new List<PlayerPosition>();

        foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
        {
            if (!winnerPos.Contains(pos))
            {
                otherPlayers.Add(pos);
            }
        }

        Color32 NEGATIVE_COLOR = new Color32(243, 22, 88, 255);
        //Color32 POSITIVE_COLOR = new Color32(0, 171,220, 255);
        Color32 POSITIVE_COLOR = new Color32(44, 200, 20, 255);

        float scaleFactor = 1.3f, endScaleFactor = 1.2f;
        bool shouldUpdatScore = true;

        foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
        {

            Sequence sequence = DOTween.Sequence();
            GameObject obj = GameObject.Find("suiticon2" + PlayerPositionHelper.getName(pos).ToLower());
            GameObject txtView = GameObject.Find("suiticon2" + PlayerPositionHelper.getName(pos).ToLower() + "/txt");

            if (winnerPos.Contains(pos))
            {
                obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("cardgamecommon/circle_custom");
                obj.GetComponent<Image>().color = POSITIVE_COLOR;
                txtView.GetComponent<Text>().text = "+" + matchState.getCurrentGameState().nilPenalty().ToString();
            }
            else
            {
                continue;
            }

            Vector3 currLoc = obj.transform.position;
            sequence.AppendInterval(1.7f);
            sequence.Append(obj.transform.DOScale(new Vector3(scaleFactor, scaleFactor, 1f), .15f));
            sequence.Append(obj.transform.DOScale(new Vector3(endScaleFactor, endScaleFactor, 1f), .15f));
            sequence.AppendInterval(.4f);
            sequence.Append(obj.transform.DOMove(GameObject.Find("player" + PlayerPositionHelper.getName(pos).ToLower() + "score").transform.position, .5f));

            sequence.Append(obj.transform.DOScale(new Vector3(0f, 0f, 1f), .05f));
            if (shouldUpdatScore)
            {

                shouldUpdatScore = false;
                sequence.AppendCallback(setScoreStringNoTrickWinMoonShoot);
                sequence.Append(GameObjectFinder.getPlayerScoreObject(winnerPos[0]).transform.DOScale(new Vector3(2f, 1.6f, 1f), .4f));
                sequence.Append(GameObjectFinder.getPlayerScoreObject(winnerPos[0]).transform.DOScale(new Vector3(1f, 0.8f, 1f), .3f));
            }

            sequence.Append(obj.transform.DOMove(currLoc, 0f));

            sequence.Play();

        }

    }

    public void enableAllCards()
    {

        GameObject.Find("playernorthcard").GetComponent<Image>().enabled = true;
        GameObject.Find("playersouthcard").GetComponent<Image>().enabled = true;
        GameObject.Find("playereastcard").GetComponent<Image>().enabled = true;
        GameObject.Find("playerwestcard").GetComponent<Image>().enabled = true;

        List<GameObject> gameObjects = GameObjectFinder.getCardObjects();

        foreach (GameObject gameObj in gameObjects)
        {


            gameObj.GetComponent<Image>().enabled = true;
        }

    }

}