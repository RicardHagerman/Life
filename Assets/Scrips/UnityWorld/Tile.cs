using UnityEngine;
using System.Collections;
using System;


public class Tile
{
    Action<Tile> cbTileTypeCanged;

    TileType type = TileType.DEAD;

    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            TileType oldType = type;
            type = value;
            if ( cbTileTypeCanged != null && oldType != type )
                cbTileTypeCanged ( this );
        }
    }

    public World World{ get; protected set; }

    public int X { get; protected set; }

    public int Y { get; protected set; }

    public Tile ( World _wolrd, int x, int y )
    {
        this.World = _wolrd;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileTypeCangedCallback ( Action<Tile> callback )
    {
        cbTileTypeCanged += callback;
    }

    public void UnregisterTileTypeCangedCallback ( Action<Tile> callback )
    {
        cbTileTypeCanged -= callback;
    }

}
