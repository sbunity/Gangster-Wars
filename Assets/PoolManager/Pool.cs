using UnityEngine;
using System;
using System.Collections.Generic;

// Pool module 1.2.0

/// <summary>
/// Basic pool class. Contains pool settings and references to pooled objects.
/// </summary>
[Serializable]
public class Pool
{
	/// <summary>
	/// Pool name, use it get reference to pool at PoolManager
	/// </summary>
	public string name = string.Empty;

	/// <summary>
	/// Reference to object which shood be pooled.
	/// </summary>
	[Space ( 5 )]
	public GameObject objectToPool;
	/// <summary>
	/// Number of objects which be created be deffault.
	/// </summary>
	public int poolSize = 10;
	/// <summary>
	/// True means: if there is no available object, the new one will be added to a pool.
	/// Otherwise will be returned null.
	/// </summary>
	public bool willGrow = true;
	/// <summary>
	/// List of pooled objects.
	/// </summary>
	public List<GameObject> pooledObjects;

	/// <summary>
	/// Parent object for this pool.
	/// </summary>
	public Transform parent;

#if UNITY_EDITOR
	/// <summary>
	/// Number of objects that where active at one time.
	/// </summary>
	public int maxItemsUsedInOneTime = 0;
#endif

	/// <summary>
	/// Basic constructor.
	/// </summary>
	public Pool ( )
	{
		name = string.Empty;
		//objectToPool = new GameObject ( );
	}


	public Pool ( string poolName, GameObject newObjectToPool, int newPoolSize, bool willGrowCheck )
	{
		name = string.Empty;
		name = poolName;

		poolSize = newPoolSize;
		willGrow = willGrowCheck;

		//objectToPool = new GameObject ( );
		objectToPool = newObjectToPool;
	}

	public void InitializePool ( )
	{
		pooledObjects = new List<GameObject> ( );

		if ( objectToPool != null )
		{
			for ( int i = 0 ; i < poolSize ; i++ )
			{
				AddObjectToPool ( );
			}
		}
		else
		{
			Debug.LogError ( "There's no attached prefab at pool: \"" + name + "\"" );
		}
	}

	public GameObject AddObjectToPool ( )
	{
		GameObject inst = ( GameObject ) MonoBehaviour.Instantiate ( objectToPool );
		inst.SetActive ( false );
		pooledObjects.Add ( inst );

		if ( parent != null )
			inst.transform.SetParent ( parent );
		else
			inst.transform.SetParent ( PoolManager.instance.objectsContainer.transform );

		return inst;
	}

	public GameObject AddObjectToPool ( bool activateObject )
	{
		GameObject inst = ( GameObject ) MonoBehaviour.Instantiate ( objectToPool );
		inst.SetActive ( activateObject );
		pooledObjects.Add ( inst );

		if ( parent != null )
			inst.transform.SetParent ( parent );
		else
			inst.transform.SetParent ( PoolManager.instance.objectsContainer.transform );

		return inst;
	}

	public GameObject AddObjectToPool ( Vector3 position, bool activateObject = true )
	{
		GameObject inst = ( GameObject ) MonoBehaviour.Instantiate ( objectToPool );
		pooledObjects.Add ( inst );
		inst.transform.position = position;
		if ( parent != null )
			inst.transform.SetParent ( parent );
		else
			inst.transform.SetParent ( PoolManager.instance.objectsContainer.transform );
		inst.SetActive ( activateObject );
		return inst;
	}

	public void ReloadPool ( )
	{
		int objectsDelta = poolSize - pooledObjects.Count;
		// if new size is bigger
		// we need to add objects
		if ( objectsDelta > 0 )
		{
			for ( int i = 0 ; i < objectsDelta ; i++ )
			{
				AddObjectToPool ( );
			}
		}
		// if new size is smaller
		// we need to delete 
		else if ( objectsDelta < 0 )
		{
			for ( int i = 0 ; i < -objectsDelta ; i++ )
			{
				MonoBehaviour.Destroy ( pooledObjects [ pooledObjects.Count - 1 ] );
				pooledObjects.RemoveAt ( pooledObjects.Count - 1 );
			}

		}
	}


