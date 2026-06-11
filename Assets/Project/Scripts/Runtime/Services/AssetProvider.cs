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
