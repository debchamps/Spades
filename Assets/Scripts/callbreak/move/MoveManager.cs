public class MoveManeger {

    FirstMove firstMove = new FirstMove();
    SubsequentMove subsequentMove = new SubsequentMove();

    public Card playMove(SpadeGameState callbreakGameState, PlayerPosition pos) {

        if(callbreakGameState.getCurrentRound().moves.Count == 0)
            return firstMove.playMove(callbreakGameState, pos);
        else 
            return subsequentMove.playMove(callbreakGameState, pos);
    }


}