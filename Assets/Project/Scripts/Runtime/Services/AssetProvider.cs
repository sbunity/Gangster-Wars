using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;
#endif
namespace SBabchuk.Runtime.Services
{
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly IPoolAssetResolver _poolAssetResolver;

        public AssetProvider(IPoolAssetResolver poolAssetResolver)
        {
            _poolAssetResolver = poolAssetResolver;
        }

        public PlayerPrefsDatabase PlayerPrefsDatabase => LoadRequiredAsset<PlayerPrefsDatabase>();
        public WeaponStoreDatabase WeaponStoreDatabase => LoadRequiredAsset<WeaponStoreDatabase>();
        public BombStoreDatabase BombStoreDatabase => LoadRequiredAsset<BombStoreDatabase>();
        public DefenseStoreDatabase DefenseStoreDatabase => LoadRequiredAsset<DefenseStoreDatabase>();
        public MainPlayerDatabase MainPlayerDatabase => LoadRequiredAsset<MainPlayerDatabase>();
        public EnemyDatabase EnemyDatabase => LoadRequiredAsset<EnemyDatabase>();
        public LevelDatabase LevelDatabase => LoadRequiredAsset<LevelDatabase>();
        public BulletDatabase BulletDatabase => LoadRequiredAsset<BulletDatabase>();

        public T LoadAsset<T>() where T : Object
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

        private T LoadRequiredAsset<T>() where T : Object
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
