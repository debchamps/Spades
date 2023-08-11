using UnityEngine;

public class AdManager {

    public static bool isBannerAdEnabled() {


        

        float deviceAspect =  DeviceTypeChecker.getAspectRatio();
        Debug.Log("deviceAspect is " + deviceAspect);
        if(deviceAspect < 1.65f) {
            return false;
        }

        return true;
        

    }

    
    public static bool isInterstitialAdEnabled() {

        return true;
        //return true;

    }

}