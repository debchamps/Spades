using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintScript : MonoBehaviour  {

    public void hint() {

        ((HumanPlayer)GamePlay.matchState.playerMap[PlayerPosition.SOUTH]).highlightBestMove();

        
    }



}