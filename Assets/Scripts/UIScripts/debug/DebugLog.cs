using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLog : MonoBehaviour
{
    static bool  debugEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void Log(string tval, bool shouldLog) {
        if(debugEnabled && shouldLog)
            GameObject.Find("debugtext").GetComponent<Text>().text = tval + "           " + GameObject.Find("debugtext").GetComponent<Text>().text ;
    }

    public static void Log(string tval) {
        if(debugEnabled)
            GameObject.Find("debugtext").GetComponent<Text>().text = tval + "           " + GameObject.Find("debugtext").GetComponent<Text>().text ;
    }
}
