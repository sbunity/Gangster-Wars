using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using System.Linq;
using System.Text.RegularExpressions;

//Pool module v 1.2.0

[CustomEditor ( typeof ( PoolManager ) )]
sealed internal class PoolManagerEditor : Editor
{
	private List<Pool> poolsList = new List<Pool> ( );
	private List<Rect> poolsRectList = new List<Rect> ( );

	private PoolManager poolManagerRef;
	private Pool selectedPool;
	private Pool newPool = null;

	private bool isNameAllowed = true;
	private bool isNameAlreadyExisting = false;

	private string searchText = string.Empty;
	private string tempRequest = null;
	private string prevNewPoolName = string.Empty;
	private string prevSelectedPoolName = string.Empty;

	private GUIStyle boxStyle = new GUIStyle ( );
	private GUIStyle headerStyle = new GUIStyle ( );
	private GUIStyle bigHeaderStyle = new GUIStyle ( );
	private GUIStyle centeredTextStyle = new GUIStyle ( );

	void OnEnable ( )
	{
		poolManagerRef = ( PoolManager ) target;

		selectedPool = null;
		newPool = null;

		ReloadPoolManager ( );
		InitStyles ( );
	}


	public void InitStyles ( )
	{
		boxStyle.border = new RectOffset ( 5, 5, 4, 4 );
		boxStyle.margin = new RectOffset ( 5, 5, 4, 4 );
		boxStyle.padding = new RectOffset ( 5, 5, 3, 3 );
		boxStyle.richText = true;
		boxStyle.alignment = TextAnchor.MiddleLeft;

		headerStyle.alignment = TextAnchor.MiddleCenter;
		headerStyle.fontStyle = FontStyle.Bold;
		headerStyle.fontSize = 12;

		bigHeaderStyle.alignment = TextAnchor.MiddleCenter;
		bigHeaderStyle.fontStyle = FontStyle.Bold;
		bigHeaderStyle.fontSize = 14;

		centeredTextStyle.alignment = TextAnchor.MiddleCenter;
	}

	public override void OnInspectorGUI ( )
	{
		EditorGUILayout.BeginVertical ( GUI.skin.box );

		poolManagerRef.poolType = ( PoolManager.PoolManagerType ) EditorGUILayout.EnumPopup ( "Pool type: ", poolManagerRef.poolType );

		poolManagerRef.useCache = EditorGUILayout.Toggle ( "Use cache :", poolManagerRef.useCache );
		poolManagerRef.debugMode = EditorGUILayout.Toggle ( "Debug mode: ", poolManagerRef.debugMode );

		poolManagerRef.objectsContainer = ( GameObject ) EditorGUILayout.ObjectField ( "Objects container: ", poolManagerRef.objectsContainer, typeof ( GameObject ), true );

		if ( GUILayout.Button ( "Add pool" ) )
		{
			AddNewPool ( );
		}

		if ( newPool != null )
		{
			EditorGUILayout.BeginVertical ( GUI.skin.box );

			newPool.name = EditorGUILayout.TextField ( "Name: ", newPool.name );

			if ( prevNewPoolName != newPool.name )
			{
				isNameAllowed = IsNameAllowed ( newPool.name );
			}

			if ( !isNameAllowed )
			{
				if ( isNameAlreadyExisting )
				{
					EditorGUILayout.HelpBox ( "Name already exists", MessageType.Warning );
				}
				else
				{
					EditorGUILayout.HelpBox ( "Not allowed name", MessageType.Warning );
				}
			}

			newPool.objectToPool = ( GameObject ) EditorGUILayout.ObjectField ( "Prefab: ", newPool.objectToPool, typeof ( GameObject ), true );
			newPool.poolSize = EditorGUILayout.IntField ( "Pool size: ", newPool.poolSize );
			newPool.willGrow = EditorGUILayout.Toggle ( "Will grow: ", newPool.willGrow );
			newPool.parent = ( Transform ) EditorGUILayout.ObjectField ( "Container: ", newPool.parent, typeof ( Transform ), true );

			if ( GUILayout.Button ( "Confirm" ) )
			{
				ConfirmPoolCreation ( );
			}

			EditorGUILayout.EndVertical ( );
		}

		EditorGUILayout.EndVertical ( );


		EditorGUILayout.BeginVertical ( );

		if ( poolsList.Count == 0 )
		{
			EditorGUILayout.LabelField ( "There's no pools yet.", headerStyle );
		}
		else
		{
			EditorGUILayout.LabelField ( "Pool objects", headerStyle );

			if ( tempRequest != searchText )
			{
				UpdatePools ( );

				tempRequest = searchText;
			}
			searchText = EditorGUILayout.TextField ( searchText );

			for ( int i = 0 ; i < poolsList.Count ; i++ )
			{
				poolsRectList [ i ] = EditorGUILayout.BeginVertical ( GUI.skin.box );
				EditorGUI.indentLevel++;
				if ( selectedPool == null || poolsList [ i ].name != selectedPool.name )
				{
					EditorGUILayout.LabelField ( poolsList [ i ].name, centeredTextStyle );
				}
				else
				{
					GUILayout.Space ( 5 );
					poolsList [ i ].name = EditorGUILayout.TextField ( "Name: ", poolsList [ i ].name );
					poolsList [ i ].objectToPool = ( GameObject ) EditorGUILayout.ObjectField ( "Prefab: ", poolsList [ i ].objectToPool, typeof ( GameObject ), true );
					poolsList [ i ].poolSize = EditorGUILayout.IntField ( "Pool size: ", poolsList [ i ].poolSize );
					poolsList [ i ].willGrow = EditorGUILayout.Toggle ( "Will grow: ", poolsList [ i ].willGrow );
					poolsList [ i ].parent = ( Transform ) EditorGUILayout.ObjectField ( "Container: ", poolsList [ i ].parent, typeof ( Transform ), true );

					GUILayout.Space ( 5 );

					if ( GUILayout.Button ( "Delete" ) )
					{
						DeletePool ( poolsList [ i ] );
					}

					GUILayout.Space ( 5 );
				}

				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical ( );
			}
		}

		EditorGUILayout.EndVertical ( );

		if ( Event.current.isMouse && Event.current.type == EventType.MouseDown )
		{
			for ( int i = 0 ; i < poolsRectList.Count ; i++ )
			{
				if ( poolsRectList [ i ].Contains ( Event.current.mousePosition ) )
				{
					if ( selectedPool == null || selectedPool != poolsList [ i ] )
					{
						selectedPool = poolsList [ i ];
						newPool = null;
					}
					else
					{
						selectedPool = null;
					}
					Event.current.Use ( );
				}
			}
		}
	}
	
