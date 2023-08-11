using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class AchievementScript : MonoBehaviour
{
    Vector3 achievementLocalScale = new Vector3(0.8f,0.8f, 1f);

    AchievementManager achievementManager = new AchievementManager();
    string TAG = "AchievementDao";

    static bool open = false;

    void Update() {
        if(open) {
            
             if(AnimationUtil.isClickedOutside(GameObject.Find("achievementparent")) && AnimationUtil.isClickedOutside(GameObject.Find("bottomBar/icon4"))) {
                 close();
             }

        }
     }

    public void initiate() {
       //if(achievementLocalScale != null)
       //     achievementLocalScale = GameObject.Find("achievementparent").transform.localScale;
    }

    public void enable() {

        if(!open) {
            open = true;
            closeOthers();
            loadAchievements();
            GameObject.Find("achievementparent").transform.position = new Vector3(Screen.width/2, Screen.height/2 , 1);

            AnimationUtil.openDarkBkg(.3f);
            AnimationUtil.openDialogue(GameObject.Find("achievementparent"), achievementLocalScale.x, .3f);

        } else {
            close();
        }
        //GameObject.Find("achievementparent").transform.DOScale(achievementLocalScale,.25f);
    }

    public void close() {

        open = false;
        AnimationUtil.closeDialogue(GameObject.Find("achievementparent"));

        //GameObject.Find("achievementparent").transform.position = new Vector3(Screen.width * 5, Screen.height/2 , 1);

    }


    public void closeOthers() {
        GameObject.Find("ScriptEmpty").GetComponent<ScoreboardScript>().close();
        GameObject.Find("ScriptEmpty").GetComponent<SettingsScript>().close();
        //GameObject.Find("ScriptEmpty").GetComponent<AchievementScript>().close();
    }



    public void loadAchievements() {
        //Load it from PlayerPref.

    /*
    Achievement achievement1 = new Achievement("BRAY_WIN", "Win 500 game", 500, 0);
    Achievement achievement2 = new Achievement("BRAY_GAME_NO_TRICK_WON", "Win 100 Rounds without winning a trick", 100, 0);
    Achievement achievement3 = new Achievement("BRAY_GAME_ZERO_POINT_WON", "Win 200 Round with scoring 0 point", 200, 0);
    Achievement achievement4 = new Achievement("BRAY_MATCH_WITH_LESS_THAN_20", "Score less than 10 points in 50 point game", 50, 0);

        List<Achievement> achievements = new List<Achievement>();
        achievements.Add(achievement1);
        achievements.Add(achievement2);
        achievements.Add(achievement3);
        achievements.Add(achievement4);*/

        Debug.Log(TAG + " Loading achievement data ");

        AchievementData  data = achievementManager.getAchievementData();

        if(data == null) {
            set(new List<Achievement>());
        } else {
            set(data.achievements);
        }

    }

    private void set(List<Achievement> achievements) {

        for(int i=1;i<=achievements.Count;i++) {
            Achievement achievement = achievements[i-1];
            GameObject achv = GameObject.Find("achievementgroup" + i.ToString()); 

            putContent(achv, achievement);

        }        
    }


    private void putContent(GameObject banner, Achievement achievement) {
        banner.transform.Find("description").GetComponent<Text>().text = achievement.description;
        banner.transform.Find("result").GetComponent<Text>().text = achievement.achieved.ToString()+"/" + achievement.target.ToString();
        banner.transform.Find("Content").transform.Find("Slider").GetComponent<Slider>().value = (float)achievement.achieved/(float)achievement.target;

    }

}