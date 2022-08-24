using System.Collections;
using System.Collections.Generic;

public class PlayerPositionHelper {

    public static PlayerPosition[] PLAYER_POSITIONS = { PlayerPosition.SOUTH, PlayerPosition.EAST, PlayerPosition.WEST, PlayerPosition.NORTH };

    public List<PlayerPosition> getOtherPlayerPositions(PlayerPosition pos)
    {
        List<PlayerPosition> otherPlayers = new List<PlayerPosition>();
        foreach(PlayerPosition playerPosition in PLAYER_POSITIONS)
        {
            if(!playerPosition.Equals(pos))
            {
                otherPlayers.Add(playerPosition);
            }
        }
        return otherPlayers;
    }

    public enum RotationType {
        CLOCKWISE, 
        ANTICLOCKWISE
    }

    private RotationType rotationType;
    public PlayerPositionHelper() {
        if(GamePlay.GAME_VARIANT.Equals(GameVariant.HEARTS))
            this.rotationType = RotationType.CLOCKWISE;
        else 
            this.rotationType = RotationType.ANTICLOCKWISE;

    }

    public PlayerPosition  getPreviousPlayerPosition(PlayerPosition playerPosition) {
            if(rotationType.Equals(RotationType.ANTICLOCKWISE)) {
                if(playerPosition.Equals(PlayerPosition.NORTH)) 
                return PlayerPosition.EAST;
                else if(playerPosition.Equals(PlayerPosition.EAST)) 
                return PlayerPosition.SOUTH;
                else if(playerPosition.Equals(PlayerPosition.SOUTH)) 
                return PlayerPosition.WEST;
                else 
                return PlayerPosition.NORTH;

            } else {
                if(playerPosition.Equals(PlayerPosition.NORTH)) 
                return PlayerPosition.WEST;
                else if(playerPosition.Equals(PlayerPosition.WEST)) 
                return PlayerPosition.SOUTH;
                else if(playerPosition.Equals(PlayerPosition.SOUTH)) 
                return PlayerPosition.EAST;
                else 
                return PlayerPosition.NORTH;

            }
    }

    public PlayerPosition getNextPlayerPosition(PlayerPosition playerPosition) {
            if(rotationType.Equals(RotationType.CLOCKWISE)) {
                if(playerPosition.Equals(PlayerPosition.NORTH)) 
                return PlayerPosition.EAST;
                else if(playerPosition.Equals(PlayerPosition.EAST)) 
                return PlayerPosition.SOUTH;
                else if(playerPosition.Equals(PlayerPosition.SOUTH)) 
                return PlayerPosition.WEST;
                else 
                return PlayerPosition.NORTH;

            } else {
                if(playerPosition.Equals(PlayerPosition.NORTH)) 
                return PlayerPosition.WEST;
                else if(playerPosition.Equals(PlayerPosition.WEST)) 
                return PlayerPosition.SOUTH;
                else if(playerPosition.Equals(PlayerPosition.SOUTH)) 
                return PlayerPosition.EAST;
                else 
                return PlayerPosition.NORTH;

            }
    }



    public PlayerPosition getOppositePlayerPosition(PlayerPosition playerPosition) {
            if(playerPosition.Equals(PlayerPosition.NORTH)) 
            return PlayerPosition.SOUTH;
            else if(playerPosition.Equals(PlayerPosition.SOUTH)) 
            return PlayerPosition.NORTH;
            else if(playerPosition.Equals(PlayerPosition.WEST)) 
            return PlayerPosition.EAST;
            else 
            return PlayerPosition.WEST;

    }

    public static string getName(PlayerPosition pos) {
        if(pos.Equals(PlayerPosition.EAST))
            return "EAST";
        if(pos.Equals(PlayerPosition.SOUTH))
            return "SOUTH";
        if(pos.Equals(PlayerPosition.WEST))
            return "WEST";
         else
            return "NORTH";
    }


    public static PlayerPosition getRandomPlayerPosition() {
        System.Random rnd = new System.Random();
        double rndVal = rnd.NextDouble();
        if(rndVal < 0.25)
        return PlayerPosition.EAST;
        if(rndVal < 0.5)
        return PlayerPosition.WEST;
        if(rndVal < 0.75)
        return PlayerPosition.SOUTH;

        return PlayerPosition.NORTH;


    }


} 