using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdLog : MonoBehaviour
{


    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    public void logOpened() {
        DebugLog.Log("Ad opened.");

    }

    public void logError(string msg) {
        DebugLog.Log("Failed to load ad because of " + msg);

    }

    public void interestialOpened() {
        DebugLog.Log("InterstitialGameEnd opened");
    }

    public void interestialClosed() {
        DebugLog.Log("InterstitialGameEnd closed");
        GamePlay.startNextMatchStatic();        
    }

    public void interestialFailed(string msg) {
        DebugLog.Log("InterstitialGameEnd failed :  " + msg);
        
        GamePlay.startNextMatchStatic();        
    }

}
