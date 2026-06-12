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
    public class LevelController : MonoBehaviour, ILevelSpawnService, IEnemyTargetProvider
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
        private IBonusDropService _bonusDropService;
        private BarricadeController _barricadeController;
        private SignalBus _signalBus;
        private bool _isSubscribedToSignals;
        private bool _isWaveFull;

        private void OnEnable()
        {
            SubscribeSignals();
        }

        private void OnDisable()
        {
            UnsubscribeSignals();
            StopLevelScheduling();
        }

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, IGameFactory gameFactory, ILevelFlowService levelFlowService, IBonusDropService bonusDropService, BarricadeController barricadeController, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _gameFactory = gameFactory;
            _levelFlowService = levelFlowService;
            _bonusDropService = bonusDropService;
            _barricadeController = barricadeController;
            _signalBus = signalBus;
            SubscribeSignals();
        }

        private void Awake()
        {
            _pathPicker = new RandomPathPicker(_spawnPoints.Count);
            _entityTracker = new LevelEntityTracker();
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

        private void SubscribeSignals()
        {
            if (_isSubscribedToSignals || _signalBus == null)
                return;

            _signalBus.Subscribe<EnemyDiedSignal>(DeleteEnemy);
            _signalBus.Subscribe<BonusPoppedSignal>(PopBonus);
            _signalBus.Subscribe<GameFinishedSignal>(HandleGameOver);
            _isSubscribedToSignals = true;
        }

        private void UnsubscribeSignals()
        {
            if (!_isSubscribedToSignals || _signalBus == null)
                return;

            _signalBus.Unsubscribe<EnemyDiedSignal>(DeleteEnemy);
            _signalBus.Unsubscribe<BonusPoppedSignal>(PopBonus);
            _signalBus.Unsubscribe<GameFinishedSignal>(HandleGameOver);
            _isSubscribedToSignals = false;
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

        public Transform GetRandomEnemy()
        {
            return _entityTracker.GetRandomEnemy();
        }

        public void SpawnGrenadeOnPlace(GrenadesName grenadesName, Vector3 position)
        {
            var grenade = _gameFactory.CreateGrenade(grenadesName, position);
            if (grenade == null)
                Debug.LogWarning("GrenadeController is missing from the spawned grenade.");
        }

        public void SpawnCollision(int collisionId, Vector3 position, Grenade grenade = null)
        {
            var collision = _gameFactory.CreateCollision(collisionId, position, grenade);
            if (collision == null)
                Debug.LogWarning("CollisionController is missing from the spawned collision.");
        }

        public void SpawnBonus(Vector3 position)
        {
            var bonusID = _bonusDropService.GetAvailableBonusId();
            if (bonusID < 0)
                return;

            var bonus = _gameFactory.CreateBonus(bonusID, position);
            if (bonus != null)
                _entityTracker.AddBonus(bonus);
            else
                Debug.LogWarning("BonusController is missing from the spawned bonus.");
        }

        public void SpawnBullet(int bulletId, int damage = 0, Vector3 position = default(Vector3), Vector3 target = default(Vector3), float offset = 0, string tag = "Bullet")
        {
            var bullet = _gameFactory.CreateBullet(bulletId, damage, position, target, offset, tag);
            if (bullet == null)
                Debug.LogWarning("BaseBulletController is missing from the spawned bullet.");
        }
    }
}
