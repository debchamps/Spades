using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;



public class BidButtonGo : MonoBehaviour {
    float movementTime = .75f;

    ConfigManager configManager = new ConfigManager();

   public void setBid() {

     Debug.Log("Setting bidValue to " );


       List<GameObject> biddingButtons = GameObjectFinder.getBiddingButtons();

       foreach(GameObject btn in biddingButtons) {
           if(btn.GetComponent<BidButton>().selected) {
               int bidVal = btn.GetComponent<BidButton>().bidValue;

               Debug.Log("Setting bidValue to " + bidVal);

                CallbreakBid callbreakBid = new CallbreakBid(123, PlayerPosition.SOUTH, bidVal);
                disableBidding();
                
                GamePlay.matchState.updateBid(callbreakBid);
           }
       }

   }

   void hideContinueButton() {
        GameObject.Find("bidcontinuetext").GetComponent<Text>().enabled = false;
       Color current = GameObject.Find("bidFinalBtn").GetComponent<Image>().color;
       current.a = 0f;
       GameObject.Find("bidFinalBtn").GetComponent<Image>().color = current;

    
   }


   public void showContinueButton() {
        GameObject.Find("bidcontinuetext").GetComponent<Text>().enabled = true;

        GameObject.Find("bidFinalBtn").GetComponent<Image>().DOFade(1,.1f);
   }



    public void animateBidding() {



    }



   public void disableBidding() {

       //GameObject layout = GameObject.Find("biddinglayout");

        //AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);
        GameObject layout = GameObject.Find("biddingparent");


        float movementTime = 1.5f;
        layout.transform.DOMoveX(2 * Screen.width, movementTime);


        //StartCoroutine(MoveTo.MoveOverSeconds(layout, new Vector3(2 * Screen.width,getBiddingY(),0), .5f));

   }

   public void enableBidding() {
        //Close others.

        var game = GamePlay.matchState.getCurrentGameState();
        closeOthers();
        hideContinueButton();
        List<GameObject> biddingButtons = GameObjectFinder.getBiddingButtons();

        foreach(GameObject btn in biddingButtons) {
            btn.GetComponent<BidButton>().deselect();
             btn.GetComponent<BidButton>().deactivated = false;
        }


       //GameObject layout = GameObject.Find("biddinglayout");
       GameObject layout = GameObject.Find("biddingparent");
       float floatY = getBiddingY();
       layout.transform.position = new Vector3(2 * Screen.width,floatY,0);
        //this.waitBeforeBiddingEnableAnimation = waitBeforeBiddingAnimation * 1.5f;

        var animationWait = configManager.callbreakTimingConfig.waitBeforeBiddingEnableAnimation;
        if (game.callBreakBidding.callBreakBiddingData.playerBidAmount.Count == 0)
        {
            animationWait = animationWait * 1.5f;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(animationWait);
        sequence.Append(layout.transform.DOMoveX(Screen.width/2, movementTime));

        sequence.Play();

        ComputerBidder computerBidder = new ComputerBidder();
        int calculatedBid = computerBidder.calculateBid(game, game.playerMap[PlayerPosition.SOUTH].allCards, PlayerPosition.SOUTH);

        int maxSouthBid = game.maxAllowedBid(PlayerPosition.SOUTH);

        if(maxSouthBid < 13) {
            for(int i=maxSouthBid+1;i<=13;i++) {
                GameObject.Find("spadeBidButton" + i.ToString()).GetComponent<BidButton>().deactivate();
            }
        }


        foreach(GameObject btn in biddingButtons) {
            if(btn.GetComponent<BidButton>().bidValue.Equals(calculatedBid)) {
                //btn.GetComponent<BidButton>().select();

            }
        }


       //StartCoroutine(MoveTo.MoveOverSeconds(layout, new Vector3(Screen.width/2,floatY,0), 1));

   }


    public void closeOthers() {
        //GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().disableScoreCard();
        GameObject.Find("ScriptEmpty").GetComponent<SettingsScript>().close();
        GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().close();
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().close();
    }

   private float getBiddingY() {
        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();


        Debug.Log("Current resolution is " + Screen.currentResolution + " width us " + Screen.width);


        float cardHeight = RectTransformExt.GetWorldRect((RectTransform)cardObjects[0].transform).height;
        float playerSouthCardBoundaryHeight = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthCardBoundary").transform).height;
        float playerSouthCardBoundary = GameObject.Find("playerSouthCardBoundary").transform.position.y;

        float factor = 1;
        if(Screen.width > 1500) {
            factor = 1.2f;
        }
       float  floatY = GameObject.Find("playerSouthCardBoundary").transform.position.y ;//+ playerSouthCardBoundaryHeight * factor;
       //float  floatY = GameObject.Find("playerSouthCardBoundary").transform.position.y + playerSouthCardBoundaryHeight * 2f;
        Rect southTopRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthTopCard").transform);
        //float firstRowY = GameObject.Find("playerSouthCardBoundary").transform.position.y + cardHeight/2;
        Rect biddingPanelRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("biddingparent").transform);

        float firstRowY = southTopRect.y + biddingPanelRect.height/2;
       return firstRowY;
 
   }



}