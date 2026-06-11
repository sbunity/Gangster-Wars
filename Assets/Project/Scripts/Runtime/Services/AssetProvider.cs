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
            var asset = LoadAsset<T>();
            if (asset == null)
                throw new MissingReferenceException("Required database asset was not found: " + typeof(T).Name);

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
        {
            switch (pool)
            {
                case NamesPool.Enemies:
                    return "Enemies";
                case NamesPool.Bullets:
                    return "Bullets";
                case NamesPool.Grenades:
                    return "Grenades";
                case NamesPool.Collisions:
                    return "Collisions";
                case NamesPool.Bonuses:
                    return "Bonuses";
                default:
                    return string.Empty;
            }
        }

        private string GetPrefabName(NamesPool pool, int id)
        {
            switch (pool)
            {
                case NamesPool.Enemies:
                    return "Enemy_" + (id + 1);
                case NamesPool.Bullets:
                    return "Bullet_" + (id + 1);
                case NamesPool.Grenades:
                    return "Grenade_" + (id + 1);
                case NamesPool.Collisions:
                    return "Collision_" + (id + 1);
                case NamesPool.Bonuses:
                    return "Bonus_" + (id + 1);
                default:
                    return string.Empty;
            }
        }
    }
}
