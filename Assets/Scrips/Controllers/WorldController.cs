using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public class WorldController : MonoBehaviour
{

    public Action <GameObject> VisualTilesReady;
    public Action InitialStateReady;
    public Action<bool> Evolve;
    public Action <TileChange> ChangeState;
    public int width = 70;
    public int height = 50;

    public static WorldController Current { get; protected set ; }

    public  World world { get; protected set; }

    public Conway conway { get; protected set; }

    public GameObject Cursor { get; protected set; }

    bool simulationReady;
    bool startSimulation;
    bool simulationIsRunning;
    float densitySliderValue = .1f;

    public int numGenerations;

    [SerializeField]
    Sprite redButton;
    [SerializeField]
    Sprite yellowButton;
    [SerializeField]
    Sprite greenButton;
    [SerializeField]
    Text infoTextUpper;
    [SerializeField]
    Text infoTextLower;
    [SerializeField]
    Slider densitySlider;
    [SerializeField]
    Image silderHandelImage;
    [SerializeField]
    Button startButton;
    [SerializeField]
    Text startButtonText;

    void Awake ()
    {
        Current = this;
        VisualTilesReady += OnVisualTilesReady;
        InitialStateReady += OnInitialStateReady;
        world = new World ( width, height );
        conway = new Conway ( world.Width, world.Height );
        Camera.main.transform.position = new Vector3 ( world.Width / 2, world.Height / 2, Camera.main.transform.position.z );
    }

    void Start ()
    {
        infoTextUpper.text = "Initial Population Density";
        infoTextLower.text = "" + Mathf.Floor ( densitySlider.value * 100f ) + "%";
        silderHandelImage.sprite = redButton;
        startButton.image.sprite = yellowButton;
        startButtonText.text = "wait";
    }

    void Update ()
    {
        if ( simulationIsRunning )
        {
            infoTextLower.text = "#  " + numGenerations;
        }
    }

    void OnVisualTilesReady ( GameObject dragCursorPrefab )
    {
        conway.Grid.InitialState ( ( int )( ( width * height ) * densitySlider.value ) );
        Cursor = dragCursorPrefab;
    }

    void OnInitialStateReady ()
    {
        simulationReady = true;
        silderHandelImage.sprite = greenButton;
        startButtonText.text = "start";
        startButton.image.sprite = greenButton;
    }

    public void DensityValue ()
    {
        if ( simulationIsRunning )
        {
            densitySlider.value = densitySliderValue;
            return;
        }
        if ( simulationReady )
        {
            infoTextUpper.text = "Initial Population Density";
            infoTextLower.text = "" + Mathf.Floor ( densitySlider.value * 100f ) + "%";
            simulationReady = false;
            silderHandelImage.sprite = yellowButton;
            conway.Grid.InitialState ( ( int )( ( width * height ) * densitySlider.value ) );
            numGenerations = 0;
        }
    }

    public void StartSimulation ()
    {
        if ( simulationReady == false )
        {
            startButton.image.sprite = yellowButton;
            startButtonText.text = "wait";
            return;
        }
        startSimulation = !startSimulation;
        simulationIsRunning = startSimulation;
        if ( startSimulation )
        {
            Evolve ( true );
            startButtonText.text = "stop";
            startButton.image.sprite = redButton;
            silderHandelImage.sprite = yellowButton;
            densitySliderValue = densitySlider.value;
            infoTextUpper.text = "Number Of Generations";
        }
        else
        {
            Evolve ( false );
            startButtonText.text = "start";
            startButton.image.sprite = greenButton;
            silderHandelImage.sprite = greenButton;
        }
    }

    public Tile GetTileAtWorldCoordinate ( Vector3 coord )
    {
        int x = Mathf.FloorToInt ( coord.x );
        int y = Mathf.FloorToInt ( coord.y );

        return world.GetTileAt ( x, y );
    }
}
