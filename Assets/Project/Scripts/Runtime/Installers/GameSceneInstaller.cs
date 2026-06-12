using System.Collections.Generic;
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
        [SerializeField] private List<Transform> _enemySpawnPoints = new();
        [SerializeField] private List<Transform> _enemyTargetPoints = new();

        public override void InstallBindings()
        {
            ValidateReferences();

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

        private void ValidateReferences()
        {
            ValidateRequired(_levelController, nameof(_levelController));
            ValidateRequired(_barricadeController, nameof(_barricadeController));
            ValidateRequired(_leaderController, nameof(_leaderController));
            ValidateRequired(_handController, nameof(_handController));
            ValidateRequired(_poolManager, nameof(_poolManager));
            ValidateRequired(_waveBar, nameof(_waveBar));

            if (_enemySpawnPoints == null || _enemySpawnPoints.Count == 0)
                Debug.LogError($"{nameof(GameSceneInstaller)} has no enemy spawn points assigned.", this);

            if (_enemyTargetPoints == null || _enemyTargetPoints.Count == 0)
                Debug.LogError($"{nameof(GameSceneInstaller)} has no enemy target points assigned.", this);

            if (_enemySpawnPoints != null && _enemyTargetPoints != null && _enemySpawnPoints.Count != _enemyTargetPoints.Count)
                Debug.LogError($"{nameof(GameSceneInstaller)} has mismatched spawn/target point counts: {_enemySpawnPoints.Count}/{_enemyTargetPoints.Count}.", this);
        }

        private void ValidateRequired(Object reference, string fieldName)
        {
            if (reference == null)
                Debug.LogError($"{nameof(GameSceneInstaller)} missing required reference: {fieldName}.", this);
        }

        private void BindIfAssigned<T>(T instance) where T : Object
        {
            if (instance != null)
                Container.Bind<T>().FromInstance(instance).AsSingle();
            if (instance is ILevelSpawnService levelSpawnService)
                Container.Bind<ILevelSpawnService>().FromInstance(levelSpawnService).AsSingle();
            if (instance is IEnemyTargetProvider enemyTargetProvider)
                Container.Bind<IEnemyTargetProvider>().FromInstance(enemyTargetProvider).AsSingle();
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
