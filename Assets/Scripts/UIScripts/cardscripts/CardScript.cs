using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;



public enum CardState
{
 Front,
 Back
}
//[RequireComponent(typeof(Image))]
public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
     public GameObject mFront; //card front
     public GameObject mBack; //Back of card
    public CardState mCardState = CardState.Back;

    private static string TAG = "CardScriptLog";
    public float mTime = 1.7f;
    private bool isActive = false;

    public int distributionIndex = -1;

	public Sprite frontSprite;
	public Sprite backSprite;
	public float uncoverTime = 12.0f;

    public bool isDragging = false;
    public bool isClicked = false;

    public Card card;
        public string cardId;

    Camera camera;

    bool cardPlayed = false;
    bool isDraggable = true;

    bool isMoving = false;

    ConfigManager configManager = new ConfigManager();


    private Vector3 end = new Vector3(0,0,0);

    private Vector3 initial;

    private bool initialScaleSet = false;
    private Vector3 initialScale;

    void Start() {

        InitCardFlip();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
       // DebugLog.Log("\nMouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      //  DebugLog.Log("\nMouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Enter->Down->Up->Clicked->Exit
        //Enter->Down->dragBegin->Up->Clicked->dragEnd->Exit
       // DebugLog.Log("\nMouse Exit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      //  DebugLog.Log("\nMouse Up");
    }


    public void inactivate() {
         mFront.GetComponent<Image>().color = new Color32(188,177,177,255);

    }

    public void activate() {
         mFront.GetComponent<Image>().color = new Color32(255,255,255,255);

    }


    public void highlight(float initialDelay, float duration) {
        float height = RectTransformExt.GetWorldRect((RectTransform)gameObject.transform).height;
        //float height = RectTransformExt.GetWorldRect(gameObject.transform).height;

        AnimationUtil.moveUpAndDown(gameObject, .1f, 2, initialDelay, height/8);


         mFront.GetComponent<Image>().color = new Color32(223,243,188,255);

         mFront.GetComponent<Image>().DOColor(new Color32(255,255,255,255),.2f).SetDelay(duration);

    }


    public void highlight() {
        float height = RectTransformExt.GetWorldRect((RectTransform)gameObject.transform).height;
        //float height = RectTransformExt.GetWorldRect(gameObject.transform).height;

        AnimationUtil.moveUpAndDown(gameObject, .1f, 2, 0f, height/8);


         mFront.GetComponent<Image>().color = new Color32(223,243,188,255);

         mFront.GetComponent<Image>().DOColor(new Color32(255,255,255,255),.2f).SetDelay(1f);

    }



    public void InitCardFlip() {
   
        //mFront = gameObject;
        if(mCardState==CardState.Front)
        {
        // If you start from the front, rotate the back 90 degrees so that you can't see the back.
        mFront.transform.eulerAngles = Vector3.zero;
        mBack.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
        // Starting from the back, the same thing
        mFront.transform.eulerAngles = new Vector3(0, 90, 0);
        mBack.transform.eulerAngles = Vector3.zero;
        }
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        //DebugLog.Log("\n OnBeginDrag ");
        initial = this.transform.position;
        eventData.pointerDrag = gameObject;

        //Anyone else is dragging should also result in no drag

        List<GameObject> cardObjects = GameObjectFinder.getCardObjects();
        foreach(GameObject cardObj in cardObjects) {
            if(cardObj.GetComponent<CardScript>().isDragging || cardObj.GetComponent<CardScript>().isClicked) {
                eventData.pointerDrag = null;

            }
        }

        if(isDragging)
            eventData.pointerDrag = null;

        if(cardPlayed)
            eventData.pointerDrag = null;



        SpadeGameState currGame = GamePlay.matchState.getCurrentGameState();
        foreach(SpadeRound round in currGame.rounds) {
            foreach(Card card1 in round.playedCards) {
                if(card1.getCardId().Equals(card.getCardId())) {
                    eventData.pointerDrag = null;

                }
            }

        }

        if(eventData.pointerDrag != null)
            isDragging = true;


        //If this card is already played. Make it null.




    }

    public void OnDrag(PointerEventData data)
    {
        isDragging = true;

        this.transform.position += (Vector3)data.delta;
    }



    public void OnPointerClick(PointerEventData eventData)
    {
         Debug.Log(TAG + "Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);

        if(isDragging || isClicked)
        return;

         Debug.Log(TAG + "Clicked: is not dragging or clicked");

        List<GameObject> cardObjects = GameObjectFinder.getCardObjects();
        foreach(GameObject cardObj in cardObjects) {
            if(cardObj.GetComponent<CardScript>().isDragging || cardObj.GetComponent<CardScript>().isClicked) {
                return;
            }
        }

         Debug.Log(TAG + "Other cards are not clicked");

        isClicked = true;
        bool isPassModeOn = GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.PASS_CARD);

        bool canPassCard = false, removeFromPassBox = false;





        bool isMatchPlayOn = GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.IN_PROGRESS);
        if(!isMatchPlayOn && !canPassCard && !removeFromPassBox) {
            isClicked = false;
            return;

        } else {

            {

                bool isPlayerTurn = isMatchPlayOn && GamePlay.matchState.getCurrentGameState().getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH);

                List<Card> validCards = new BrayCardHelper().validCards(GamePlay.matchState.getCurrentGameState(), PlayerPosition.SOUTH);
                bool isValidCard = new CardHelper().contains(validCards, card);
                Debug.Log(TAG + " isValidCaed" + isValidCard + " for " + card.getCardId());
                Debug.Log("isValidCard " + isValidCard + " isPlayerTurn " + isPlayerTurn );
                if(!isValidCard && isPlayerTurn) {
                    WarningEntity invalidReason = new BrayCardHelper().getInvalidReason(GamePlay.matchState.getCurrentGameState(), PlayerPosition.SOUTH, card);
                    if(invalidReason != null) {
                        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().disappear();
                        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(invalidReason);

                    }

                    isClicked = false;
                    return;

                }


                if(!isPlayerTurn || !isValidCard ) {
                    isClicked = false;
                    return;
                    //StartCoroutine (MoveTo.MoveOverSeconds (gameObject, initial, configManager.callbreakTimingConfig.humanCardBringToInitialPos/1000));

                } else {
                    moveCardAnimation();
                }    
            }    



        }
        


        //Only a click not a 


        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      //  DebugLog.Log("\n OnEndDrag ");
            

        camera = GameObject.Find ("CameraMain").GetComponent<Camera>();

        end = this.transform.position;

        //potentially have a check whether this card is already played.

        bool isMatchPlayOn =GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.IN_PROGRESS);

        bool isPlayerTurn = isMatchPlayOn && GamePlay.matchState.getCurrentGameState().getCurrentRound().nextMovePlayer.Equals(PlayerPosition.SOUTH);

        //Debug.Log("isMatchPlayOn " + isMatchPlayOn + " isPlayerTurn " + isPlayerTurn +  "roundNumber " + GamePlay.callbreakMatchState.getCurrentGameState().getCurrentRound().roundNumber + " moveNumber " +  GamePlay.callbreakMatchState.getCurrentGameState().getCurrentRound().moveNumber );
        bool isPassModeOn = GamePlay.matchState.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.PASS_CARD);

        bool canPassCard = false , removeFromPassBox = false;






        if(!isMatchPlayOn && !canPassCard && !removeFromPassBox) {
            StartCoroutine (MoveTo.MoveOverSeconds (gameObject, initial, configManager.callbreakTimingConfig.humanCardBringToInitialPos/1000));
            isDragging = false;

            return;
        } else {
             {
                //Logic to play card
                List<Card> validCards = new BrayCardHelper().validCards(GamePlay.matchState.getCurrentGameState(), PlayerPosition.SOUTH);
                bool isValidCard = new CardHelper().contains(validCards, card);
                Debug.Log("isValidCard " + isValidCard + " isPlayerTurn " + isPlayerTurn );
                if(!isValidCard && isPlayerTurn) {
                    WarningEntity invalidReason = new BrayCardHelper().getInvalidReason(GamePlay.matchState.getCurrentGameState(), PlayerPosition.SOUTH, card);
                    if(invalidReason != null) {
                        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().disappear();
                        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(invalidReason);

                        //GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappearV2(invalidReason);
                        //WarningScript.showWarningAndDisappear(invalidReason);
                    }

                }

                bool notPlayed = false;
                if(initial.y > end.y) {
                    notPlayed = true;
                }

                if(!isPlayerTurn || !isValidCard || notPlayed) {
                    StartCoroutine (MoveTo.MoveOverSeconds (gameObject, initial, configManager.callbreakTimingConfig.humanCardBringToInitialPos/1000));
                    isDragging = false;

                } else {
                    moveCardAnimation();
                }        

            }



        }


    }




    private void moveCardAnimation() {

            initialScale = gameObject.transform.localScale;
            initialScaleSet = true;
            cardPlayed = true;
            GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().disappear();

            Rect northCardRec = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playernorthcard").transform);
            Rect cardRect = RectTransformExt.GetWorldRect((RectTransform)gameObject.transform);
            Vector3 scaleVector = new Vector3(1.2f * northCardRec.width/cardRect.width,1.2f * northCardRec.height/cardRect.height , 1);
            gameObject.transform.DOMove(playSouthPosition(),configManager.callbreakTimingConfig.humanCardPlayAnimation/1000);
            gameObject.transform.DOScale(scaleVector,configManager.callbreakTimingConfig.humanCardPlayAnimation/1000);
            StartCoroutine(AudioManagerScript.playWithDelay(AudioClipType.PLAY_CARD, configManager.callbreakTimingConfig.humanCardPlayAnimation * 2/(3 * 1000)));
            StartCoroutine(updateGameState());

    }


    public void autoThrow() {
        //If Autothrow enabled.

        Debug.Log("Autothrow : Checking " + isDragging + " for " +  card.getCardId());

        if(isDragging)
        {
            return;
        }
        else {
            cardPlayed = true;
            Debug.Log("Autothrow : Start" + " for " +  card.getCardId());
            StartCoroutine (MoveTo.MoveOverSeconds (gameObject, playSouthPosition(), configManager.callbreakTimingConfig.humanCardPlayAnimation/1000));
            StartCoroutine(AudioManagerScript.playWithDelay(AudioClipType.PLAY_CARD, configManager.callbreakTimingConfig.humanCardPlayAnimation * 2/(3 * 1000)));

        }


    }



    IEnumerator updateGameState() {
        SpadeMove callbreakMove = new SpadeMove(PlayerPosition.SOUTH, card);
        yield return new WaitForSeconds(configManager.callbreakTimingConfig.humanCardPlayAnimation/1000);
        GamePlay.matchState.updateMove(callbreakMove);
        yield return new WaitForSeconds(2*configManager.callbreakTimingConfig.humanCardPlayAnimation/1000);
        isDragging = false;
        isClicked = false;


    }


