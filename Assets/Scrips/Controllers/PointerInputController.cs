using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PointerInputController : MonoBehaviour
{
    Vector3 lastFramePosition;
    Vector3 currentFramePosition;
    Vector3 dragStartPosition;
    GameObject cursorToSpawn;
    List<GameObject> dragPreviewObjects;
    Camera cam;
    Vector3 camStartPosition;
    int worldSizeX;
    int worldSizeY;
    float zoomfactorX = 0.516f;
    float zoomfactorY = 0.855f;

    void Start ()
    {
        cursorToSpawn = WorldController.Current.Cursor;
        worldSizeX = WorldController.Current.world.Width;
        worldSizeY = WorldController.Current.world.Height;
        dragPreviewObjects = new List<GameObject> ( 500 );
        cam = Camera.main;
        camStartPosition = cam.transform.position;
    }

    void Update ()
    {
        if ( cursorToSpawn == null )
            return;
        MousePosition ();
        UpdateDraging ();
        UpdateCameraMovement ();
        LateMousePosition ();
    }

    void UpdateDraging ()
    {
        if ( Input.GetMouseButtonUp ( 0 ) )
        {
            if ( WorldController.Current.ChangeState != null )
                WorldController.Current.ChangeState ( TileChange.ADDLIVE );
            CleanUpCursors ();
        }

        if ( EventSystem.current.IsPointerOverGameObject () )
            return;
        if ( Input.GetMouseButtonDown ( 0 ) )
            dragStartPosition = currentFramePosition;
        if ( Input.GetMouseButton ( 0 ) )
        {
            int start_x = 0;
            int end_x = 0;
            int start_y = 0;
            int end_y = 0;
            PointerDragPosition ( ref start_x, ref  end_x, ref start_y, ref  end_y );
            CleanUpCursors ();
            for ( int x = start_x; x <= end_x; x++ )
            {
                for ( int y = start_y; y <= end_y; y++ )
                {
                    Tile t = WorldController.Current.world.GetTileAt ( x, y );
                    if ( t != null )
                    {
                        SelectedTiles.Selection.Add ( t );
                        if ( y == Mathf.FloorToInt ( dragStartPosition.y ) || x == Mathf.FloorToInt ( dragStartPosition.x )
                             || x == end_x || x == start_x || y == end_y || y == start_y )
                        {
                            GameObject go = ObjectPool.Spawn ( gameObject, cursorToSpawn, new Vector3 ( t.X, t.Y, 0 ), Quaternion.identity, true );
                            dragPreviewObjects.Add ( go );
                        }
                    }
                }
            }
        }
    }

    void UpdateCameraMovement ()
    {
        if ( Input.GetMouseButton ( 2 ) || Input.GetMouseButton ( 1 ) )
        {
            Vector3 diffMousePosition = lastFramePosition - currentFramePosition;
            cam.transform.Translate ( diffMousePosition );
            ZoomBounds ();
        }
        Camera.main.orthographicSize -= Camera.main.orthographicSize * ( float )( System.Math.Round ( Input.GetAxis ( "Mouse ScrollWheel" ), 2 ) / 2 );
        Camera.main.orthographicSize = Mathf.Clamp ( Camera.main.orthographicSize, 10f, 160f );
        ZoomBounds ();
    }

    void ZoomBounds ()
    {
        if ( cam.orthographicSize > 158f )
        {
            cam.transform.position = camStartPosition;
            return;
        }

        if ( cam.transform.position.x < MinXpos () )
            cam.transform.position = new Vector3 ( MinXpos (), cam.transform.position.y, cam.transform.position.z );
        else if ( cam.transform.position.x > MaxXpos () )
            cam.transform.position = new Vector3 ( MaxXpos (), cam.transform.position.y, cam.transform.position.z );
    
        if ( cam.transform.position.y < MinYpos () )
            cam.transform.position = new Vector3 ( cam.transform.position.x, MinYpos (), cam.transform.position.z );
        else if ( cam.transform.position.y > MaxYpos () )
            cam.transform.position = new Vector3 ( cam.transform.position.x, MaxYpos (), cam.transform.position.z );
    }

    float MinXpos ()
    {
        return cam.orthographicSize * zoomfactorX;
    }

    float MaxXpos ()
    {
        return worldSizeX - cam.orthographicSize * zoomfactorX;
    }

    float MinYpos ()
    {
        return cam.orthographicSize * zoomfactorY;
    }

    float MaxYpos ()
    {
        return worldSizeY - cam.orthographicSize * zoomfactorY;
    }

    void PointerDragPosition ( ref int start_x, ref int end_x, ref int start_y, ref int end_y )
    {
        start_x = Mathf.FloorToInt ( dragStartPosition.x );
        end_x = Mathf.FloorToInt ( currentFramePosition.x );
        if ( end_x < start_x )
        {
            int temp = end_x;
            end_x = start_x;
            start_x = temp;
        }
        start_y = Mathf.FloorToInt ( dragStartPosition.y );
        end_y = Mathf.FloorToInt ( currentFramePosition.y );
        if ( end_y < start_y )
        {
            int temp = end_y;
            end_y = start_y;
            start_y = temp;
        }
    }

    void CleanUpCursors ()
    {
        while ( dragPreviewObjects.Count > 0 )
        {
            GameObject go = dragPreviewObjects [ 0 ];
            dragPreviewObjects.RemoveAt ( 0 );
            ObjectPool.Despawn ( go );
        }
        SelectedTiles.Selection.Clear ();
    }

    void MousePosition ()
    {
        currentFramePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        currentFramePosition.z = 0;
    }

    void LateMousePosition ()
    {
        lastFramePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        lastFramePosition.z = 0;
    }

}
