public class CallbreakStatisticsKey {

    public static string TOTAL_POINTS = "BrayTotalPoints";
    public static string TOTAL_ROUNDS = "BrayTotalRounds";

    public static string LAST_N_MATCH_RESULT = "LastNMatchResult";

    public static string NO_ROUND_WIN = "BrayStatNoTrickWin";

    public static string TOTAL_MATCHES = "BrayTotalMatch";

    public static string CONSECUTIVE_WIN = "BrayConsecutiveWin";


    public static string COMPLETED_MATCH_POINTS = "BrayTotalRounds";



    public static string getRankKey(int rank) {

        return "BrayOfflineRank" + rank.ToString();

    }




}