Vector3 playSouthPosition() {

    float height = UnityHelper.Get_Height(GameObject.Find("playedCard"));
    float width = UnityHelper.Get_Width(GameObject.Find("playedCard"));

    float cardWidth = UnityHelper.Get_Width(GameObject.Find("playedsouthcard"));
    float cardHeight = UnityHelper.Get_Width(GameObject.Find("playedsouthcard"));
    
    float posX = GameObject.Find("playedCard").transform.position.x + width/2 - cardWidth/2;
    float posY = GameObject.Find("playedCard").transform.position.y;

    //return new Vector3(posX, posY, 0);

     return GameObject.Find("playersouthpos").transform.position;


}


void Update()
{
    
}



public  void setCardSprite(Card card1) {
        this.card = card1;
        this.cardId = card1.getCardId();
        string path =   "Cards/" +  card1.getCardId().ToLower();
        frontSprite = Resources.Load<Sprite>(path);
        string backpath =   "Cards/card_back" ;
        backSprite = Resources.Load<Sprite>(backpath);
        cardPlayed = false;

        //eventData.pointerDrag = gameObject;

        isDragging = false;
        isClicked = false;


        if(initialScaleSet)
            gameObject.transform.localScale = initialScale;

        mFront.GetComponent<Image>().sprite = frontSprite;
        activate();

        //new Color32(255,255,255,255);
        //gameObject.GetComponent<Image>().sprite = backSprite;
        //InitCardFlip();
        //StartCoroutine(uncoverCardEnumerator(gameObject.transform, true));
        //StartCoroutine(uncoverCardEnumerator(gameObject.transform, true));
        //StartCoroutine(ToFront());
}


