using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.Services
{
    public sealed class SaveService : ISaveService, IInitializable
    {
        private readonly IAssetProvider _assetProvider;

        public SaveService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public void Initialize()
        {
            LoadAsync().Forget();
        }

        public UniTask LoadAsync()
        {
            foreach (var asset in GetPersistentAssets())
                LoadAsset(asset);

            return UniTask.CompletedTask;
        }

        public UniTask SaveAsync()
        {
            foreach (var asset in GetPersistentAssets())
                SaveAsset(asset);

            return UniTask.CompletedTask;
        }

        public UniTask SaveAsync(ScriptableObject objectToPersist)
        {
            if (objectToPersist != null)
                SaveAsset(objectToPersist);

            return UniTask.CompletedTask;
        }

        private ScriptableObject[] GetPersistentAssets() 
            => new ScriptableObject[]
            {
                _assetProvider.PlayerPrefsDatabase,
                _assetProvider.WeaponStoreDatabase,
                _assetProvider.BombStoreDatabase,
                _assetProvider.DefenseStoreDatabase,
                _assetProvider.MainPlayerDatabase,
                _assetProvider.EnemyDatabase,
                _assetProvider.LevelDatabase,
                _assetProvider.BulletDatabase
            };

        private void LoadAsset(ScriptableObject asset)
        {
            var path = GetPath(asset);
            if (!File.Exists(path))
                return;

            var formatter = new BinaryFormatter();

            using var file = File.Open(path, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)formatter.Deserialize(file), asset);
        }

        private void SaveAsset(ScriptableObject asset)
        {
            var formatter = new BinaryFormatter();
            using var file = File.Create(GetPath(asset));
            {
                var json = JsonUtility.ToJson(asset);
                formatter.Serialize(file, json);
            }
        }

        private string GetPath(ScriptableObject asset) 
            => Path.Combine(Application.persistentDataPath, $"Main_{asset.name}.pso");
    }
}
