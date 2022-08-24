public class AudioConfig {

    public string id;
    public string path;

    public float volume;


    public AudioConfig(string id, string path, float volume) {
        this.id = id;
        this.path = path;
        this.volume = volume;

    }

    public AudioConfig(string id, string path) {
        this.id = id;
        this.path = path;
        this.volume = 1f;

    }


}