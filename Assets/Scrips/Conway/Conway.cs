using System;


//brutally adapted from http://www.jeremybytes.com/Downloads.aspx#ConwayTDD

public class Conway
{
    public LifeGrid Grid{ get; private set; }


    public Conway ( int gridWidth, int gridHeight )
    {
        Grid = new LifeGrid ( gridWidth, gridHeight );
    }




}


