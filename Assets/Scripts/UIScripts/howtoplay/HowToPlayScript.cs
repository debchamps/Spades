
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class HowToPlayScript : MonoBehaviour {
    static int pageNumber = 1;

    static bool open = false;

    public int totalPage;

    // Text-based help content loaded from JSON
    private HelpContent _helpContent;

    public static Dictionary<int, string> pageNumberToHeader = new Dictionary<int, string>();


    void Start() {
        totalPage = 7;
        _helpContent = HelpPopupManager.LoadContent("Localization/spades_help");
    }

    void Update() {
        if (open) {

             if(AnimationUtil.isClickedOutside(GameObject.Find("howtoplay")) && AnimationUtil.isClickedOutside(GameObject.Find("bottomBar/help"))) {
                 close();
             }

        }
     }

    public void show() {
        if(!open) {
            loadHeader();
            open = true;
            GameObject howtoplayGO = GameObject.Find("howtoplay");
            howtoplayGO.transform.position = new Vector3(Screen.width/2, -Screen.height/2, 1f);

            // Inject TMP text view into the panel (idempotent)
            HelpPopupManager.Ensure(howtoplayGO);

            AnimationUtil.openDarkBkg(0.5f);
            howtoplayGO.transform.DOMove(new Vector3(Screen.width/2, Screen.height/2, 1f), .5f);
            GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(0, 0f);
            GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(1, 0f);
            AudioManagerScript.play(AudioClipType.DEFAULT_NOTIFICATION);
            setPage();
        } else {
            close();
        }

    }

    public void close() {
        open = false;
        GameObject.Find("howtoplay").transform.DOMove(new Vector3(Screen.width/2, -Screen.height/2, 1f), .25f);
        AnimationUtil.closeDarkBkg();
        pageNumber = 1;
    }


    public void setPageAnimate() {
        setPage();
    }

    public void setPage() {
        GameObject howtoplayGO = GameObject.Find("howtoplay");

        // Update the page header text
        if (pageNumberToHeader.ContainsKey(pageNumber))
            GameObject.Find("howtoplay/header").GetComponent<Text>().text = pageNumberToHeader[pageNumber];

        // Render localized text content
        HelpPopupManager.RenderPage(howtoplayGO, pageNumber, totalPage, _helpContent);
    }

    public void next() {
        if(pageNumber < totalPage) {
            pageNumber++;
            setPage();
            if(pageNumber >= 2) {
                GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(1, .1f);
            }
            if(pageNumber == totalPage) {
                GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(0, .1f);
            }
        }
    }

    public void previous() {
        if(pageNumber > 1) {
            pageNumber--;
            setPage();
            if(pageNumber == 1) {
                GameObject.Find("howtoplay/previoushelpicon").GetComponent<Image>().DOFade(0, .1f);
            }
            if(pageNumber <= totalPage -1) {
                GameObject.Find("howtoplay/nexthelpicon").GetComponent<Image>().DOFade(1, .1f);
            }
        }
    }


    void loadHeader() {
        pageNumberToHeader[1] = LocalizationManager.Instance.Get("howtoplay_objective");
        pageNumberToHeader[2] = LocalizationManager.Instance.Get("howtoplay_bidding");
        pageNumberToHeader[3] = LocalizationManager.Instance.Get("howtoplay_nil_bid");
        pageNumberToHeader[4] = LocalizationManager.Instance.Get("howtoplay_scoring");
        pageNumberToHeader[5] = LocalizationManager.Instance.Get("howtoplay_gameplay");
        pageNumberToHeader[6] = LocalizationManager.Instance.Get("howtoplay_gameplay_trump");
        pageNumberToHeader[7] = LocalizationManager.Instance.Get("howtoplay_sandbag_penalty");
    }


}
