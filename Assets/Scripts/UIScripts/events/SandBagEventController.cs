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

        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(new WarningEntity(LocalizationManager.Instance.Get("sandbag_penalty_for", teamName), ""));


        //
    }





}
