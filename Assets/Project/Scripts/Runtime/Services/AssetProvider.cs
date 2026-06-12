using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SBabchuk.Runtime.Services
{
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly IPoolAssetResolver _poolAssetResolver;
        private readonly Dictionary<Type, UnityEngine.Object> _databaseCache = new();

        public AssetProvider(IPoolAssetResolver poolAssetResolver)
        {
            _poolAssetResolver = poolAssetResolver;
        }

        public PlayerPrefsDatabase PlayerPrefsDatabase => GetDatabase<PlayerPrefsDatabase>();
        public WeaponStoreDatabase WeaponStoreDatabase => GetDatabase<WeaponStoreDatabase>();
        public BombStoreDatabase BombStoreDatabase => GetDatabase<BombStoreDatabase>();
        public DefenseStoreDatabase DefenseStoreDatabase => GetDatabase<DefenseStoreDatabase>();
        public MainPlayerDatabase MainPlayerDatabase => GetDatabase<MainPlayerDatabase>();
        public EnemyDatabase EnemyDatabase => GetDatabase<EnemyDatabase>();
        public LevelDatabase LevelDatabase => GetDatabase<LevelDatabase>();
        public BulletDatabase BulletDatabase => GetDatabase<BulletDatabase>();

        public T LoadAsset<T>() where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var asset = guids.Length > 0
                ? AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]))
                : null;
            if (asset != null)
                return asset;
#endif
            return Resources.Load<T>("Databases/" + typeof(T).Name);
        }

        private T GetDatabase<T>() where T : UnityEngine.Object
        {
            if (_databaseCache.TryGetValue(typeof(T), out var cached) && cached != null)
                return (T)cached;

            var asset = LoadRequiredAsset<T>();
            _databaseCache[typeof(T)] = asset;
            return asset;
        }

        private T LoadRequiredAsset<T>() where T : UnityEngine.Object
        {
            var asset = LoadAsset<T>() ?? throw new MissingReferenceException("Required database asset was not found: " + typeof(T).Name);
            return asset;
        }

        public GameObject LoadPrefab(NamesPool pool, int id)
        {
            var path = _poolAssetResolver.GetPrefabResourcesPath(pool, id);
            if (string.IsNullOrEmpty(path))
                return null;
            return Resources.Load<GameObject>(path);
        }
    }
}
