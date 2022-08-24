using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AchievementDao {

    string TAG = "AchievementDao";

    public void initiateAchievement(AchievementData achievementData) {

        if(!PlayerPrefs.HasKey(achievementData.gameType)) 
        {
            saveAchievementData(achievementData);
        } else {
            //Just copy  the achieved from all of them.
            AchievementData existingAchievementData = getAchievementData(achievementData.gameType);

            foreach(Achievement achievement in achievementData.achievements) {
                //Check if corresponding achievement is there or not. If it is new add it.
                Achievement existingAchievement =  getAchievementFromAchievementData(existingAchievementData, achievement.id);
                if(existingAchievement != null)
                    achievement.achieved = existingAchievement.achieved;


            }

            saveAchievementData(achievementData);

        }

    }


    public void reset(string gameType) {
        PlayerPrefs.SetString(gameType, null);
        PlayerPrefs.Save();   

    }


    public void saveAchievementData(AchievementData achievementData) {
        string jsonStr = JsonUtility.ToJson(achievementData);
        Debug.Log(TAG + " JSON is " + jsonStr);
        PlayerPrefs.SetString(achievementData.gameType, jsonStr);
        PlayerPrefs.Save();   

    }

    public Achievement getAchievement(string gameType, string achievementId) {

        AchievementData achievementData =  getAchievementData(gameType);


        return getAchievementFromAchievementData(achievementData, achievementId);

    }

    private void copyAchievement(Achievement existing, Achievement newAchievement) {
        existing.achieved = newAchievement.achieved;
        existing.target = newAchievement.target;
        existing.description = newAchievement.description;

    }

    public void incrementAchievement(string gameType, string achievementId) {

        Debug.Log(TAG + "Incrementing " + achievementId);

        AchievementData achievementData =  getAchievementData(gameType);
        foreach(Achievement achievement in achievementData.achievements) {
            if(achievement.id.Equals(achievementId)) {
                if(achievement.achieved < achievement.target) {
                    achievement.achieved  = achievement.achieved  + 1;
                }
            }
        }

        saveAchievementData(achievementData);


    }

    Achievement getAchievementFromAchievementData(AchievementData achievementData, string achievementKey) {
        foreach(Achievement achievement in achievementData.achievements) {
            if(achievement.id.Equals(achievementKey)) {
                return achievement;
            }
        }
        return null;
    }


    public AchievementData getAchievementData(string gameType) {

        string achvKey = getAchievementKey(gameType);
        Debug.Log(TAG + " achvKey is " + achvKey);

        if(PlayerPrefs.HasKey(achvKey)) {

            string achvDataStr = PlayerPrefs.GetString(achvKey);
            Debug.Log(TAG + " achvDataStr is " + achvDataStr);
           AchievementData achievementData = JsonUtility.FromJson<AchievementData>(achvDataStr);

            return achievementData;

        }

        return null;
    

    }


    public void getAllAchievement() {

    }


    public string getAchievementKey(string gameType) {

        return gameType ;

    }




}