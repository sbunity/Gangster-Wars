using System.Collections.Generic;
using UnityEngine;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Levels;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine.Serialization;
using Zenject;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Gameplay.Collisions;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Gameplay.Projectiles;
using SBabchuk.Runtime.UI;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("spawnPoints")]
        private List<Transform> _spawnPoints;

        [SerializeField, FormerlySerializedAs("targetsPoints")]
        private List<Transform> _targetPoints;

        [SerializeField, FormerlySerializedAs("background")]
        private SpriteRenderer _background;

        [SerializeField, FormerlySerializedAs("WaveBar")]
        private FilledBarController _waveBar;

        private LevelDatabase _database;
        private PlayerPrefsDatabase _pPrefs;
        private RandomPathPicker _pathPicker;
        private LevelEntityTracker _entityTracker;
        private LevelWaveScheduler _waveScheduler;
        private Level _properties;
        private int _maxEnemyCount;
        private float _fillStep;
        private bool _isLevelFinished;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private IGameFactory _gameFactory;
        private ILevelFlowService _levelFlowService;
        private BarricadeController _barricadeController;
        private SignalSubscriptions _signals;
        private bool _isWaveFull;

        private void OnEnable()
        {
            _signals?.Enable();
        }

        private void OnDisable()
        {
            _signals?.Disable();
            StopLevelScheduling();
        }

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, IGameFactory gameFactory, ILevelFlowService levelFlowService, LevelEntityTracker entityTracker, BarricadeController barricadeController, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _gameFactory = gameFactory;
            _levelFlowService = levelFlowService;
            _entityTracker = entityTracker;
            _barricadeController = barricadeController;
            _signals = new SignalSubscriptions(signalBus)
                .Add<EnemyDiedSignal>(DeleteEnemy)
                .Add<BonusPoppedSignal>(PopBonus)
                .Add<GameFinishedSignal>(HandleGameOver);
            _signals.Enable();
        }

        private void Awake()
        {
            _pathPicker = new RandomPathPicker(_spawnPoints.Count);
            _waveScheduler = new LevelWaveScheduler(SpawnEnemies);
        }

        private void Start()
        {
            _database = _assetProvider.LevelDatabase;
            _pPrefs = _progressService.Preferences;
            Init(_pPrefs.PlayerPrefs.LevelId);
            StartLevel();
        }

        public void Init(int id = 0)
        {
            _properties = _database.GetLevel(id);
            _levelFlowService?.Start(_properties);
            _entityTracker.Clear();
            _maxEnemyCount = 0;

            foreach (Waves wave in _properties.Waves)
            {
                foreach (EnemyOfWave enemies in wave.Enemies)
                {
                    _maxEnemyCount += enemies.CountEnemy;
                }
            }

            _fillStep = 1.0f / _maxEnemyCount;
            _waveScheduler.Initialize(_properties);
        }

        public void Restart()
        {
            Time.timeScale = 1;
        }

        public void StartLevel()
        {
            if (_properties != null)
            {
                _isLevelFinished = false;
                Wave();
            }
            else
            {
                Debug.LogWarning("Level properties are missing.");
            }
        }

        public void ContinueLevel()
        {
            StartLevel();
        }

        public void Wave()
        {
            if (_isLevelFinished)
                return;

            _isWaveFull = false;
            _waveScheduler.Wave();
        }

        public void StartWave(int waveIndex)
        {
            if (_isLevelFinished)
                return;

            _waveScheduler.StartWave(waveIndex);
        }

        public void WaveHandler(int waveIndex, int currentEnemyIndex)
        {
            if (_isLevelFinished)
                return;

            _waveScheduler.WaveHandler(waveIndex, currentEnemyIndex);
        }

        public void SpawnEnemies(EnemyOfWave enemyOfWave)
        {
            if (_isLevelFinished)
                return;

            var path = _pathPicker.Next();
            var enemy = _gameFactory.CreateEnemy(enemyOfWave, _spawnPoints[path], _targetPoints[path]);
            if (enemy != null)
            {
                _entityTracker.AddEnemy(enemy);
                _waveBar.UpdateFilled(_fillStep);
            }
            else
            {
                Debug.LogWarning("EnemyControllerBase is missing from the spawned enemy.");
            }
        }

        private void DeleteEnemy(EnemyDiedSignal signal)
        {
            DeleteEnemy(signal.EnemyId);
        }

        public void DeleteEnemy(int id)
        {
            _entityTracker.RemoveEnemy(id);
            if (_entityTracker.EnemyCount == 0)
            {
                CheckGameOver();
            }
        }

        private void CheckGameOver()
        {
            _isWaveFull = _waveScheduler.IsWaveFull;
            if (_waveScheduler.CurrentWave == _properties.Waves.Count)
            {
                if (_isWaveFull)
                {
                    if (_entityTracker.BonusCount == 0 && _entityTracker.EnemyCount == 0)
                    {
                        var barricadeController = _barricadeController;
                        if (barricadeController == null)
                            return;

                        var healthPercent = (float)barricadeController.CurrentHealth / (float)barricadeController.MaxHealth;
                        _progressService.CompleteCurrentLevel(healthPercent);
                        _levelFlowService.Finish(Panels.Win);
                    }
                }
            }
        }

        private void HandleGameOver(GameFinishedSignal signal)
        {
            HandleGameOver(signal.Panel);
        }

        private void HandleGameOver(Panels panel)
        {
            _isLevelFinished = true;
            StopLevelScheduling();
        }

        private void StopLevelScheduling()
        {
            _waveScheduler?.Stop();
        }

        private void PopBonus(BonusPoppedSignal signal)
        {
            PopBonus(signal.Bonus);
        }

        public void PopBonus(BonusController bonus)
        {
            _entityTracker.RemoveBonus(bonus);
            if (_entityTracker.EnemyCount == 0)
            {
                CheckGameOver();
            }
        }
    }
}
