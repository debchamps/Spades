using System.Collections;
using UnityEngine;
#if UNITY_ANDROID 
using Google.Play.Review;
#endif

public class AppReview : MonoBehaviour
{

    #if UNITY_ANDROID 

    private ReviewManager _reviewManager;
    PlayReviewInfo _playReviewInfo;
    #endif




    private void Awake()
    { 
        
        #if UNITY_ANDROID 

        _reviewManager = new ReviewManager();
        #endif
           
         //StartCoroutine(review());
        
    }

    public void openReview() {
        #if UNITY_ANDROID 
            StartCoroutine(review());
        #endif

    }

    IEnumerator review()
    {

        #if UNITY_ANDROID

        yield return new WaitForSeconds(0.1f);
        DebugLog.Log("\nInfo1 requestFlowOperation");

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
       // DebugLog.Log("\nInfo2 requestFlowOperation");
        yield return requestFlowOperation;
         DebugLog.Log("\nInfo3 requestFlowOperation");
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().

            //Debug.Log("Error in requestFlowOperation1 " + requestFlowOperation.Error.ToString());
            //DebugLog.Log("\nError in requestFlowOperation1 " + requestFlowOperation.Error.ToString());
            yield break;
        }
        DebugLog.Log("\nInfo4 requestFlowOperation");
        _playReviewInfo = requestFlowOperation.GetResult();
         //DebugLog.Log("\nInfo5 requestFlowOperation");
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
         DebugLog.Log("\nInfo6 requestFlowOperation");
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log("Error in requestFlowOperation2 " + requestFlowOperation.Error.ToString());
            DebugLog.Log("\nError in requestFlowOperation2 " + requestFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
         DebugLog.Log("\nInfo7 requestFlowOperation");
        #endif

        yield return new WaitForSeconds(0f);


    }

}
