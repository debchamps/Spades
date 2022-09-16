using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using UnityEngine.Analytics;
using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;

public class AdjustGameObjectResolution {


    public static void adjust() {

        ENUM_Device_Type device_Type =  DeviceTypeChecker.GetDeviceType();
        Debug.Log("Device type is : " + device_Type);

        Rect topObjRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("TopObj").transform);
        bool testGenericAspect2 = true;
        float defaultAspect = 16f/9, deviceAspect =  DeviceTypeChecker.getAspectRatio();
            //GameObject.Find("BiddingPanel").transform.localScale = new Vector3(.8f, .6f, 1f);


        // device 2.25 default 1.77 yAspect = 1.27 xAspect 2.01  cardAspect   
            float yAspect = deviceAspect/defaultAspect;
            float diff = Mathf.Max(deviceAspect, defaultAspect) - Mathf.Min(deviceAspect, defaultAspect);
            float xAspect = 0f, cardAspectSmall = 0f, cardAspect = 0f;
            if(yAspect < 1) {

                 xAspect = (deviceAspect + diff/2)/defaultAspect;
                 cardAspect = (xAspect+ yAspect)/2;
                 cardAspectSmall = (deviceAspect + diff*.66f)/defaultAspect;
            } else {

                 xAspect = (deviceAspect - diff/2)/defaultAspect;
                 cardAspect = (xAspect+ yAspect)/2;
            
                 cardAspectSmall = (deviceAspect + diff*.66f)/defaultAspect;
                 cardAspectSmall = yAspect;
            }
        if(testGenericAspect2 && (deviceAspect< 1.5f || deviceAspect > 1.9f))  {

            Debug.Log("deviceAspect" + deviceAspect + "xAspect: " + xAspect + " yAspect: " + yAspect + ", cardAspect " + cardAspect + " cardAspectSmall: " + cardAspectSmall);
            GameObject.Find("activePlayerCards").transform.localScale = new Vector3( 1/cardAspect, 1/cardAspectSmall, 1f);

            GameObject.Find("playedCard").transform.localScale = new Vector3(1/cardAspect, 1/cardAspectSmall, 1f);
            multiplyLocalScale(GameObject.Find("achievementparent") ,new Vector3(1/yAspect, 1/yAspect, 1f) );
            //multiplyLocalScale(GameObject.Find("achievementparent") ,new Vector3(1/yAspect, 1f, 1f) );
            multiplyLocalScale(GameObject.Find("ScoreCardV2") ,new Vector3(1/yAspect, 1/xAspect, 1f) );
            multiplyLocalScale(GameObject.Find("ratingparent") ,new Vector3(1/yAspect, 1/xAspect, 1/yAspect) );
             multiplyLocalScale(GameObject.Find("howtoplay") ,new Vector3(1/yAspect, 1/xAspect, 1/yAspect) );
             multiplyLocalScale(GameObject.Find("showlasthand") ,new Vector3(1/yAspect, 1/xAspect, 1/yAspect) );
           if(deviceAspect> 1.5f)
            multiplyLocalScale(GameObject.Find("settingsparent") ,new Vector3(1/yAspect, 1/yAspect, 1) );
            else 
            multiplyLocalScale(GameObject.Find("settingsparent") ,new Vector3(1/xAspect, 1/xAspect, 1) );

            multiplyLocalScale(GameObject.Find("playwarning") ,new Vector3(1/xAspect, 1/xAspect, 1f) );
            multiplyLocalScale(GameObject.Find("quitgame") ,new Vector3(1/yAspect, 1/xAspect, 1f) );
            multiplyLocalScale(GameObject.Find("restartgame") ,new Vector3(1/yAspect, 1/xAspect, 1f) );

            multiplyLocalScale(GameObject.Find("bottomBar") ,new Vector3(1/yAspect, 1/xAspect, 1/yAspect) );
            //multiplyLocalScale(GameObject.Find("playwarningv2") ,new Vector3(1/yAspect, 1/xAspect, 1/yAspect) );

            float currentWidth = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("playwarningv2").transform).width;