	private void AddNewPool ( )
	{
		newPool = new Pool ( );

		isNameAllowed = IsNameAllowed ( newPool.name );
		isNameAllowed = true; // to prevent warning message when just created pool (there will be new check on confirm method)
	}

	private void ConfirmPoolCreation ( )
	{
		if ( IsNameAllowed ( newPool.name ) )
		{
			poolManagerRef.pools.Add ( newPool );
			ReloadPoolManager ( true );
			newPool = null;
			prevNewPoolName = string.Empty;
		}
	}

	private void DeletePool ( Pool poolToDelete )
	{
		poolManagerRef.pools.Remove ( poolToDelete );
		selectedPool = null;

		ReloadPoolManager ( );
	}

	private bool IsNameAllowed ( string nameToCheck )
	{
		if ( nameToCheck.Equals ( string.Empty ) )
		{
			isNameAllowed = false;
			isNameAlreadyExisting = false;
			return false;
		}

		if ( poolManagerRef.pools.Find ( x => x.name.Equals ( nameToCheck ) ) != null )
		{
			isNameAllowed = false;
			isNameAlreadyExisting = true;
			return false;
		}
		else
		{
			isNameAllowed = true;
			isNameAlreadyExisting = false;
			return true;
		}
	}

	private void ReloadPoolManager ( bool sortPool = false )
	{
		poolsList.Clear ( );

		UpdatePools ( sortPool );

		poolsRectList.Clear ( );
		for ( int i = 0 ; i < poolsList.Count ; i++ )
		{
			poolsRectList.Add ( new Rect ( ) );
		}
	}

	private void UpdatePools ( bool needToSort = false )
	{
		if ( needToSort )
		{
			poolManagerRef.pools.Sort ( ( x, y ) => x.name.CompareTo ( y.name ) );
		}

		if ( poolManagerRef.pools != null )
		{
			poolsList = poolManagerRef.pools.FindAll ( x => x.name.IndexOf ( searchText, StringComparison.OrdinalIgnoreCase ) >= 0 );
		}
	}
}