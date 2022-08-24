using System.Collections.Generic;

public class CallbreakSetScore {

    Dictionary<PlayerPosition, double> setScore = new Dictionary<PlayerPosition, double>();
    Dictionary<int, double> westScoreRound = new Dictionary<int, double>();
    Dictionary<int, double> eastScoreRound = new Dictionary<int, double>();
    Dictionary<int, double> northScoreRound = new Dictionary<int, double>();

    Dictionary<int, double> southScoreRound = new Dictionary<int, double>();

    public CallbreakSetScore() {
        setScore.Add(PlayerPosition.NORTH, 0.0);
        setScore.Add(PlayerPosition.EAST, 0.0);
        setScore.Add(PlayerPosition.SOUTH, 0.0);
        setScore.Add(PlayerPosition.WEST, 0.0);
    }


    public void updateScore(int roundNumber, double northScore, double eastScore, double southScore, double westScore) {

        setScore.Add(PlayerPosition.NORTH, setScore[PlayerPosition.NORTH] + northScore);
        setScore.Add(PlayerPosition.EAST,setScore[PlayerPosition.EAST] + eastScore);
        setScore.Add(PlayerPosition.SOUTH,setScore[PlayerPosition.SOUTH] + southScore);
        setScore.Add(PlayerPosition.WEST,setScore[PlayerPosition.WEST] + westScore);

        northScoreRound.Add(roundNumber, northScore);
        eastScoreRound.Add(roundNumber, eastScore);
        southScoreRound.Add(roundNumber, southScore);
        westScoreRound.Add(roundNumber, westScore);
    }

}

