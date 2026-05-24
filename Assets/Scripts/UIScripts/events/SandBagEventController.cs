using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;



public class SandBagEventController : MonoBehaviour
{


    [System.Serializable]
    public class SandBagScoreEvent : UnityEvent<int>
    {
        
    }


    public class SandBagCompleteEvent : UnityEvent<int>
    {
    }


    public static SandBagCompleteEvent sandBagCompleteEvent;
    public static SandBagScoreEvent sandBagScoreEvent;

    // Start is called before the first frame update
    void Start()
    {
        if(sandBagScoreEvent == null)
            sandBagScoreEvent = new SandBagScoreEvent();


        if(sandBagCompleteEvent == null)
            sandBagCompleteEvent = new SandBagCompleteEvent();

        sandBagScoreEvent.AddListener(sandBagScoreEventLogic);
        sandBagCompleteEvent.AddListener(sandBagCompleteEventLogic);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void InvokeSandBagScoreEvent(int team) {
        if(sandBagScoreEvent == null)
            sandBagScoreEvent = new SandBagScoreEvent();

        Debug.Log("Invoked Sandbag with team " + team);
        sandBagScoreEvent.Invoke(team);
    }

    public static void InvokeSandBagCompleteEvent(int team) {
        if(sandBagCompleteEvent == null)
            sandBagCompleteEvent = new SandBagCompleteEvent();
        sandBagCompleteEvent.Invoke(team);
    }



    public static void sandBagScoreEventLogic(int teamNo) {
        //Can add slight delay. But  it is ok for now.
        Debug.Log("In sandBagScoreEventLogic " + teamNo);

        //Animate the score and increase it by one.
        int sandBagScore = 0;
        if(teamNo == 1) {
            sandBagScore = GamePlay.matchState.cumulativeSandbagTeam1 ;
        } else {
            sandBagScore = GamePlay.matchState.cumulativeSandbagTeam2 ;

        }
            
        Debug.Log("In sandBagScoreEventLogic sandBagScore " + sandBagScore);

        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().scaleUpAndDown(GameObject.Find("sandbag" + teamNo.ToString())  , .6f, 1.5f, 1.5f);
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("sandbag" + teamNo.ToString())  , sandBagScore.ToString(), 1.3f);


    }


    public static void sandBagCompleteEventLogic(int teamNo) {

        int sandBagScore = 0;
        if(teamNo == 1) {
            sandBagScore = GamePlay.matchState.cumulativeSandbagTeam1;
        } else {
            sandBagScore = GamePlay.matchState.cumulativeSandbagTeam2;

        }
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().setTextWithDelay(GameObject.Find("sandbag" + teamNo.ToString())  , sandBagScore.ToString(), 1.3f);
        string teamName = LocalizationManager.Instance.Get("our_team_short");
        if(teamNo == 2) {
            teamName = LocalizationManager.Instance.Get("opponent");
        }

        // BUGFIX (user-reported): the warning popup used to read a locale
        // string with the penalty amount hardcoded as "-50", which is the
        // half-game (250-target) value. For the default 500-target match
        // the actual deduction is −100, so the UI was wildly inconsistent
        // with what got applied. The locale strings now use {0} for the
        // amount and {1} for the team name; we feed the correct value from
        // the central sandbagPenalty() helper so the message always matches
        // what hit the scorecard.
        int penaltyAmount = GamePlay.matchState.getCurrentGameState().sandbagPenalty();
        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(
            new WarningEntity(LocalizationManager.Instance.Get("sandbag_penalty_for", penaltyAmount, teamName), ""));


        //
    }





}
