using SBabchuk.Runtime.Factories;
using SBabchuk.Runtime.Services;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Characters;
using SBabchuk.Runtime.Gameplay.Levels;
using SBabchuk.Runtime.UI;

namespace SBabchuk.Runtime.Installers
{
    public sealed class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelController _levelController;
        [SerializeField] private BarricadeController _barricadeController;
        [SerializeField] private LeaderGangsterController _leaderController;
        [SerializeField] private HandController _handController;
        [SerializeField] private SightController _sightController;
        [SerializeField] private PoolManager _poolManager;
        [SerializeField] private FilledBarController _waveBar;

        public override void InstallBindings()
        {
            ValidateReferences();

            Container.Bind<PoolManager>().FromInstance(_poolManager).AsSingle();
            Container.Bind<IPoolService>().To<PoolService>().AsSingle();
            Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();

            // Level runtime: command (spawn) and query (targeting) are now separate services
            // sharing a single entity tracker, instead of both living on LevelController.
            Container.Bind<LevelEntityTracker>().AsSingle();
            Container.Bind<IEnemyTargetProvider>().To<LevelEntityTracker>().FromResolve();
            Container.Bind<ILevelSpawnService>().To<LevelSpawnService>().AsSingle();

            BindInstance(_levelController);
            BindInstance(_barricadeController);
            BindInstance(_waveBar);
            BindInstance(_leaderController);
            BindInstance(_handController);
            BindInstance(_sightController);

            if (_leaderController != null)
                Container.Bind<ILeaderWeaponController>().FromInstance(_leaderController).AsSingle();
            if (_handController != null)
                Container.Bind<IHandService>().FromInstance(_handController).AsSingle();
            if (_sightController != null)
                Container.Bind<IAimService>().FromInstance(_sightController).AsSingle();
            else
                Container.Bind<IAimService>().To<NullAimService>().AsSingle();
        }

        private void ValidateReferences()
        {
            ValidateRequired(_levelController, nameof(_levelController));
            ValidateRequired(_barricadeController, nameof(_barricadeController));
            ValidateRequired(_leaderController, nameof(_leaderController));
            ValidateRequired(_handController, nameof(_handController));
            ValidateRequired(_poolManager, nameof(_poolManager));
            ValidateRequired(_waveBar, nameof(_waveBar));
        }

        private void ValidateRequired(Object reference, string fieldName)
        {
            if (reference == null)
                Debug.LogError($"{nameof(GameSceneInstaller)} missing required reference: {fieldName}.", this);
        }

        private void BindInstance<T>(T instance) where T : Object
        {
            if (instance != null)
                Container.Bind<T>().FromInstance(instance).AsSingle();
        }
    }
}
