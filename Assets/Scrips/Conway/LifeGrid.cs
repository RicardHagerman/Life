using System;


public class LifeGrid
{
    int gridHeight;
    int gridWidth;
    bool evolve;

    public TileType [,] CurrentState{ get; private set; }

    TileType [,] nextState;

    public LifeGrid ( int width, int height )
    {
        gridWidth = width;
        gridHeight = height;
        WorldController.Current.Evolve += OnEvolve;
        WorldController.Current.ChangeState += OnChangeState;
        CurrentState = new TileType[gridWidth, gridHeight];
        nextState = new TileType[gridWidth, gridHeight];
    }

    public void UpdateState ()
    {
        if ( evolve )
        {
            int x = 0;
            for ( int i = 0; i < gridWidth; i++ )
                for ( int j = 0; j < gridHeight; j++ )
                {
                    var liveNeighbors = GetLiveNeighbors ( i, j );
                    nextState [ i , j ] = LifeRules.GetNewState ( CurrentState [ i , j ], liveNeighbors );
                    x++;
                    var t = WorldController.Current.world.GetTileAt ( i, j );
                    t.Type = nextState [ i , j ];
                }
            CurrentState = nextState;
            nextState = new TileType[gridWidth, gridHeight];
            WorldController.Current.numGenerations++;
        }
    }

    public void InitialState ( int liveTiles )
    {
        if ( evolve )
            return;
        for ( int i = 0; i < gridWidth; i++ )
            for ( int j = 0; j < gridHeight; j++ )
            {
                CurrentState [ i , j ] = TileType.DEAD;
                var t = WorldController.Current.world.GetTileAt ( i, j );
                t.Type = TileType.DEAD;
            }
        Random random = new Random ();
        int rI;
        int rJ;
        int limit = 0;
        for ( int i = 0; i < liveTiles; i++ )
        {
            limit++;
            rI = random.Next ( gridWidth );
            rJ = random.Next ( gridHeight );
            if ( CurrentState [ rI , rJ ] == TileType.LIVE )
            {
                i--;
                if ( limit > liveTiles * 2 )
                {
                    break;
                }
            }
            else
            {
                CurrentState [ rI , rJ ] = TileType.LIVE;
                var t = WorldController.Current.world.GetTileAt ( rI, rJ );
                t.Type = TileType.LIVE;
            }
        }
        if ( WorldController.Current.InitialStateReady != null )
            WorldController.Current.InitialStateReady ();
    }

    int GetLiveNeighbors ( int positionX, int positionY )
    {
        int liveNeighbors = 0;
        for ( int i = -1; i <= 1; i++ )
            for ( int j = -1; j <= 1; j++ )
            {
                if ( i == 0 && j == 0 )
                    continue;
                int neighborX = positionX + i;
                int neighborY = positionY + j;
                if ( neighborX >= 0 && neighborX < gridWidth &&
                     neighborY >= 0 && neighborY < gridHeight )
                {
                    if ( CurrentState [ neighborX , neighborY ] == TileType.LIVE )
                        liveNeighbors++;
                }
            }
        return liveNeighbors;
    }

    void OnEvolve ( bool value )
    {
        evolve = value;
    }

    void OnChangeState ( TileChange change )
    {
        if ( evolve )
            return;
        switch ( change )
        {
            case TileChange.ADDLIVE:
                foreach ( Tile tile in SelectedTiles.Selection )
                {
                    CurrentState [ tile.X , tile.Y ] = TileType.LIVE;
                    tile.Type = TileType.LIVE;
                }
                break;
            case TileChange.KILL:
                foreach ( Tile tile in SelectedTiles.Selection )
                {
                    CurrentState [ tile.X , tile.Y ] = TileType.DEAD;
                    tile.Type = TileType.DEAD;
                }
                break;
            default:
                break;
        }
    }
}


