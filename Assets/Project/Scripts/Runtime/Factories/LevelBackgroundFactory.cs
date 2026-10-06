using SBabchuk.Runtime.Gameplay.Levels;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Factories
{
    public sealed class LevelBackgroundFactory : ILevelBackgroundFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly DiContainer _container;

        public LevelBackgroundFactory(IAssetProvider assetProvider, DiContainer container)
        {
            _assetProvider = assetProvider;
            _container = container;
        }

        public LevelBackground Create(int levelId, Transform parent)
        {
            var prefab = _assetProvider.LevelBackgroundDatabase.GetBackground(levelId);
            if (prefab == null)
                throw new MissingReferenceException($"No background prefab configured for level {levelId}.");

            return _container.InstantiatePrefabForComponent<LevelBackground>(prefab, parent);
        }
    }
}
