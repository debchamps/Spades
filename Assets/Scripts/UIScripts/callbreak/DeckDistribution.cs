using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using System;


public class DeckDistribution : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerPositionHelper playerPositionHelper = new PlayerPositionHelper();

    
    void Start()
    {
        
    }


    ConfigManager configManager = new ConfigManager();

    /*
    private void setCardToGameObject(List<GameObject> gameObjects, List<Card> cards) {

        for(int i=0;i<cards.Count;i++) {

            Image existingImage = gameObject.GetComponent<Image>();

            string path = "Cards/" + cards[i].getCardId() + ".png";

            existingImage.sprite = (Sprite)Resources.Load(path);
        }
    }
    */

    public  void setCard(GameObject obj, Card card) {

        obj.GetComponent<CardScript>().setCardSprite(card);
        string path =   "Cards/" +  card.getCardId().ToLower();
        //TODO: CHeck
        //obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
        //obj.GetComponent<Image>().enabled = true;

    }

    public  void distribute(SpadeGameState callbreakGameState) {

        StartCoroutine(distributeEnumarator(callbreakGameState));
    }

    public void resume(SpadeMatchState matchState) {
        List<Card> southCards = matchState.getCurrentGameState().playerCardMap[PlayerPosition.SOUTH];
        associateCardWithCardObjects(matchState.getCurrentGameState());
        //Now put the card which was 
        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();


        List<Card> rearrangedCards = new BrayCardHelper().rearrange(matchState.getCurrentGameState().playerCardMap[PlayerPosition.SOUTH]);
        int i=0;
        Dictionary<string, int> cardToLocationMapping = new Dictionary<string, int>();
        foreach(Card card in rearrangedCards) {
            cardToLocationMapping[card.getCardId()] = i;
            i++;
        }

        for(int j=0;j<13;j++) {
            int newIndex = cardToLocationMapping[cardObjects[j].GetComponent<CardScript>().card.getCardId()];
            cardObjects[j].transform.position = getCardPositionInScreen(newIndex);
            cardObjects[j].GetComponent<CardScript>().setFront();
        }

    }


    void MovePassCardToPlayer(SpadeGameState gameState) {
        

        PlayerPosition playerToPass = gameState.playerToPass(PlayerPosition.SOUTH);
        Vector3 moveLocation = GameObject.Find("deck" + PlayerPositionHelper.getName(playerToPass).ToLower()).transform.position;

        for(int i=1;i<=3;i++) {
            Sequence sequence=  DOTween.Sequence();
            sequence.AppendInterval(0f);
            Card cardToPass = GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().card;
            GameObject cardToPassObj = GameObjectFinder.findCardGameObject(cardToPass);
            sequence.Append(cardToPassObj.transform.DOMove(moveLocation, .5f));
            sequence.AppendInterval(.3f);
            sequence.Append(cardToPassObj.transform.DOScale(0, .5f));
            sequence.Play();
            //sequence.Append(cardToPassObj.transform.DOMove(moveLocation, .5f));
        }        
        /*
        //sequence.AppendInterval(.5f);
        for(int i=1;i<=3;i++) {
            Card cardToPass = GameObject.Find("cardCombo" + i.ToString()).GetComponent<PassCard>().card;
            GameObject cardToPassObj = GameObjectFinder.findCardGameObject(cardToPass);
            cardToPassObj.transform.DOScale(0, .5f);
        }        
        */

    }

    void RotateBackCards() {
        for(int i=1;i<=13;i++) {
            if(GameObject.Find("testCard" + i.ToString()).GetComponent<CardScript>().mCardState.Equals(CardState.Back)) {
                Debug.Log("Card i " + i.ToString() + " is back");
                GameObject.Find("testCard" + i.ToString()).GetComponent<CardScript>().moveToFront();
                GameObject.Find("testCard" + i.ToString()).GetComponent<CardScript>().highlight(.3f,1f);
            }
            //sequence.Append(cardToPassObj.transform.DOMove(moveLocation, .5f));
        }        


    }


    public  IEnumerator distributeEnumarator(SpadeGameState callbreakGameState) {

        // Invalidate the card-object cache so every lookup in this deal
        // resolves against the freshly-assigned sprites — not the previous hand.
        // Without this, stale cache hits leave played cards stranded on screen.
        GameObjectFinder.InvalidateCardCache();

        AudioManagerScript.play(AudioClipType.SHUFFLE_CARD);

        List<Card> southCards = callbreakGameState.playerCardMap[PlayerPosition.SOUTH];
        /*
        for(int i=1;i<=13;i++) {
             GameObject.Find("testCard" + i.ToString());


            GameObject.Find("testCard" + i.ToString()).GetComponent<CardScript>().setCardSprite = southCards[i-1];
            //GameObject.Find("testCard" + i.ToString()).GetComponent<CardScript>().setCardSprite(southCards[i-1]);
        }  */

        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();
        List<Card> cards = callbreakGameState.playerCardMap[PlayerPosition.SOUTH];

        int j=0;
        foreach(GameObject cardObj in cardObjects) {

            cardObj.GetComponent<CardScript>().setBack();
            cardObj.GetComponent<CardScript>().setCardSprite(cards[j]);
            cardObj.GetComponent<CardScript>().distributionIndex = j;

            PlayerPosition shuffler = playerPositionHelper.getPreviousPlayerPosition(callbreakGameState.matchStarter);

            cardObj.transform.position = GameObject.Find("deck" + PlayerPositionHelper.getName(shuffler).ToLower()).transform.position;

            j++;
        }

        
        yield return new WaitForSeconds(configManager.callbreakTimingConfig.cardDistributionDelay/1000);


        for(int i=0;i<13;i++) {
            cardObjects[i].transform.DOMove(getCardPositionInScreen(i), .75f).SetEase(Ease.OutBack);

        }

        /*
        int margin = Screen.width/7;

        //StartCoroutine (MoveOverSeconds (cardObjects[0], new Vector3( 10 + 5*margin, 200,  0), .4f));                
       // StartCoroutine (moveToPosition (cardObjects[0].transform, new Vector3( 10 + 5*margin, 100,  0), .4f));

        float cardHeight = RectTransformExt.GetWorldRect((RectTransform)cardObjects[0].transform).height;

        //float cardHeight = ((RectTransform)cardObjects[0].transform).rect.height;

        Rect southRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthCardBoundary").transform);
        Rect topRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("TopObj").transform);


        float southHeight = Screen.height - topRect.height;

        //float firstRowY = Screen.height * 2.2f/8.6f;
        //float secondRowY = firstRowY - cardHeight * 1.2f;
        //float firstRowY = GameObject.Find("playerSouthCardBoundary").transform.position.y + cardHeight/2;
        //float firstRowY = GameObject.Find("playerSouthCardBoundary").transform.position.y + southRect.height/2 - cardHeight/2;
        float firstRowY = 0;

        Debug.Log("southHeight/cardHeight" + southHeight/cardHeight);

        if(southHeight > 5 * cardHeight)
        firstRowY = Screen.height - (topRect.height + southHeight/2 - cardHeight);
        else 
        firstRowY = Screen.height - (topRect.height + cardHeight);

        Rect southTopRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthTopCard").transform);
        //float firstRowY = GameObject.Find("playerSouthCardBoundary").transform.position.y + cardHeight/2;

        firstRowY = southTopRect.y + southTopRect.height;

        float gap = 1.2f;
        if(southHeight> 5 * cardHeight)
        gap = 1.5f;

        float secondRowY = firstRowY - cardHeight * gap;
        Debug.Log("firstRowY" + firstRowY + "topRect.height" + topRect.height + "southHeight " + southHeight + " cardHeight" + cardHeight + "Screen.height  " + Screen.height );

        for(int i=0;i<7;i++) {

        //StartCoroutine (MoveTo.MoveOverSeconds(cardObjects[i], new Vector3( margin/3 + i*margin, firstRowY,  0), .3f));
        cardObjects[i].transform.DOMove( new Vector3( margin/3 + i*margin, firstRowY,  0), .3f);

        }
        //yield return new WaitForSeconds(.1f);

        margin = Screen.width/6;

        for(int i=0;i<6;i++) {

        //StartCoroutine (MoveTo.MoveOverSeconds(cardObjects[7+i], new Vector3( margin/3 + i*margin , secondRowY,  0), .3f));

        cardObjects[7+i].transform.DOMove(new Vector3(margin/3 + i*margin , secondRowY,  0), .3f);

        //StartCoroutine (MoveTo.MoveOverSeconds (cardObjects[7+i], new Vector3( margin/3 + i*margin, cardHeight*1.3f,  0), .3f));
        //yield return new WaitForSeconds(.1f);

        }
        */
        
        yield return new WaitForSeconds(.3f);


        foreach(GameObject cardObj in cardObjects) {
            yield return new WaitForSeconds(.05f);
            cardObj.GetComponent<CardScript>().moveToFront();
        }

    }



    private void associateCardWithCardObjects(SpadeGameState callbreakGameState) {
        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();
        List<Card> cards = callbreakGameState.playerCardMap[PlayerPosition.SOUTH];

        int j=0;
        foreach(GameObject cardObj in cardObjects) {

            cardObj.GetComponent<CardScript>().setBack();
            cardObj.GetComponent<CardScript>().setCardSprite(cards[j]);

            PlayerPosition shuffler = playerPositionHelper.getPreviousPlayerPosition(callbreakGameState.matchStarter);

            cardObj.transform.position = GameObject.Find("deck" + PlayerPositionHelper.getName(shuffler).ToLower()).transform.position;

            j++;
        }

    }


    public void moveCardToPosition(Card cardToMove) {

        GameObject cardObj = GameObjectFinder.findCardGameObject(cardToMove);
        if(cardObj != null)
          cardObj.transform.DOMove(getCardPositionInScreen(cardObj.GetComponent<CardScript>().distributionIndex), .3f);      


    }

    public Vector3 getCardPositionInScreen(int cardIndex) {

        List<GameObject> cardObjects =  GameObjectFinder.getCardObjects();

        //float firstRowY = 0;
        Rect southRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthCardBoundary").transform);
        Rect topRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("TopObj").transform);
        float cardHeight = RectTransformExt.GetWorldRect((RectTransform)cardObjects[0].transform).height;
        Rect southTopRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playerSouthTopCard").transform);

        float southHeight = Screen.height - topRect.height;

        float firstRowY = GameObject.Find("playerSouthCardBoundary").transform.position.y + cardHeight/2;
        int margin = Mathf.Min(Screen.width, Screen.height)/7;
        int extraMargin = 0;

        if(Screen.width > Screen.height) {
            extraMargin = (Screen.width - Screen.height)/2;
        }


        if(southHeight > 5 * cardHeight)
            firstRowY = Screen.height - (topRect.height + southHeight/2 - cardHeight);
        else 
            firstRowY = Screen.height - (topRect.height + cardHeight);

        firstRowY = southTopRect.y + southTopRect.height;

        if(cardIndex <= 6) {
            return new Vector3( extraMargin + margin/3 + cardIndex*margin, firstRowY,  0);

        } else {
            float gap = 1.2f;
            if(southHeight> 5 * cardHeight)
                gap = 1.5f;
            float secondRowY = firstRowY - cardHeight * gap;
            margin = Mathf.Min(Screen.width, Screen.height)/6;
            return new Vector3(extraMargin + margin/3 + (cardIndex - 7)*margin , secondRowY,  0);

        }


    }

    private void animate() {

    }
}
