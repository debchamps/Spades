using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private InterstitialAd _interstitialAd;
    BannerView _bannerView;

    void Start()
    {
        setupAds();
    }


    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitIdInter = "ca-app-pub-1368766915086140/4463076032";
#elif UNITY_IPHONE
              private string _adUnitIdInter = "ca-app-pub-1368766915086140/9957201010";
#else
              private string _adUnitIdInter = "unused";
#endif



    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-1368766915086140/8083876403";
#elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
      private string _adUnitId = "unused";
#endif


    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
    }


    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }




    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitIdInter, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }


    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }


    public static bool isBannerAdEnabled()
    {




        float deviceAspect = DeviceTypeChecker.getAspectRatio();
        Debug.Log("deviceAspect is " + deviceAspect);
        if (deviceAspect < 1.65f)
        {
            return false;
        }

        return true;


    }


    public static bool isInterstitialAdEnabled()
    {

        return true;
        //return true;

    }


    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }



    public  void setupAds()
    {
        List<string> deviceIds = new List<string>();
        deviceIds.Add("");
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = deviceIds
        };
        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize((initStatus) => {
            DebugLog.Log("Initialized MobileAds with status " + initStatus);

            if (AdManager.isInterstitialAdEnabled())
            {
                //InterstitialAdGameObject interstitialAd = MobileAds.Instance.GetAd<InterstitialAdGameObject>("Interstitial Ad");
                //interstitialAd.LoadAd();
                LoadInterstitialAd();
            }

        });



    }

}
