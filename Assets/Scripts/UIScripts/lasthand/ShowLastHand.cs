using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/**
    Rating Manager. Redirects to Google PlayStore / IOS App Store when user submits an internal review.
*/
public class ShowLastHand : MonoBehaviour {


    bool isDebug = false;

    void Start() {
        if(isDebug) {
            GameObject.Find("showlasthand/previousicon").transform.localScale = new Vector3(1,1,1);
            GameObject.Find("showlasthand/nexticon").transform.localScale = new Vector3(1,1,1);
        }
        

        
    }

    static List<List<SpadeMove>> allMoves = new List<List<SpadeMove>>();

    static int currentRound = 0;

    static bool open = false;

    public void enableButton() {
            GameObject.Find("showlasthandbutton").GetComponent<Image>().DOFade(1,0.5f);
            GameObject.Find("showlasthandbutton/btntxt").GetComponent<Text>().DOFade(1,0.5f);

    }


    public void disableButton() {
            GameObject.Find("showlasthandbutton").GetComponent<Image>().DOFade(0,0.5f);
            GameObject.Find("showlasthandbutton/btntxt").GetComponent<Text>().DOFade(0,0.5f);

    }


    void Update() {
        if(open) {
             if(AnimationUtil.isClickedOutside(GameObject.Find("showlasthand")) && AnimationUtil.isClickedOutside(GameObject.Find("showlasthandbutton"))) {
                 close();
             }

        }
     }


    
    void openDialogue(GameObject obj, float scaleTo, float animateTime) {
        var increaseScale = scaleTo * 1.05f;
        Sequence sequence=  DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(increaseScale, animateTime * .85f));
        sequence.Append(obj.transform.DOScale(scaleTo, animateTime * .15f));
        sequence.Play();
       
    }

    public void close() {    
        //GameObject.Find("showlasthand").transform.position = new Vector3(Screen.width * 5 , Screen.height/2, 1f);
        if(open) {
            open = false;

        }
        GameObject.Find("showlasthand").transform.DOScale(0f, .25f);
    }   


    public void show() {    
        
        if(open) {
            close();   
            return;
        } else {
            open = true;

        GameObject.Find("showlasthand/playerWestSeatLastHand/PlayerScreen").GetComponent<OpenPlayerProfile>().set();
        GameObject.Find("showlasthand/playerEastSeatLastHand/PlayerScreen").GetComponent<OpenPlayerProfile>().set();
        GameObject.Find("showlasthand/playerSouthSeatLastHand/PlayerScreen").GetComponent<OpenPlayerProfile>().set();
        GameObject.Find("showlasthand/playerNorthSeatLastHand/PlayerScreen").GetComponent<OpenPlayerProfile>().set();
        


            GameObject.Find("showlasthand").transform.DOScale(0f, 0f);
            GameObject obj = GameObject.Find("showlasthand");


            if(GamePlay.matchState == null)
                return;

            var currentGame = GamePlay.matchState.getCurrentGameState();

            foreach(var round in currentGame.rounds) {
                if(round.roundState.Equals(SpadeRound.RoundState.COMPLETED)) {
                    allMoves.Add(round.moves);
                }
            }

            currentRound = allMoves.Count - 1;
            Debug.Log("currentRound " + currentRound);

            if(currentRound < 0 )
                return;

            AnimationUtil.openDialogue(obj, .8f, .3f);
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

            showCards(allMoves[currentRound]);

        }


        //Get only the last allMoves

    }



    public void showCards(List<SpadeMove> moves) {
        GameObject.Find("showlasthand").transform.position = new Vector3(Screen.width/2, Screen.height/2, 1f);

        int i=0;
        foreach(SpadeMove move in moves) {
            GameObject cardObj = getCardObj(move);
            string path =   "Cards/" +  move.card.getCardId().ToLower();
            cardObj.GetComponent<Image>().DOFade(0.0f,0f);
            cardObj.GetComponent<Image>().sprite =  Resources.Load<Sprite>(path);
            cardObj.GetComponent<Image>().DOFade(1.0f,.1f).SetDelay((i+1) * .4f);
            i++;
        }

    }

    GameObject getCardObj(SpadeMove move) {
        //Debug.Log("playedcardlasthand/player" + PlayerPositionHelper.getName(move.playerPosition).ToLower()+"pos");
         return GameObject.Find("playedcardlasthand/player" + PlayerPositionHelper.getName(move.playerPosition).ToLower()+"cardlh");
    }


    public void previousRound() {
        Debug.Log(" previousRound currentRound " + currentRound);
        if(currentRound == 0)
            return;
        currentRound = currentRound - 1;
        Debug.Log(" previousRound currentRound " + currentRound);
        showCards(allMoves[currentRound]);

    }

    public void nextRound() {

        Debug.Log(" nextRound currentRound " + currentRound + " allMoves.Count" +  allMoves.Count);
        if(currentRound >= allMoves.Count - 1)
            return;

        currentRound = currentRound + 1;
                Debug.Log(" nextRound currentRound " + currentRound);

        showCards(allMoves[currentRound]);

    }

}