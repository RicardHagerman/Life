using UnityEngine;
using System.Collections.Generic;
using System;


public enum TileType
{
    DEAD,
    LIVE
}

public class World
{
    Action <Tile> cbTileCanged;

    Tile [,] tiles;

    public int Width { get; protected set; }

    public int Height{ get; protected set; }

    public World ( int width, int height )
    {
        Width = width;
        Height = height;
        tiles = new Tile[Width, Height];
        for ( int x = 0; x < Width; x++ )
        {
            for ( int y = 0; y < Height; y++ )
            {
                tiles [ x , y ] = new Tile ( this, x, y );
                tiles [ x , y ].RegisterTileTypeCangedCallback ( OnTileCanged );   
            }
        }
        Debug.Log ( "World creatde with " + ( Width * Height ) + " tiles  " + Width );
    }

    public Tile GetTileAt ( int x, int y )
    {
        if ( x > Width - 1 || x < 0 || y > Height - 1 || y < 0 )
        {
            return null;
        }
        return tiles [ x , y ];
    }

    public void RegisterTileCangedCallback ( Action <Tile> callbackfunc )
    {
        cbTileCanged += callbackfunc;
    }

    public void UnregisterTileCangedCallback ( Action <Tile> callbackfunc )
    {
        cbTileCanged -= callbackfunc;
    }

    void OnTileCanged ( Tile t )
    {
        if ( cbTileCanged != null )
            cbTileCanged ( t );
    }



}
