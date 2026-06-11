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
        private const int AMMO_BONUS_DROP_CHANCE = 35;

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
        private int[] _listRandomPath;
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
        private IPoolService _poolService;
        private IGameFactory _gameFactory;
        private ILevelFlowService _levelFlowService;
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
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, IPoolService poolService, IGameFactory gameFactory, ILevelFlowService levelFlowService, BarricadeController barricadeController, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _poolService = poolService;
            _gameFactory = gameFactory;
            _levelFlowService = levelFlowService;
            _barricadeController = barricadeController;
            _signalBus = signalBus;
            SubscribeSignals();
        }

        private void Awake()
        {
            _listRandomPath = new int[]
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
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

            var path = GetRandomPath();
            var enemy = _gameFactory != null ? _gameFactory.CreateEnemy(enemyOfWave, _spawnPoints[path], _targetPoints[path]) : (GetPool(enemyOfWave.EnemyId, NamesPool.Enemies).GetPooledObject()).GetComponent<EnemyControllerBase>();
            if (enemy != null)
            {
                _enemies.Add(enemy);
                if (_gameFactory == null)
                    enemy.Init(enemyOfWave, _spawnPoints[path], _targetPoints[path], enemyOfWave.DropChance);

                _waveBar.UpdateFlled(_fillStep);
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
            _signalBus.Subscribe<BonusPoppedSignal>(PopedBonus);
            _signalBus.Subscribe<GameFinishedSignal>(HandleGameOver);
            _isSubscribedToSignals = true;
        }

        private void UnsubscribeSignals()
        {
            if (!_isSubscribedToSignals || _signalBus == null)
                return;

            _signalBus.Unsubscribe<EnemyDiedSignal>(DeleteEnemy);
            _signalBus.Unsubscribe<BonusPoppedSignal>(PopedBonus);
            _signalBus.Unsubscribe<GameFinishedSignal>(HandleGameOver);
            _isSubscribedToSignals = false;
        }

        private void PopedBonus(BonusPoppedSignal signal)
        {
            PopedBonus(signal.Bonus);
        }

        public void PopedBonus(BonusController bonus)
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
            var grenade = _gameFactory != null ? _gameFactory.CreateGrenade(grenadesName, position) : (GetPool((int)grenadesName, NamesPool.Grenades).GetPooledObject()).GetComponent<GrenadeController>();
            if (grenade != null)
            {
                if (_gameFactory == null)
                    grenade.Init((int)grenadesName, 5, position);
            }
            else
            {
                Debug.LogWarning("GrenadeController is missing from the spawned grenade.");
            }
        }

        public void SpawnCollision(int collisionId, Vector3 position, Grenade grenade = null)
        {
            var collision = _gameFactory != null ? _gameFactory.CreateCollision(collisionId, position, grenade) : (GetPool(collisionId, NamesPool.Collisions).GetPooledObject()).GetComponent<CollisionController>();
            if (collision != null)
            {
                if (_gameFactory == null && grenade != null)
                    collision.Init(position: position, damage: grenade.Damage, radius: grenade.Radius, time: grenade.Time);
                else if (_gameFactory == null)
                    collision.Init(position: position);
            }
            else
            {
                Debug.LogWarning("CollisionController is missing from the spawned collision.");
            }
        }

        public void SpawnBonus(Vector3 position)
        {
            var bonusID = GetAvailableBonusID();
            if (bonusID < 0)
                return;

            var bonus = _gameFactory != null ? _gameFactory.CreateBonus(bonusID, position) : (GetPool(bonusID, NamesPool.Bonuses).GetPooledObject()).GetComponent<BonusController>();
            if (bonus != null)
            {
                _bonuses.Add(bonus);
                if (_gameFactory == null)
                    bonus.Init(position);
            }
            else
            {
                Debug.LogWarning("BonusController is missing from the spawned bonus.");
            }
        }

        private int GetAvailableBonusID()
        {
            var ammoBonuses = new List<int>();
            var otherBonuses = new List<int>();

            for (int bonusID = 0; bonusID < 8; bonusID++)
            {
                if (!CanDropBonus(bonusID))
                    continue;

                if (IsAmmoBonus(bonusID))
                    ammoBonuses.Add(bonusID);
                else
                    otherBonuses.Add(bonusID);
            }

            if (ammoBonuses.Count == 0 && otherBonuses.Count == 0)
                return -1;

            if (ammoBonuses.Count > 0 && Random.Range(0, 100) < AMMO_BONUS_DROP_CHANCE)
                return ammoBonuses[Random.Range(0, ammoBonuses.Count)];

            if (otherBonuses.Count > 0)
                return otherBonuses[Random.Range(0, otherBonuses.Count)];

            return -1;
        }

        private bool IsAmmoBonus(int bonusID)
        {
            return bonusID >= 0 && bonusID <= 3;
        }

        private bool CanDropBonus(int bonusID)
        {
            if (_progressService == null)
                return false;

            if (bonusID >= 0 && bonusID <= 3)
            {
                var weaponInfo = _progressService.GetWeaponShortInfo(bonusID + 1);
                return weaponInfo != null && weaponInfo.IsBuy == mySwitch.On;
            }

            if (bonusID >= 4 && bonusID <= 7)
            {
                var grenadeInfo = _progressService.GetGrenadeShortInfo(bonusID - 4);
                return grenadeInfo != null && grenadeInfo.IsBuy == mySwitch.On;
            }

            return false;
        }

        public void SpawnBullet(int bulletId, int damage = 0, Vector3 position = default(Vector3), Vector3 target = default(Vector3), float offset = 0, string tag = "Bullet")
        {
            var bullet = _gameFactory != null ? _gameFactory.CreateBullet(bulletId, damage, position, target, offset, tag) : (GetPool(bulletId, NamesPool.Bullets).GetPooledObject()).GetComponent<BaseBulletController>();
            if (bullet != null)
            {
                if (_gameFactory == null)
                {
                    bullet.transform.tag = tag;
                    bullet.Init(bulletId, damage, position, target, offset);
                }
            }
            else
            {
                Debug.LogWarning("BaseBulletController is missing from the spawned bullet.");
            }
        }

        public Pool GetPool(int id, NamesPool pool)
        {
            if (_poolService == null)
                return null;
                
            return _poolService.GetPool(pool, id);
        }

        public int GetRandomPath()
        {
            if (_listRandomPath.Length == 0)
            {
                _listRandomPath = new int[]
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5
                };
            }

            var numToRemove = Random.Range(0, _listRandomPath.Length);
            var index = _listRandomPath[numToRemove];
            RemoveAt(ref _listRandomPath, numToRemove);
            return index;
        }

        public void RemoveAt<T>(ref T[] arr, int index)
        {
            for (var i = index; i < arr.Length - 1; i++)
            {
                arr[i] = arr[i + 1];
            }

            System.Array.Resize(ref arr, arr.Length - 1);
        }
    }
}
