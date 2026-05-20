using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class SeatSwitcher : MonoBehaviour
{


	// Use this for initialization
	void Start()
	{
        StartCoroutine(init());
	}

	// Update is called once per frame
	void Update()
	{
			
	}


    public IEnumerator init()
    {
        Debug.Log("Switching seats");
		bool isGamePlayOn = true;

        //Set it to Name to start with.

        setToPlayerNames();

        yield return new WaitForSeconds(5f);

        var match = GamePlay.matchState;

        if (match == null || match.getCurrentGameState() == null || !match.getCurrentGameState().gameStatus.Equals(SpadeGameState.GameStatus.IN_PROGRESS)) {
			isGamePlayOn = false;
        }

        Debug.Log("Status is " + match.getCurrentGameState().gameStatus);
		if(isGamePlayOn)
        {

            setToPlayerScores(0f);
			// Set it to score in 'x' seconds.
        }

		//Othwewise have the opponent feel.

    }

    public void setToPlayerNames()
    {
        foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
        {
            GameObject playerSeat = getSeat(pos);
            Debug.Log("Pos " + pos);
            Debug.Log("playerSeat is " + playerSeat + "");
            Debug.Log("playerSeat  chld is " + playerSeat.transform.Find("seatLabel") + "");
            //playerSeat.transform.Find("seatLabel").GetComponent<Image>().sprite = null;

            Resources.Load<Sprite>("cardgamecommon/blue_name");

            var s1 = playerSeat.transform.Find("seatLabel/score1").GetComponent<TMP_Text>();
            var s2 = playerSeat.transform.Find("seatLabel/score2").GetComponent<TMP_Text>();
            s1.text = "";
            s2.text = "";

            if (pos.Equals(PlayerPosition.SOUTH) || pos.Equals(PlayerPosition.NORTH))
            {
                playerSeat.transform.Find("seatLabel/seatName").GetComponent<Text>().text = LocalizationManager.Instance.Get("our_team");
            }
            else
            {
                playerSeat.transform.Find("seatLabel/seatName").GetComponent<Text>().text = LocalizationManager.Instance.Get("opponent");

            }

            if (pos.Equals(PlayerPosition.SOUTH) || pos.Equals(PlayerPosition.NORTH))
                playerSeat.transform.Find("seatLabel").GetComponent<Image>().sprite = Resources.Load<Sprite>("cardgamecommon/blue_name");
            if (pos.Equals(PlayerPosition.EAST) || pos.Equals(PlayerPosition.WEST))
                playerSeat.transform.Find("seatLabel").GetComponent<Image>().sprite = Resources.Load<Sprite>("cardgamecommon/red_name");
        }
    }

    public void setToPlayerScores(float delay)
    {
        StartCoroutine(setToPlayerScoresAsync(delay));

    }


     public IEnumerator setToPlayerScoresAsync(float delay) {

        yield return new WaitForSeconds(delay);

        var match = GamePlay.matchState;

        foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
        {
            GameObject playerSeat = getSeat(pos);
            playerSeat.transform.Find("seatLabel/seatName").GetComponent<Text>().text = "";
            var s1 = playerSeat.transform.Find("seatLabel/score1").GetComponent<TMP_Text>();
            var s2 = playerSeat.transform.Find("seatLabel/score2").GetComponent<TMP_Text>();
            s1.text = match.getCurrentGameState().tricksWinnerCount[pos].ToString(); ;
            s2.text = match.getCurrentGameState().callBreakBidding.callBreakBiddingData.playerBidAmount[pos].ToString();

            if(pos.Equals(PlayerPosition.SOUTH) || pos.Equals(PlayerPosition.NORTH))
                playerSeat.transform.Find("seatLabel").GetComponent<Image>().sprite = Resources.Load<Sprite>("cardgamecommon/blue_score");
            if (pos.Equals(PlayerPosition.EAST) || pos.Equals(PlayerPosition.WEST))
                playerSeat.transform.Find("seatLabel").GetComponent<Image>().sprite = Resources.Load<Sprite>("cardgamecommon/red_score");

        }

    }


	public static GameObject getSeat(PlayerPosition pos)
    {
		if(pos.Equals(PlayerPosition.SOUTH))
        {
			return GameObject.Find("playerSouthSeat");
        }
        if (pos.Equals(PlayerPosition.NORTH))
        {
            return GameObject.Find("playerNorthSeat");
        }
        if (pos.Equals(PlayerPosition.EAST))
        {
            return GameObject.Find("playerEastSeat");
        }
            return GameObject.Find("playerWestSeat");
    }



    public void onGameComplete() {

		//Wait for 'x' seconds and change it. when the scoreboards have come up in a way.

		//Change it back to 
	}

    public void onBiddingComplete()
    {
		//Change it to scoremode.
    }


}

