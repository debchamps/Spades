public class CallbreakTimingConfig {

    public float delayFirstBid = 2500;
    public float delayBetweenBid = 1000;

    public float delayBetweenMoves = 500;

    public float delayBetweenFirstRoundFirstMove = 4500;
    public float delayBetweenPassingFirstRoundFirstMove = 7000;

    public float delayBetweenRoundFirstMove = 2500;
    public float delayBetweenRound = 300;

    public float cardDistributionDelay = 800;

    public float humanCardPlayAnimation = 500;

    public float humanCardBringToInitialPos = 200;


    public float scoreboardAutoDisappearTime = 5000;

    public float defaultDialogMoveTime = 0.4f;

    public float waitBeforeWinningMoveAnimationInSeconds;

    public float winningMoveAnimationTimeInSeconds = 0.5f;

    public float waitBeforeBiddingAnimation = 1.25f;

    public float waitBeforeBiddingEnableAnimation = 1.5f;

    public float waitBeforeShowingScoreboardAfterRoundsInSeconds;


    public float additionalDelayForShootingMoonAnimation = 3f;



    public CallbreakTimingConfig() {
        this.waitBeforeWinningMoveAnimationInSeconds = humanCardPlayAnimation * 1.8f/1000;
        this.waitBeforeShowingScoreboardAfterRoundsInSeconds = .5f + waitBeforeWinningMoveAnimationInSeconds  + winningMoveAnimationTimeInSeconds * 1.6f;

        this.waitBeforeBiddingEnableAnimation = waitBeforeBiddingAnimation * 1.5f;

    }


}