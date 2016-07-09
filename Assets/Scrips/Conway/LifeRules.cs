using System;

public class LifeRules
{
    public static TileType GetNewState ( TileType currentState, int liveNeighbors )
    {
        switch ( currentState )
        {
            case TileType.LIVE:
                if ( liveNeighbors < 2 || liveNeighbors > 3 )
                    return TileType.DEAD;
                break;
            case TileType.DEAD:
                if ( liveNeighbors == 3 )
                    return TileType.LIVE;
                break;
        }
        return currentState;
    }
}
    





