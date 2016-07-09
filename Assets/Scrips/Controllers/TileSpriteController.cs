using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileChange
{
    ADDLIVE,
    KILL
}

public class TileSpriteController: MonoBehaviour
{
    [SerializeField] Sprite liveSprite;
    [SerializeField] Sprite deadSprite;
    [SerializeField] GameObject dragCursorPrefab;

    World world { get { return WorldController.Current.world; } }

    Conway conway{ get { return WorldController.Current.conway; } }

    Dictionary <Tile,GameObject> tileGameObjectMap;

    float timer;

    bool evolve;

    void Start ()
    {
        tileGameObjectMap = new Dictionary<Tile, GameObject> ();
        CreateVisualTiles ();
        world.RegisterTileCangedCallback ( OnTileChanged );
        WorldController.Current.Evolve += OnEvolve;
        timer = 0f;
        if ( WorldController.Current.VisualTilesReady != null )
            WorldController.Current.VisualTilesReady ( dragCursorPrefab );
    }

    void Update ()
    {
        if ( evolve )
        {
            timer += Time.deltaTime;
            if ( timer > .05f )
            {
                timer = 0f;
                conway.Grid.UpdateState ();
            }
        }
    }

    void OnEvolve ( bool _evolve )
    {
        Debug.Log ( "evlove" );
        evolve = _evolve;
    }

    #region VisualToDataLink


    void OnTileChanged ( Tile tile_data )
    {
        if ( tileGameObjectMap.ContainsKey ( tile_data ) == false )
        {
            Debug.LogError ( "OnTileTypeChanged -- no tile_data ??" );
        }

        GameObject tile_go = tileGameObjectMap [ tile_data ];

        if ( tile_go == null )
        {
            Debug.LogError ( "tileGameObjectMap returned tile-go as null." );
            return;
        }

        if ( tile_data.Type == TileType.LIVE )
        {
            tile_go.GetComponent<SpriteRenderer> ().sprite = liveSprite;
        }
        else if ( tile_data.Type == TileType.DEAD )
        {
            tile_go.GetComponent<SpriteRenderer> ().sprite = deadSprite;
        }
        else
        {
            Debug.LogError ( "OnTileTypeChanged unrecognized tile type" );
        }
    }


    #endregion

    #region helper Methods

    void CreateVisualTiles ()
    {
        for ( int x = 0; x < world.Width; x++ )
        {
            for ( int y = 0; y < world.Height; y++ )
            {
                Tile tile_data = world.GetTileAt ( x, y );

                GameObject tile_go = new GameObject ();

                tileGameObjectMap.Add ( tile_data, tile_go );

                tile_go.name = "Tile _" + x + "_" + y;
                tile_go.transform.position = new Vector3 ( tile_data.X, tile_data.Y, 0 );
                tile_go.transform.SetParent ( this.transform, true );
                    
                tile_go.AddComponent<SpriteRenderer> ().sprite = deadSprite;
            }
        }
        ObjectPool.Preload ( gameObject, dragCursorPrefab, 500 );
    }

    #endregion


}
