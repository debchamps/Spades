using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbreakAudioInit {


    public static void Init() {

        AudioConfig audioConfig1 = new AudioConfig(AudioClipType.DEFAULT_NOTIFICATION, "audio/" + AudioClipType.DEFAULT_NOTIFICATION, .5f);
        AudioConfig audioConfig2 = new AudioConfig(AudioClipType.PLAY_CARD, "audio/" + AudioClipType.PLAY_CARD, .1f);
        AudioConfig audioConfig3 = new AudioConfig(AudioClipType.ROUND_WON, "audio/" + AudioClipType.ROUND_WON, .1f);
        AudioConfig audioConfig4 = new AudioConfig(AudioClipType.SHUFFLE_CARD, "audio/" + AudioClipType.SHUFFLE_CARD, .1f);


        List<AudioConfig> audioConfigs = new List<AudioConfig>();

        audioConfigs.Add(audioConfig1);
        audioConfigs.Add(audioConfig2);
        audioConfigs.Add(audioConfig3);
        audioConfigs.Add(audioConfig4);

        AudioManagerScript.initAudios(audioConfigs);

    }
}