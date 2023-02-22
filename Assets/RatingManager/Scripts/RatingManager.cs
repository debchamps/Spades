using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

/**
    Rating Manager. Redirects to Google PlayStore / IOS App Store when user submits an internal review.
*/
public class RatingManager : MonoBehaviour {


    public string gameIdAndroid;
    public string gameIdIOS;

    public string name;

    static bool ratingSet = false;

    static int ratingValue;

    public int target  = 5;

    public bool CheckInternetConnection(){
        return !(Application.internetReachability == NetworkReachability.NotReachable);
    }


    public void increment() {
        int calledTime = PlayerPrefs.GetInt("shouldOpenRatingManager", 0);

        PlayerPrefs.SetInt("shouldOpenRatingManager", calledTime + 1);
        PlayerPrefs.Save();
    }


    public bool shouldOpen() {

        if(!CheckInternetConnection())
            return false;



        int noOfCall = PlayerPrefs.GetInt("shouldOpenRatingManager" , 0);

        int noOfTimeOpened = PlayerPrefs.GetInt("openCount", 0);

        if(noOfTimeOpened <= 0 && noOfCall > target) {
            return true;
        }
        return false;

        
        //If number of game > 50 Ask for review and Ask for rating. 
        // If number of game > 150 Ask for review and Ask for rating 



    }

    public void open() {

        GameObject.Find("ratingsubmit").GetComponent<Image>().DOFade(1f, 0f);
        GameObject.Find("ratingsubmit/buttonText").GetComponent<Text>().DOFade(1f, 0f);

        GameObject.Find("thanksnote").GetComponent<Image>().DOFade(0f, 0f);
        GameObject.Find("thanksnote/desc").GetComponent<Text>().DOFade(0f, 0f);
        GameObject.Find("thanksnote").GetComponent<Image>().transform.DOScale(0f, 0f);
        Vector3 existingScale = GameObject.Find("ratingparent").transform.localScale;
        GameObject.Find("ratingparent").transform.localScale = new Vector3(0f, 0f, 1f);
        GameObject.Find("ratingparent").transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        GameObject.Find("ratingparent").transform.DOScale(existingScale, .3f).SetDelay(1f);
        int noOfTimeOpened = PlayerPrefs.GetInt("openCount", 0);
        PlayerPrefs.SetInt("openCount", noOfTimeOpened + 1);
        PlayerPrefs.Save();

    }



    public void close() {
        GameObject.Find("ratingparent").transform.position = new Vector3(5 * Screen.width, Screen.height/2, 0);
        ratingSet = false;
        ratingValue = 0;
        
    }


    public void setRating(int rating) {
        ratingSet = true;
        ratingValue = rating;
        Debug.Log("Rating is ratingSet " + ratingSet + " and value is " + ratingValue);
    }


    public IEnumerator showThankyouAndClose() {

        GameObject.Find("ratingsubmit").GetComponent<Image>().DOFade(0f, 0f);
        GameObject.Find("ratingsubmit/buttonText").GetComponent<Text>().DOFade(0f, 0f);

        GameObject.Find("thanksnote").GetComponent<Image>().transform.DOScale(1f, 0f);
        GameObject.Find("thanksnote").GetComponent<Image>().DOFade(1f, 0.2f);
        GameObject.Find("thanksnote/desc").GetComponent<Text>().DOFade(1f, 0.2f);

        yield return new WaitForSeconds(3f);

        close();

        yield return new WaitForSeconds(.1f);


        #if UNITY_IPHONE
          Application.OpenURL("market://details?id=" + Application.identifier);
        #endif

        #if UNITY_ANDROID
            androidReview();

        //Application.OpenURL("market://details?id=" + gameIdAndroid);
         #endif

        yield return new WaitForSeconds(.3f);


    }


    public void androidReview()
    {
        GameObject.Find("ratingparent").GetComponent<AppReview>().openReview();

    }

    public void submit() {
        Debug.Log("Rating is ratingSet " + ratingSet + " and value is " + ratingValue);
        if(ratingSet && ratingValue >=4) {

            StartCoroutine(showThankyouAndClose());
            //Application.OpenURL("market://details?id=" + Application.identifier);


        } else {
            close();

        }


        GameObject.Find("ratingparent").GetComponent<RatingSetter>().unsetAll();


    }

}