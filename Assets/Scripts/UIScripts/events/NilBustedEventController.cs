using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;



public class NilBustedEventController : MonoBehaviour
{


    [System.Serializable]
    public class PlayerPositionEvent : UnityEvent<PlayerPosition>
    {
    }

    public static PlayerPositionEvent nilBustedEvent;

    // Start is called before the first frame update
    void Start()
    {
        if(nilBustedEvent == null)
            nilBustedEvent = new PlayerPositionEvent();

        nilBustedEvent.AddListener(nilBustedLogic);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void InvokeEvent(PlayerPosition position) {
        if(nilBustedEvent == null)
            nilBustedEvent = new PlayerPositionEvent();
        nilBustedEvent.Invoke(position);
    }


    public static void nilBustedLogic(PlayerPosition pos) {
        //Can add slight delay. But  it is ok for now.

        GameObject.Find("southbidvalv2").GetComponent<TMP_Text>().text = LocalizationManager.Instance.Get("nil_busted_text");
        //Also add a notification in the top.
        GameObject.Find("ScriptEmpty").GetComponent<WarningScript>().showWarningAndDisappear(new WarningEntity(LocalizationManager.Instance.Get("nil_busted_for", pos.ToString()), ""));

        //Add a delay. and a slight animation.
        GameObject.Find(PlayerPositionHelper.getName(pos).ToLower() + "nilBubble").GetComponent<Image>().sprite = Resources.Load<Sprite>("bray/nil_red");


    }




}
