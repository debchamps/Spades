using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CloseScoreboard : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    long clickedTime = 0;
    long MIN_DIFF_BETWEEN_CLICK_SECONDS = 1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        long currTime = System.DateTime.Now.Ticks;
        if((currTime -  clickedTime)/1000 > MIN_DIFF_BETWEEN_CLICK_SECONDS) {
            clickedTime = currTime;
            GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().disableScoreCard();
        } else {
            Debug.Log("Double click on close scoreboard");
        }


        // OnClick code goes here ...
    }

}
