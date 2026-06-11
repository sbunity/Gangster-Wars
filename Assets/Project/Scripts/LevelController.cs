using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine.Serialization;
using Zenject;

namespace SBabchuk
{
    public class LevelController : MonoBehaviour, ILevelRuntimeService
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
        private Level _properties;
        private int _currentWave;
        private int _maxEnemyCount;
        private float _fillStep;
        private List<EnemyControllerBase> _enemies = new();
        private List<BonusController> _bonuses = new();
        private Tween _waveDelayTween;
        private Tween _nextWaveTween;
        private Tween _enemySpawnTween;
        private Tween _nextEnemyGroupTween;
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
            StopSpawnTweens();
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

            foreach (Waves wave in _properties.Waves)
            {
                foreach (EnemyOfWave enemies in wave.Enemies)
                {
                    _maxEnemyCount += enemies.CountEnemy;
                }
            }

            _fillStep = 1.0f / _maxEnemyCount;
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

            if (_currentWave < _properties.Waves.Count)
            {
                _isWaveFull = false;
                _waveDelayTween = DOVirtual.DelayedCall(_properties.Waves[_currentWave].StartDelay, () =>
                {
                    if (_isLevelFinished)
                        return;

                    StartWave(_currentWave);
                }).SetUpdate(false);
            }
            else
            {
                Debug.LogWarning("There are no more waves to start.");
            }
        }

        public void StartWave(int waveIndex)
        {
            if (_isLevelFinished)
                return;

            _nextWaveTween = DOVirtual.DelayedCall(_properties.Waves[waveIndex].Delay, () =>
            {
                if (_isLevelFinished)
                    return;

                _currentWave++;
                Wave();
            }).SetUpdate(false);
            WaveHandler(waveIndex, 0);
        }

        public void WaveHandler(int waveIndex, int currentEnemyIndex)
        {
            if (_isLevelFinished)
                return;

            if (currentEnemyIndex < _properties.Waves[waveIndex].Enemies.Count)
            {
                _isWaveFull = false;
                var enemyOfWave = _properties.Waves[waveIndex].Enemies[currentEnemyIndex];
                _enemySpawnTween = DOVirtual.DelayedCall(0, () =>
                {
                    if (!_isLevelFinished)
                        SpawnEnemies(enemyOfWave);
                }).SetLoops(enemyOfWave.CountEnemy).OnComplete(() =>
                {
                    if (_isLevelFinished)
                        return;

                    var nextEnemy = currentEnemyIndex + 1;
                    _isWaveFull = nextEnemy == _properties.Waves[waveIndex].Enemies.Count;
                    if (!_isWaveFull)
                    {
                        _nextEnemyGroupTween = DOVirtual.DelayedCall(_properties.Waves[waveIndex].Enemies[nextEnemy].Interval, () =>
                        {
                            WaveHandler(waveIndex, nextEnemy);
                        }).SetUpdate(false);
                    }
                }).SetUpdate(false);
            }
        }

        public void SpawnEnemies(EnemyOfWave enemyOfWave)
        {
            if (_isLevelFinished)
                return;

            var path = _pathPicker.Next();
            var enemy = _gameFactory.CreateEnemy(enemyOfWave, _spawnPoints[path], _targetPoints[path]);
            if (enemy != null)
            {
                _enemies.Add(enemy);
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
            _enemies.Remove(_enemies.Find(enemy => enemy.Properties.Id == id));
            if (_enemies.Count == 0)
            {
                CheckGameOver();
            }
        }

        private void CheckGameOver()
        {
            if (_currentWave == _properties.Waves.Count)
            {
                if (_isWaveFull)
                {
                    if (_bonuses.Count == 0 && _enemies.Count == 0)
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
            StopSpawnTweens();
        }

        private void StopSpawnTweens()
        {
            _waveDelayTween?.Kill();
            _nextWaveTween?.Kill();
            _enemySpawnTween?.Kill();
            _nextEnemyGroupTween?.Kill();
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
            _bonuses.Remove(bonus);
            if (_enemies.Count == 0)
            {
                CheckGameOver();
            }
        }

        public Transform GetRandomEnemy()
        {
            return _enemies[Random.Range(0, _enemies.Count)].Center.GetTransform();
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
                _bonuses.Add(bonus);
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