public void slightUp() {

    RectTransform rt = (RectTransform)gameObject.transform;
    

    float width = rt.rect.width;
    float height = rt.rect.height;

    Vector3 currPosition = gameObject.transform.position;


    Vector3 finalPosition = new Vector3(currPosition.x, currPosition.y + 0.5f * height, currPosition.z);

    StartCoroutine (MoveTo.MoveOverSeconds (gameObject, finalPosition, .1f));

    //gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, finalPosition, Time.deltaTime);        

}


public void slightDown() {

    RectTransform rt = (RectTransform)gameObject.transform;
    

    float width = rt.rect.width;
    float height = rt.rect.height;

    Vector3 currPosition = gameObject.transform.position;
    Vector3 finalPosition = new Vector3(currPosition.x, currPosition.y - 0.5f * height, currPosition.z);

    //StartCoroutine (ToFront());


}


public void setFront() {

      mBack.transform.DORotate(new Vector3(0, 90, 0), 0f);
      mFront.transform.DORotate(new Vector3(0, 0, 0), 0f);
        mCardState = CardState.Front;

}



public void setBack() {

      mFront.transform.DORotate(new Vector3(0, 90, 0), 0f);
      mBack.transform.DORotate(new Vector3(0, 0, 0), 0f);
      mCardState = CardState.Back;

}


public void moveToBack() {

        StartCoroutine (ToBack());

}

public void moveToFront() {

    StartCoroutine (ToFront());

}

IEnumerator ToBack()
 {
  isActive = true;
  mFront.transform.DORotate(new Vector3(0, 90, 0), mTime);
  for (float i = mTime; i >= 0; i -= Time.deltaTime)
   yield return 0;
  mBack.transform.DORotate(new Vector3(0, 0, 0), mTime);
  isActive = false;
  mCardState = CardState.Back;

 }
 /// <summary>
 /// Turn to the front
 /// </summary>
 IEnumerator ToFront()
 {

//       yield return new WaitForSeconds(4f);

  //   InitCardFlip();

  isActive = true;
  mBack.transform.DORotate(new Vector3(0, 90, 0), mTime);
  for (float i = mTime; i >= 0; i -= Time.deltaTime)
   yield return 0;
  mFront.transform.DORotate(new Vector3(0, 0, 0), mTime);
  isActive = false;
  mCardState = CardState.Front;
 }


}