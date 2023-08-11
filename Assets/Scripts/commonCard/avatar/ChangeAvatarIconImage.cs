using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeAvatarIconImage : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


    public void onSelected() {
        
        //Close the dialogue.
        //
        AvatarData avatarData = GameObject.Find("avatarempty").GetComponent<AvatarData>();





        GameObject obj = GameObject.Find("playersouthavatar");
        obj.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;

        AnimationUtil.scaleUpAndDown(obj, .5f, .5f, 1.1f);

        //Now scale up and down slightly
        GameObject.Find("avatarparent").transform.position = new Vector3(Screen.width * 3, Screen.height/2, 1);
        GameObject.Find("avatarempty").GetComponent<AvatarData>().unset();
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected();
        // OnClick code goes here ...
    }
}
