using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiatePlayerIconScript : MonoBehaviour
{

    public GameObject[] faceObjects;
    // Start is called before the first frame update
    void Start()
    {
        if(faceObjects != null) {
            for(int i=0;i<faceObjects.Length; i++) {
                faceObjects[i].GetComponent<OpenPlayerProfile>().set();

            }

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
