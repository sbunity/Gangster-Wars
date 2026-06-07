using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// Pool module v 1.2.0

/// <summary>
/// Class that manages all pool operations.
/// </summary>
public class PoolManager : MonoBehaviour
{
	/// <summary>
	/// Static referense to instance of a class.
	/// </summary>
	public static PoolManager instance;

	/// <summary>
	/// Empty object to store all pooled objects at scene (can be asigned manualy).
	/// </summary>
	public GameObject objectsContainer;

	/// <summary>
	/// List of all existing pools.
	/// </summary>
	public List<Pool> pools;

	public PoolManagerType poolType = PoolManagerType.Local;

	/// <summary>
	/// When enabled PoolManager will automaticaly setup pooled objects amount using cashed data.
	/// </summary>
	public bool useCache = false;

	/// <summary>
	/// When enabled allows to know how much objects where used.
	/// </summary>
	public bool debugMode = false;

	/// <summary>
	/// When enabled pool in not destroyed between scenes (usefull when the same objects are used in different scenes).
	/// </summary>

	/// <summary>
	/// Dictionary which allows to acces Pool by name.
	/// </summary>
	private Dictionary<string, Pool> poolsDictionary;

	/// <summary>
	/// True when PoolManager is already initialized.
	/// </summary>
	private bool isInited;

	private PoolManagerCache cache;

	private const string CACHE_FILE_NAME = "PoolManagerCache";

	public enum PoolManagerType
	{
		Global,
		Local,
	}

	private string currentCacheId = string.Empty;

	/// <summary>
	/// Initialize single instance of PoolManager.
	/// </summary>
	public static void InitSingletone ( )
	{
		PoolManager poolManager = FindObjectOfType<PoolManager> ( );

		if ( poolManager != null )
		{
			poolManager.Init ( );

			instance = poolManager;
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogError ( "Please, create pool manager" );
		}
#endif
	}



#if UNITY_EDITOR

	/// <summary>
	/// Output provided by debug mode.
	/// </summary>
	private string debugInfo;

	/// <summary>
	/// Gathering and printing output information provided by debug mode.
	/// </summary>
	public void OnApplicationQuit ( )
	{
		if ( useCache )
		{
			//UpdateCache ( );
		}

		if ( debugMode )
		{

			debugInfo = "";
			for ( int i = 0 ; i < pools.Count ; i++ )
			{
				Pool currentPool = pools [ i ];
				debugInfo += "Pool: " + currentPool.name + "\n";
				debugInfo += "pool size : " + currentPool.poolSize + "\tmaxUsed : " + currentPool.maxItemsUsedInOneTime + "\n\n";
				pools [ i ].poolSize = currentPool.maxItemsUsedInOneTime;
			}
			Debug.Log ( debugInfo );
			Debug.Break ( );
		}
	}
#endif

	void Awake ( )
	{
		instance = this;

		Init ( );
	}

	/// <summary>
	/// Initializing of PoolManager.
	/// </summary>
	void Init ( )
	{
		if ( instance == null )
			return;

		if ( poolType == PoolManagerType.Global )
		{
			DontDestroyOnLoad ( this );
		}

		if ( objectsContainer == null )
		{
			objectsContainer = new GameObject ( "PooledObjects" );

			if ( poolType == PoolManagerType.Global )
			{
				DontDestroyOnLoad ( objectsContainer );
			}
		}

		// comment
		if ( useCache )
		{
			LoadCahe ( );
		}

		poolsDictionary = new Dictionary<string, Pool> ( );

		foreach ( Pool pool in pools )
		{
			//InitializePool ( pool );
			pool.InitializePool ( );
			poolsDictionary.Add ( pool.name, pool );
		}
	}


	/// <summary>
	/// Adds one more object to Pool.
	/// </summary>
	/// <param name="pool">Pool at which should be added new object.</param>
	/// <returns>Returns reference to just added object.</returns>
	public static GameObject AddObjectToPool ( Pool pool )
	{
		GameObject inst = ( GameObject ) Instantiate ( pool.objectToPool );
		inst.SetActive ( false );
		pool.pooledObjects.Add ( inst );

		inst.transform.SetParent ( instance.objectsContainer.transform );

		return inst;
	}