            float yScaleFactor = Screen.width/currentWidth;
            //multiplyLocalScale(GameObject.Find("playwarningv2") ,new Vector3(1/(Screen.width * .8f/currentWidth), 1/xAspect, 1/yAspect) );
            multiplyLocalScale(GameObject.Find("playwarningv2") ,new Vector3(1/yAspect, 1/yAspect, 1f) );

            if(deviceAspect > 1.9f)
            multiplyLocalScale(GameObject.Find("biddingparent") ,new Vector3(1/yAspect, 1/yAspect, 1f) );

            Debug.Log("Changing avatar scale from " + GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().scale  
            + "  to " + 1/xAspect);
            GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().scale = GameObject.Find("avatarParent").GetComponent<PlayerProfileScript>().scale * 1/xAspect;
            //GameObject.Find("playwarning").transform.position = new Vector3(Screen.width/2 - warningRect.width/2, warnpos.y, warnpos.z);
        }


        if(deviceAspect < 1.5f) {
            
            Vector3 currPos = GameObject.Find("TopObj").transform.position;
            float updatedWidth = (topObjRect.height + topObjRect.width)/2;
            float shiftX = (topObjRect.width - updatedWidth)/2;
            Vector3 currWestPos = GameObject.Find("playerWestSeat").transform.position;
            Vector3 currWestDeckPos = GameObject.Find("deckwest").transform.position;
            GameObject.Find("playerWestSeat").transform.position = new Vector3(currWestPos.x + shiftX,currWestPos.y, currWestPos.z );
             GameObject.Find("deckwest").transform.position = new Vector3(currWestDeckPos.x + shiftX,currWestDeckPos.y, currWestDeckPos.z );
            Vector3 currEastDeckPos = GameObject.Find("deckeast").transform.position;
            Vector3 currEastPos = GameObject.Find("playerEastSeat").transform.position;
            GameObject.Find("playerEastSeat").transform.position = new Vector3(currEastPos.x - shiftX,currEastPos.y, currEastPos.z );
             GameObject.Find("deckeast").transform.position = new Vector3(currEastDeckPos.x - shiftX,currEastDeckPos.y, currEastDeckPos.z );
           //multiplyLocalScale(GameObject.Find("TopObj") ,new Vector3(1 + (deviceAspect - defaultAspect)/deviceAspect, 1f, 1f) );
            multiplyLocalScale(GameObject.Find("playerSouthSeat") ,new Vector3(1/cardAspect, 1/cardAspect, 1f) );
            multiplyLocalScale(GameObject.Find("playerNorthSeat") ,new Vector3(1/cardAspect, 1/cardAspect, 1f) );
            multiplyLocalScale(GameObject.Find("playerEastSeat") ,new Vector3(1/cardAspect, 1/cardAspect, 1f) );
            multiplyLocalScale(GameObject.Find("playerWestSeat") ,new Vector3(1/cardAspect, 1/cardAspect, 1f) );

            multiplyLocalScale(GameObject.Find("turnWarning") ,new Vector3(1/yAspect, 1/yAspect, 1f) );
            Rect warnRect = RectTransformExt.GetWorldRect((RectTransform)GameObject.Find("turnWarning").transform);
            Vector3 warnPos = GameObject.Find("turnWarning").transform.position;

            GameObject.Find("turnWarning").transform.position = new Vector3(-Screen.width/2 + warnRect.width, warnPos.y, 1);
            //multiplyLocalScale(GameObject.Find("playwarningv2") ,new Vector3(1/cardAspect, 1/yAspect, 1f) );

        }

    }

    public static void multiplyLocalScale(GameObject obj, Vector3 scale) {
            Vector3 currScale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(currScale.x * scale.x, currScale.y * scale.y, currScale.z * scale.z);

    }

}