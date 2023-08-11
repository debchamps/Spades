using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class RestartScript : MonoBehaviour
{

    List<GameObject> panels = new List<GameObject>();

    public class GameDetail
    {
        public Image icon;
        public string name;
    }


    public int[] gameDetails1;
    public Image[] gameImages;

    public GameDetail[] gameDetails;

    Vector3 originalPosition;
    static bool open = false;


    public void restart()
    {
        GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().restartMatch();
        close();


    }


    void Update()
    {
        if (open)
        {

            if (AnimationUtil.isClickedOutside(GameObject.Find("restartgame")) && AnimationUtil.isClickedOutside(GameObject.Find("bottomBar/icon4")))
            {
                Debug.Log("Closing restartgame");
                close();
            }


        }
    }


    public void close()
    {
        Vector3 closingPosition = new Vector3(Screen.width / 2, -Screen.height * 0.2f, 1f);
        GameObject.Find("restartgame").transform.DOMove(closingPosition, .2f);
        Debug.Log("Closing restartgame with open = " + open);
        AnimationUtil.closeDarkBkg();
        open = false;

    }

    public void show()
    {
        Debug.Log("In restartgame with open = " + open);
        bool canRestart = GameObject.Find("ScriptEmpty").GetComponent<GamePlay>().canRestartMatch();

        if (!canRestart)
            return;


        if (!open)
        {
            open = true;
            Vector3 positionToMove = getPositionToMove();
            float height = RectTransformExt.GetWorldRect(GameObject.Find("bottomBar").GetComponent<RectTransform>()).height;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(GameObject.Find("restartgame").transform.DOMove(new Vector3(positionToMove.x, positionToMove.y + height / 8, 1f), .2f));
            sequence.Append(GameObject.Find("restartgame").transform.DOMove(positionToMove, .05f));
            sequence.Play();

            AnimationUtil.openDarkBkg(0.3f);
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

            if (panels.Count == 0)
            {
                fillPanels();
            }

        }
        else
        {
            close();
        }


        //depending on name of 

    }

    void fillPanels()
    {

        panels.Add(GameObject.Find("restartgame"));
        //panels.Add(GameObject.Find("icon4"));

    }

    Vector3 getPositionToMove()
    {
        originalPosition = GameObject.Find("bottomBar").transform.position;
        float height = RectTransformExt.GetWorldRect(GameObject.Find("bottomBar").GetComponent<RectTransform>()).height;
        float heightRestart = RectTransformExt.GetWorldRect(GameObject.Find("restartgame").GetComponent<RectTransform>()).height;
        float y = GameObject.Find("bottomBar").transform.position.y + height + heightRestart / 2;
        return new Vector3(Screen.width / 2, Screen.height/2, 1f);
        //return new Vector3(Screen.width / 2, y, 1f);
        //Formulat 
        //
        //x = Screen.width.2 y = bottomBar Top + Bottom bar height * .2f


    }



}