using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SBabchuk.Runtime.Services
{
    public sealed class AssetProvider : IAssetProvider
    {
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
            var folder = GetPrefabFolder(pool);
            if (string.IsNullOrEmpty(folder))
                return null;
            return Resources.Load<GameObject>("Prefabs/" + folder + "/" + GetPrefabName(pool, id));
        }

        private string GetPrefabFolder(NamesPool pool) 
            => pool switch
            {
                NamesPool.Enemies => "Enemies",
                NamesPool.Bullets => "Bullets",
                NamesPool.Grenades => "Grenades",
                NamesPool.Collisions => "Collisions",
                NamesPool.Bonuses => "Bonuses",
                _ => string.Empty,
            };

        private string GetPrefabName(NamesPool pool, int id) 
            => pool switch
            {
                NamesPool.Enemies => "Enemy_" + (id + 1),
                NamesPool.Bullets => "Bullet_" + (id + 1),
                NamesPool.Grenades => "Grenade_" + (id + 1),
                NamesPool.Collisions => "Collision_" + (id + 1),
                NamesPool.Bonuses => "Bonus_" + (id + 1),
                _ => string.Empty,
            };
    }
}
