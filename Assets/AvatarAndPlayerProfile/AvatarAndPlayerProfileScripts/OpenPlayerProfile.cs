using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**


*/
public class OpenPlayerProfile : MonoBehaviour
{

    // Seat Object
    // Face Image

    public GameObject playerImage;
    public string playerId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void set() {
        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().playerId = playerId;
        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().externalPlayerImage = playerImage;

        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().setImageForPlayerId(playerId);
    }


    public void open() {

        if(GamePlay.IS_DEBUG)
            return;        

        //PlayerProfileScript.externalFace = face;
        //PlayerProfileScript.externalSeat = seat;
        //PlayerProfileScript.externalFace = face;
        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().playerId = playerId;
        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().externalPlayerImage = playerImage;
        GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().open();

    }
}
