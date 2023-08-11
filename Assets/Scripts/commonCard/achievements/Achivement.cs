[System.Serializable]
public class Achievement {
    public string id;
    public string description;

    
    public int target;
    public int achieved;


    public Achievement(string id, string description, int target, int achieved) {
        this.id = id;
        this.description = description;
        this.target = target;
        this.achieved = achieved;
    }
}