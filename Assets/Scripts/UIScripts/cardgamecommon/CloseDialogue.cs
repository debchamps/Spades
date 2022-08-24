using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseDialogue : MonoBehaviour , IPointerClickHandler
{
    // Start is called before the first frame update

    public GameObject dialogue;

    public CloseMode closeMode;
    public enum CloseMode {
        MOVE_AWAY,
        DISAPPEAR

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {


        if(closeMode == null || closeMode.Equals(CloseMode.MOVE_AWAY))
        StartCoroutine(MoveTo.MoveOverSeconds(dialogue, new Vector3(Screen.width/2, -3f* Screen.height,0), 1));
        else
        dialogue.transform.position = new Vector3(Screen.width/2, -3f* Screen.height,0);
        //Move it by 2* Screen width.
        // OnClick code goes here ...
    }

}
