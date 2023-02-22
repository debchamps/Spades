using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{

    static Dictionary<string, AudioClip> cliptypeToClipMap;

    static Dictionary<string, AudioConfig> audioConfigMap;

    static AudioSource audioSource;
    // Start is called before the first frame update

    
    void Start()
    {

        cliptypeToClipMap = new Dictionary<string, AudioClip>();
        audioSource = GetComponent<AudioSource>();        
    }

    public static void initAudios(List<AudioConfig> audioConfigs) {
        audioConfigMap = new Dictionary<string, AudioConfig>();
        foreach(AudioConfig audioConfig in audioConfigs) {
            audioConfigMap[audioConfig.id] = audioConfig;
            cliptypeToClipMap[audioConfig.id] = Resources.Load<AudioClip>(audioConfig.path);
        }
    }

    public static IEnumerator  playWithDelay(string clipType, float delayInSeconds) {
        yield return new WaitForSeconds(delayInSeconds);
        play(clipType);
    }

    public static void play(string clipType) {

        SettingsManager settingsManager = new SettingsManager();
        BraySettings settings = settingsManager.getBraySettings();
        if(!settings.soundEnabled)
            return;

        if(audioConfigMap.ContainsKey(clipType)) {
            AudioConfig config = audioConfigMap[clipType];
            audioSource.PlayOneShot(cliptypeToClipMap[clipType], config.volume);

        }

    }
}

//11961

//009000  Green bkg on blue_bkg
