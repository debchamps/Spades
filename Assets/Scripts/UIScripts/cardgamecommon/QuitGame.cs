using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void quit() {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void close() {
        Debug.Log("Closing");
        GameObject quitGameObj = GameObject.Find("quitgame");
        quitGameObj.transform.position = new Vector3(Screen.width* 4 , Screen.height/2, 0);        

    }


    public void show() {
        GameObject quitGameObj = GameObject.Find("quitgame");
        quitGameObj.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);        
    }


}