	public void ReloadPools ( )
	{
		foreach ( Pool pool in pools )
		{
			int objectsDelta = pool.poolSize - pool.pooledObjects.Count;
			// if new size is bigger
			// we need to add objects
			if ( objectsDelta > 0 )
			{
				for ( int i = 0 ; i < objectsDelta ; i++ )
				{
					pool.AddObjectToPool ( );
				}
			}
			// if new size is smaller
			// we need to delete 
			else if ( objectsDelta < 0 )
			{
				for ( int i = 0 ; i < -objectsDelta ; i++ )
				{
					Destroy ( pool.pooledObjects [ pool.pooledObjects.Count - 1 ] );
					pool.pooledObjects.RemoveAt ( pool.pooledObjects.Count - 1 );
				}

			}
		}
	}

	/// <summary>
	/// Returns reference to Pool by it's name.
	/// </summary>
	/// <param name="poolName">Name of Pool which should be returned.</param>
	/// <returns>Reference to Pool.</returns>
	public static Pool GetPoolByName ( string poolName )
	{
		if ( instance == null )
		{
			InitSingletone ( );
		}

		if ( instance.poolsDictionary.ContainsKey ( poolName ) )
		{
			return instance.poolsDictionary [ poolName ];
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError ( "Not found pool with name: '" + poolName + "'" );
#endif
			return null;
		}
	}

	public static Pool AddNewPool ( string poolName, GameObject poolObj, int poolSize )
	{
		Pool newPool = new Pool ( poolName, poolObj, poolSize, true );
		newPool.InitializePool ( );
		instance.pools.Add ( newPool );
		instance.poolsDictionary.Add ( newPool.name, newPool );
        return newPool;
	}


	#region Cache Management

	// //////////////////////////////////////////////////////////////////////////////////
	// New Chace system

	private void LoadCahe ( )
	{
		//cache = Serializer.DeserializeFromResourses<PoolManagerCache> ( CACHE_FILE_NAME );
	}

	public static void InitializeCache ( string levelId )
	{
		//UsefulShortcuts.ClearConsole();

		instance.currentCacheId = levelId;

		List<PoolCache> poolsCache = instance.cache.GetPoolCache ( levelId );

		// if there is cache - applying it
		if ( poolsCache != null )
		{
#if UNITY_EDITOR
			for ( int i = 0 ; i < instance.pools.Count ; i++ )
			{
				int index = poolsCache.FindIndex ( x => x.poolName == instance.pools [ i ].name );
				if ( index != -1 )
				{
					instance.pools [ i ].poolSize = poolsCache [ index ].poolSize;
				}
			}
#else
            /*for (int i = 0; i < instance.pools.Count; i++)
			{
				instance.pools[i].poolSize = poolsCache[index].poolSize;
			}*/
#endif
			instance.ReloadPools ( );
		}
		else
		{
			Debug.Log ( "There is no cache" );
		}
	}

	private void UpdateCache ( )
	{
		//UsefulShortcuts.ClearConsole();

		/*if ( currentCacheId != "" )
		{
			// true if there is no saved cache
			bool init = !cache.ContainsLevel ( currentCacheId );

			List<PoolCache> newCache = new List<PoolCache> ( );

			for ( int i = 0 ; i < pools.Count ; i++ )
			{
				// if we initializing cahe we simple adding current pool info
				if ( init )
				{
					newCache.Add ( new PoolCache ( pools [ i ].name, pools [ i ].poolSize ) );
				}
				// if there is cache, let's update this stuff
				else
				{
					int index = cache.poolsCache [ currentCacheId ].FindIndex ( x => x.poolName == pools [ i ].name );
					if ( index != -1 )
					{
						cache.poolsCache [ currentCacheId ] [ index ].UpdateSize ( pools [ i ].maxItemsUsedInOneTime );
						newCache.Add ( cache.poolsCache [ currentCacheId ] [ index ] );
					}
					else
					{
						newCache.Add ( new PoolCache ( pools [ i ].name, pools [ i ].poolSize ) );
					}
				}
			}

			if ( init )
			{
				cache.poolsCache.Add ( currentCacheId, newCache );
			}
			else
			{
				cache.UpdateCache ( currentCacheId, newCache );
			}

			//Serializer.SerializeToResources ( cache, CACHE_FILE_NAME );
		}
		else
		{
			Debug.Log ( "NOT updating cache! THIS LEVEL WAS NOT INITIALIZED" );
		}*/

	}

	public static void OnCachedLevelComplete ( )
	{
		instance.currentCacheId = "";
	}

	public void DeleteCache ( )
	{
		//Serializer.DeleteFileResourses ( CACHE_FILE_NAME );
	}

	#endregion
}

// Log
// v 1.0.0 
// Basic version of pool

// v 1.1.0 
// Added PoolManager editor

// v 1.2.0
// Added cache system


// IDEAS
// Allow to use local(only this scene) and global(doesn't destroyed beetwen scenes) pools
// Remove Serializer using, to make this module independent