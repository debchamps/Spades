using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void show() {

       StartCoroutine(MoveTo.MoveOverSeconds(gameObject, new Vector3(Screen.width/2,Screen.height/3,0), 1));


    }



}
