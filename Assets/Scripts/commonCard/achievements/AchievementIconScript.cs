using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementIconScript : MonoBehaviour,  IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
 


    public void onSelected() {
        GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().enable();
        AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("AchievementIconScript selected");
        onSelected();
        // OnClick code goes here ...
    }

}
