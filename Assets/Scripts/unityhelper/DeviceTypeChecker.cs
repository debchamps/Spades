using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
public enum ENUM_Device_Type
{
    Tablet,
    Phone
}
 
public class DeviceTypeChecker
{
    public static bool isTablet;
 
    private static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
 
        return diagonalInches;
    }
 
     public static float getAspectRatio() {
         float aspectRatio = (float)Mathf.Max(Screen.width, Screen.height) /(float) Mathf.Min(Screen.width, Screen.height);
        return aspectRatio;
     }
     
    public static ENUM_Device_Type GetDeviceType()
    {
        Debug.Log("DeviceType " + " Start");
    #if UNITY_IOS
    bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        Debug.Log("DeviceType " + " Start deviceIsIpad" + deviceIsIpad);
            if (deviceIsIpad)
            {
                return ENUM_Device_Type.Tablet;
            }
            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            Debug.Log("DeviceType " + " Start deviceIsIphone" + deviceIsIphone);
            if (deviceIsIphone)
            {
                return ENUM_Device_Type.Phone;
            }
        #endif
 
        float aspectRatio = (float)Mathf.Max(Screen.width, Screen.height) /(float) Mathf.Min(Screen.width, Screen.height);
        Debug.Log("Screen.width " + Screen.width + " Screen.height" + Screen.height);
        Debug.Log("aspectRatio is : " + aspectRatio + " DeviceDiagonalSizeInInches() " + DeviceDiagonalSizeInInches());
        bool isTablet = (DeviceDiagonalSizeInInches() > 4.9f && aspectRatio < 1.5f);
 
        if (isTablet)
        {
            return ENUM_Device_Type.Tablet;
        }
        else
        {
            return ENUM_Device_Type.Phone;
        }
        //return ENUM_Device_Type.Phone;

        return ENUM_Device_Type.Phone;

    }
}