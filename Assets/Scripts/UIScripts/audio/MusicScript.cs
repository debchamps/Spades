using UnityEngine;
using UnityEngine.UI;

public class MusicScript : MonoBehaviour
{
    public static string MUSIC_ENABLED = "MusicEnabled";
    
    void Start()
    {

        if (isMusicEnabled())
        {
            gameObject.GetComponent<AudioSource>().Play();
        } else
        {
            gameObject.GetComponent<AudioSource>().Stop();

        }

    }

    public void onMusicChange() {
        bool isMusicEnabled = gameObject.GetComponent<Toggle>().isOn;
        if (isMusicEnabled)
            enableMusic();
        else
            disableMusic();
    }

    public void enableMusic() {
        gameObject.GetComponent<AudioSource>().Play();

        PlayerPrefs.SetInt(MUSIC_ENABLED, 1);
        PlayerPrefs.Save();

    }


    public void disableMusic()
    {
        gameObject.GetComponent<AudioSource>().Stop();
        PlayerPrefs.SetInt(MUSIC_ENABLED, 0);
        PlayerPrefs.Save();
    }

    public bool isMusicEnabled()
    {
        return (PlayerPrefs.GetInt(MUSIC_ENABLED, 0) == 1);
    }


}

//11961

//009000  Green bkg on blue_bkg
