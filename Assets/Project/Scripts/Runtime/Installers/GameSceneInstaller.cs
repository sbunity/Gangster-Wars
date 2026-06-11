using System.Collections.Generic;
using SBabchuk.Runtime.Factories;
using SBabchuk.Runtime.Services;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

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
        [SerializeField] private List<Transform> _enemySpawnPoints = new();
        [SerializeField] private List<Transform> _enemyTargetPoints = new();

        public override void InstallBindings()
        {
            var references = new GameSceneReferences(_levelController, _barricadeController, _leaderController, _handController, _sightController, _poolManager, _waveBar, _enemySpawnPoints, _enemyTargetPoints);
            Container.Bind<GameSceneReferences>().FromInstance(references).AsSingle();
            Container.Bind<PoolManager>().FromInstance(_poolManager).AsSingle();
            Container.Bind<IPoolService>().To<PoolService>().AsSingle();
            Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();
            BindIfAssigned(_levelController);
            BindIfAssigned(_barricadeController);
            BindIfAssigned(_waveBar);
            BindIfAssigned(_leaderController);
            BindIfAssigned(_handController);
            BindIfAssigned(_sightController);

            if (_leaderController != null)
                Container.Bind<ILeaderWeaponController>().FromInstance(_leaderController).AsSingle();
            if (_handController != null)
                Container.Bind<IHandService>().FromInstance(_handController).AsSingle();
            if (_sightController != null)
                Container.Bind<IAimService>().FromInstance(_sightController).AsSingle();
            else
                Container.Bind<IAimService>().To<NullAimService>().AsSingle();
        }

        private void BindIfAssigned<T>(T instance) where T : Object
        {
            if (instance != null)
                Container.Bind<T>().FromInstance(instance).AsSingle();
            if (instance is ILevelRuntimeService levelRuntimeService)
                Container.Bind<ILevelRuntimeService>().FromInstance(levelRuntimeService).AsSingle();
        }
    }

    public sealed class GameSceneReferences
    {
        public GameSceneReferences(LevelController levelController, BarricadeController barricadeController, LeaderGangsterController leaderController, HandController handController, SightController sightController, PoolManager poolManager, FilledBarController waveBar, IReadOnlyList<Transform> enemySpawnPoints, IReadOnlyList<Transform> enemyTargetPoints)
        {
            LevelController = levelController;
            BarricadeController = barricadeController;
            LeaderController = leaderController;
            HandController = handController;
            SightController = sightController;
            PoolManager = poolManager;
            WaveBar = waveBar;
            EnemySpawnPoints = enemySpawnPoints;
            EnemyTargetPoints = enemyTargetPoints;
        }

        public LevelController LevelController { get; }
        public BarricadeController BarricadeController { get; }
        public LeaderGangsterController LeaderController { get; }
        public HandController HandController { get; }
        public SightController SightController { get; }
        public PoolManager PoolManager { get; }
        public FilledBarController WaveBar { get; }
        public IReadOnlyList<Transform> EnemySpawnPoints { get; }
        public IReadOnlyList<Transform> EnemyTargetPoints { get; }
    }
}