	/// <summary>
	/// Returns reference to pooled object if it's currently available.
	/// </summary>
	/// <returns>Pooled object or null if pooled object can't be returned or created.</returns>
	public GameObject GetPooledObject ( )
	{
#if UNITY_EDITOR
		if ( PoolManager.instance.debugMode || PoolManager.instance.useCache )
		{
			int itemsUsed = 0;

			for ( int i = 0 ; i < pooledObjects.Count ; i++ )
			{
				if ( pooledObjects [ i ].activeInHierarchy )
				{
					itemsUsed++;
				}
			}

			if ( willGrow )
			{
				itemsUsed++; // adding one extra item which we will return below 
			}

			if ( itemsUsed > maxItemsUsedInOneTime )
			{
				maxItemsUsedInOneTime = itemsUsed;
			}
		}
#endif
		for ( int i = 0 ; i < pooledObjects.Count ; i++ )
		{
			if ( !pooledObjects [ i ].activeInHierarchy )
			{
				return pooledObjects [ i ];
			}
		}

		if ( willGrow )
		{
			return AddObjectToPool ( );
		}

		return null;
	}

	/// <summary>
	/// Returns reference to pooled object if it's currently available.
	/// </summary>
	/// <param name="activateObject">If true object will be set as active.</param>
	/// <returns>Pooled object or null if pooled object can't be returned or created.</returns>
	public GameObject GetPooledObject ( bool activateObject )
	{
#if UNITY_EDITOR
		if ( PoolManager.instance.debugMode || PoolManager.instance.useCache )
		{
			int itemsUsed = 0;

			for ( int i = 0 ; i < pooledObjects.Count ; i++ )
			{
				if ( pooledObjects [ i ].activeInHierarchy )
				{
					itemsUsed++;
				}
			}

			if ( willGrow )
			{
				itemsUsed++; // adding one extra item which we will return below 
			}

			if ( itemsUsed > maxItemsUsedInOneTime )
			{
				maxItemsUsedInOneTime = itemsUsed;
			}
		}
#endif

		for ( int i = 0 ; i < pooledObjects.Count ; i++ )
		{
			if ( !pooledObjects [ i ].activeInHierarchy )
			{
				pooledObjects [ i ].SetActive ( activateObject );

				return pooledObjects [ i ];
			}
		}

		if ( willGrow )
		{
			return AddObjectToPool ( activateObject );

		}

		return null;
	}

	/// <summary>
	/// Returns reference to pooled object if it's currently available.
	/// </summary>
	/// <param name="position">Sets object to specified position.</param>
	/// <param name="activateObject">If true object will be set as active.</param>
	/// <returns>Pooled object or null if pooled object can't be returned or created.</returns>
	public GameObject GetPooledObject ( Vector3 position, bool activateObject = true )
	{
#if UNITY_EDITOR
		if ( PoolManager.instance.debugMode || PoolManager.instance.useCache )
		{
			int itemsUsed = 0;

			for ( int i = 0 ; i < pooledObjects.Count ; i++ )
			{
				if ( pooledObjects [ i ].activeInHierarchy )
				{
					itemsUsed++;
				}
			}

			if ( willGrow )
			{
				itemsUsed++; // adding one extra item which we will return below 
			}

			if ( itemsUsed > maxItemsUsedInOneTime )
			{
				maxItemsUsedInOneTime = itemsUsed;
			}
		}
#endif


		for ( int i = 0 ; i < pooledObjects.Count ; i++ )
		{
			if ( !pooledObjects [ i ].activeInHierarchy )
			{
				pooledObjects [ i ].transform.position = position;
				pooledObjects [ i ].SetActive ( activateObject );

				return pooledObjects [ i ];
			}
		}

		if ( willGrow )
		{
			return AddObjectToPool ( position, activateObject );
		}

		return null;
	}


}
