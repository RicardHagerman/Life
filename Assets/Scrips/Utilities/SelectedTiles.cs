
using System.Collections.Generic;


public static class SelectedTiles
{

    static List<Tile> selection;

    public static List<Tile> Selection
    { 
        get
        { 
            return selection;
        } 
        set
        {
            selection = value;
        }
    }

    static SelectedTiles ()
    {
        selection = new List<Tile> ( 5000 );
    }
	
}
