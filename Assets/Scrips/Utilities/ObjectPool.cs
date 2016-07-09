using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ObjectPool
{
    
    #region Pool classes

    /// <summary>
    /// The Pool class represents the pool for a particular prefab.
    /// </summary>
    class Pool
    {
        //Id of anything instantiated (purley cosmetic)
        int nextID = 1;

        //collection of inactive objects
        Stack<GameObject> inactive;

        // The prefab that are being pooled
        GameObject prefab;

        //Constructor
        public Pool ( GameObject _prefab, int initialQty )
        {
            this.prefab = _prefab;
            inactive = new Stack<GameObject> ( initialQty );
        }

        public GameObject Spawn ( GameObject _parent, Vector3 pos, Quaternion rot, bool grow )
        {
            GameObject obj;

            //if no objects in pool
            if ( inactive.Count == 0 )
            {
                if ( grow )
                {
                    obj = GameObject.Instantiate ( prefab, pos, rot ) as GameObject;
                    obj.name = prefab.name + " (" + ( nextID++ ) + ")";

                    // add a pool member component so we know with pool the gameobject belongs to
                    obj.AddComponent<PoolMember> ().myPool = this;
                }
                else
                {
                    Debug.Log ( "Empty Pool" );
                    return null;
                }

            }
            else
            {
                obj = inactive.Pop ();

                if ( obj == null )
                {
                    // The inactive object we expected to find no longer exists.
                    // The most likely causes are:
                    //   - Someone calling Destroy() on our object
                    //   - A scene change (which will destroy all our objects).
                    //     NOTE: This could be prevented with a DontDestroyOnLoad
                    //     if you really don't want this.
                    // No worries -- we'll just try the next one in our sequence.

                    return Spawn ( _parent, pos, rot, grow );
                }
            }

            obj.transform.SetParent ( _parent.transform, true );
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive ( true );
            return obj;
        }

        //inactivates the gameobject and adds it to the stack inactive
        public void Despawn ( GameObject obj )
        {
            obj.SetActive ( false );
            inactive.Push ( obj );
        }

    }

    /// <summary>
    /// Added to freshly instantiated objects, so we can link back
    /// to the correct pool on despawn.
    /// </summary>
    class PoolMember : MonoBehaviour
    {
        public Pool myPool;
    }

    #endregion

    const int DEFAULT_POOL_SIZE = 500;

    static Dictionary <GameObject,Pool> pools;

    static void Init ( GameObject prefab = null, int qty = DEFAULT_POOL_SIZE )
    {
        if ( pools == null )
        {
            pools = new Dictionary<GameObject, Pool> ();
        }
        if ( prefab != null && pools.ContainsKey ( prefab ) == false )
        {
            pools [ prefab ] = new Pool ( prefab, qty );
        }
    }

    public static void Preload ( GameObject _parent, GameObject prefab, int qty = 1 )
    {
        Init ( prefab, qty );

        GameObject [] objs = new GameObject[qty];

        for ( int i = 0; i < qty; i++ )
        {
            objs [ i ] = Spawn ( _parent, prefab, Vector3.zero, Quaternion.identity, true );
        }

        for ( int i = 0; i < qty; i++ )
        {
            Despawn ( objs [ i ] );
        }
    }

    /// <summary>
    /// Returns a gameobject, at position pos
    /// with rotation rot, from the pool of the prefab and
    /// sets the parent to the _parent gameobject. 
    /// If grow == true, the pool will increase in size 
    /// if spawn is caled and the pool is empty
    /// The returned gameobject is set to active
    /// </summary>
    public static GameObject Spawn ( GameObject _parent, GameObject prefab, Vector3 pos, Quaternion rot, bool grow )
    {
        // if there are no pool, create one
        Init ( prefab );

        // returns an inactive gameobject from te relevant pool (the pool instantiates an new gameobject if required and grow == true)
        return pools [ prefab ].Spawn ( _parent, pos, rot, grow );
    }

    public static void Despawn ( GameObject obj )
    {
        PoolMember pm = obj.GetComponent<PoolMember> ();

        if ( pm == null )
        {
            Debug.Log ( "Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead." );
            GameObject.Destroy ( obj );
        }
        else
        {
            pm.myPool.Despawn ( obj );
        }
    }
        
	
}
