using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class AvatarSelection : MonoBehaviour, IPointerClickHandler {
    public PlayerPosition position;

    bool avatarEnabled = false;

    Dictionary<PlayerPosition, string> playerPositionToSelectedAvatar = new Dictionary<PlayerPosition, string>();


    public void setAvatarImages() {
        //

    }

    public void enable() {

        //Set the avataeData

        //GameObject.Find("avatarparent").transform.position = new Vector3(Screen.width/2, Screen.height/2, 1);
        //GameObject.Find("avatarempty").GetComponent<AvatarData>().set(position);

    }


    public void disable() {
        GameObject.Find("avatarparent").transform.position = new Vector3(Screen.width * 3, Screen.height/2, 1);
        GameObject.Find("avatarempty").GetComponent<AvatarData>().unset();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Avatar selected");

        if(!avatarEnabled)
        return;
        enable();

        //Move it by 2* Screen width.
        // OnClick code goes here ...
    }

}