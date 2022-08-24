using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StatisticsDao {


    public void add(string key, float f) {

        float val = PlayerPrefs.GetFloat(key, 0.0f);
        PlayerPrefs.SetFloat(key, val + f);
        PlayerPrefs.Save();


    }

    public void  increment(string key) {
        int val = get(key);
        set(key, val+1);

    }

    int get(string key) {
        return PlayerPrefs.GetInt(key, 0);
    }

    public string getString(string key) {
        return PlayerPrefs.GetString(key, "");
    }

    public void setString(string key, string val) {
       PlayerPrefs.SetString(key, val); 
       PlayerPrefs.Save();
    }



    void set(string key, int val) {
       PlayerPrefs.SetInt(key, val); 
       PlayerPrefs.Save();
    }

    void updateOnGameComplete() {

    }


}