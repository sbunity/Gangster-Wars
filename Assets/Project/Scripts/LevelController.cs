using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class LevelController : MonoBehaviour, ILevelRuntimeService
    {
        private const int AmmoBonusDropChance = 35;

        [Header("Точки появи ворогів")]
        public List<Transform> spawnPoints;

        [Header("Точки атаки ворогів")]
        public List<Transform> targetsPoints;

        [Header("Фон")]
        public SpriteRenderer background;

        [Header("База даних")]
        private LevelDatabase database;

        private PlayerPrefsDatabase pPrefs;

        [Header("Список індексів випадкових шляхів")]
        private int[] listRandomPath;// = new int[]{ 0, 1, 2, 3, 4, 5, 6 };

        [Header("Властивості рівня")]
        private Level properties;

        [Header("Поточна хвиля")]
        public int currentWave = 0;

        [Header("Кількість ворогів")]
        private int countEnemies;

        [Header("Скільки юнітів буде на рівні")]
        public int maxCountEnemies;

        [Header("Крок збільшення заповнення за 1 юніта")]
        public float filledStep;

        [Header("Список ворогів на сцені")]
        private List<EnemyControllerBase> enemies = new List<EnemyControllerBase>();

        [Header("Список бонусів на сцені")]
        public List<BonusController> bonuses = new List<BonusController>();

        [Header("Прогрес хвиль")]
        public FilledBarController WaveBar;

        Tween twn;

        Tween twn1;

        Tween twn2;

        Tween twn3;

        private bool isLevelFinished;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private IPoolService _poolService;
        private IGameFactory _gameFactory;
        private ILevelFlowService _levelFlowService;
        private BarricadeController _barricadeController;
        private SignalBus _signalBus;
        private bool _isSubscribedToSignals;

        /// <summary>
        /// Чи всі вже вороги із хвилі проспавнились
        /// </summary>
        public bool waveIsFull;

        public void OnEnable()
        {
            SubscribeSignals();
        }

        public void OnDisable()
        {
            UnsubscribeSignals();

            StopSpawnTweens();
        }

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            IPoolService poolService,
            IGameFactory gameFactory,
            ILevelFlowService levelFlowService,
            BarricadeController barricadeController,
            SignalBus signalBus)
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

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        public void Awake()
        {
            //Список потрібний, щоб був точний рандом
            listRandomPath = new int[] { 0, 1, 2, 3, 4, 5 };
        }

        /// <summary>
		/// Старт
		/// </summary>
		public void Start()
        {
            database = _assetProvider.LevelDatabase;

            pPrefs = _progressService.Preferences;

            Debug.Log(pPrefs.PlayerPrefs.levelID);
            Init(pPrefs.PlayerPrefs.levelID);

            StartLevel();
        }

        /// <summary>
        /// Витягуєм з бази властивості левела
        /// </summary>
        /// <param name="_id">Identifier.</param>
        public void Init(int _id = 0)
        {
            properties = database.GetLevel(_id);
            _levelFlowService?.Start(properties);

            // The scene's Background SpriteRenderer holds the designed level background.
            // The Level.ico field is the level-select icon (small UI sprites, some missing),
            // so it must NOT overwrite the background here.

            foreach (Waves wave in properties.waves)
            {
                foreach (EnemyOfWave enemies in wave.enemies)
                {
                    maxCountEnemies += enemies.countEnemy;
                }
            }

            filledStep = 1.0f / maxCountEnemies;
        }

        /// <summary>
        /// Рестарт левела
        /// </summary>
        public void Restart()
        {
            Time.timeScale = 1;
        }

        /// <summary>
		/// Стартує рівень
		/// </summary>
		/// <param name="_id">Identifier.</param>
        public void StartLevel()
        {
            if (properties != null)
            {
                isLevelFinished = false;
                Wave();
            }
            else
            {
                print("@@@@@@@@@@ немає левела");
            }
        }

        /// <summary>
        /// Повернення до проходження
        /// </summary>
        public void ContinueLevel() // ДЛЯ КОНСТРУКТОРА
        {
            StartLevel();
        }

        /// <summary>
		/// Хвиля
		/// </summary>
        public void Wave()
        {
            if (isLevelFinished)
                return;

            if (currentWave < properties.waves.Count)
            {
                Debug.Log(Time.time + ": " + "@@ Хвиля(" + (currentWave + 1) + "/" + properties.waves.Count + ")");

                waveIsFull = false;

                twn = DOVirtual.DelayedCall(properties.waves[currentWave].startDelay, () =>
                { //стартуєм нову хвилю після часу затримки хвилі
                    if (isLevelFinished)
                        return;

                    StartWave(currentWave);
                }).SetUpdate(false);
            }
            else
            {
                print(Time.time + ": " + "@@ Хвиль більше немає");
            }
        }

        /// <summary>
		/// Запуск хвилі
		/// </summary>
		/// <param name="_wave">Wave.</param>
        public void StartWave(int _wave)
        {
            if (isLevelFinished)
                return;

            //OnWaveTime(properties.waves[_wave].delay);

            twn1 = DOVirtual.DelayedCall(properties.waves[_wave].delay, () =>
            { //стартуєм час до закінчення виділеного часу на проходження хвилі
                if (isLevelFinished)
                    return;

                currentWave++;
                Wave();
            }).SetUpdate(false);

            WaveHandler(_wave, 0);
        }

        /// <summary>
        /// Спавнимо юнітів для хвилі
        /// </summary>
        /// <param name="_wave"></param>
        /// <param name="_currentEnemy"></param>
        public void WaveHandler(int _wave, int _currentEnemy)
        {
            if (isLevelFinished)
                return;

            if (_currentEnemy < properties.waves[_wave].enemies.Count)
            {
                waveIsFull = false;

                EnemyOfWave enemyOfWave = properties.waves[_wave].enemies[_currentEnemy];

                twn2 = DOVirtual.DelayedCall(0, () =>
                {
                    if (!isLevelFinished)
                        SpawnEnemies(enemyOfWave);
                })
                .SetLoops(enemyOfWave.countEnemy)
                .OnComplete(() =>
                {
                    if (isLevelFinished)
                        return;

                    int nextEnemy = _currentEnemy + 1;
                  
                    waveIsFull = nextEnemy == properties.waves[_wave].enemies.Count;

                    if (!waveIsFull)
                    {
                        twn3 = DOVirtual.DelayedCall(properties.waves[_wave].enemies[nextEnemy].interval, () =>
                        {
                            WaveHandler(_wave, nextEnemy);
                        }).SetUpdate(false);
                    }
                }).SetUpdate(false);
            }
        }

        /// <summary>
        /// Створюєм юніта 
        /// </summary>
        public void SpawnEnemies(EnemyOfWave _enemyOfWave)
        {
            if (isLevelFinished)
                return;

            int path = GetRandomPath();

            EnemyControllerBase enemy = _gameFactory != null
                ? _gameFactory.CreateEnemy(_enemyOfWave, spawnPoints[path], targetsPoints[path])
                : (GetPool(_enemyOfWave.enemyID, NamesPool.Enemies).GetPooledObject()).GetComponent<EnemyControllerBase>();

            if (enemy != null)
            {
                enemies.Add(enemy);

                if (_gameFactory == null)
                {
                    enemy.Init(_enemyOfWave, spawnPoints[path], targetsPoints[path], _enemyOfWave.changeCraft);
                }

                WaveBar.UpdateFlled(filledStep);
            }
            else
            {
                Debug.Log("EnemyControllerBase == null");
            }
        }

        /// <summary>
        /// Видалити ворога, який помер
        /// </summary>
        /// <param name="_id"></param>
        private void DeleteEnemy(EnemyDiedSignal signal)
        {
            DeleteEnemy(signal.EnemyId);
        }

        public void DeleteEnemy(int _id)
        {
            enemies.Remove(enemies.Find(enemy => enemy.properties.id == _id));

            if (enemies.Count == 0)
            {
                CheckGameOver();
            }
        }

        /// <summary>
        /// Перевірка чи можн азавершувати гру
        /// </summary>
        private void CheckGameOver()
        {
            if (currentWave == properties.waves.Count)
            {
                if (waveIsFull)
                {
                    if (bonuses.Count == 0 && enemies.Count == 0)
                    {
                        var barricadeController = _barricadeController;
                        if (barricadeController == null)
                            return;

                        var healthPercent = (float)barricadeController.currentHealth/(float)barricadeController.maxHealth;

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

        private void HandleGameOver(Panels _panel)
        {
            isLevelFinished = true;
            StopSpawnTweens();
        }

        private void StopSpawnTweens()
        {
            twn?.Kill();
            twn1?.Kill();
            twn2?.Kill();
            twn3?.Kill();
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

        /// <summary>
        /// Видалити бонус, який взяли
        /// </summary>
        /// <param name="_id"></param>
        private void PopedBonus(BonusPoppedSignal signal)
        {
            PopedBonus(signal.Bonus);
        }

        public void PopedBonus(BonusController _value)
        {
            bonuses.Remove(_value);

            if (enemies.Count == 0)
            {
                CheckGameOver();
            }
        }

        /// <summary>
        /// Oтримати Трансформ для випадкового ворога
        /// </summary>
        /// <returns></returns>
        public Transform GetRandomEnemy()
        {
            return enemies[Random.Range(0, enemies.Count)].center.GetTransform();
        }

        /// <summary>
        /// Спавними відповідну бомбу на позиції, що отримали
        /// </summary>
        /// <param name="_grenadesName"></param>
        /// <param name="_position"></param>
        public void SpawnGrenadeOnPlace(GrenadesName _grenadesName, Vector3 _position)
        {
            GrenadeController grenade = _gameFactory != null
                ? _gameFactory.CreateGrenade(_grenadesName, _position)
                : (GetPool((int)_grenadesName, NamesPool.Grenades).GetPooledObject()).GetComponent<GrenadeController>();

            if (grenade != null)
            {
                if (_gameFactory == null)
                    grenade.Init((int)_grenadesName, 5, _position);
            }
            else
            {
                Debug.Log("grenade == null");
            }
        }

        /// <summary>
        /// Спавними відповідну анімації зриву
        /// </summary>
        public void SpawnCollision(int _collisionID, Vector3 _position, Grenade _properties = null)
        {
            CollisionController _collision = _gameFactory != null
                ? _gameFactory.CreateCollision(_collisionID, _position, _properties)
                : (GetPool(_collisionID, NamesPool.Collisions).GetPooledObject()).GetComponent<CollisionController>();

            if (_collision != null)
            {
                if (_gameFactory == null && _properties != null)
                {
                    _collision.Init(_position: _position, _damage: _properties.damage, _radius: _properties.radius, _time: _properties.time);
                }
                else if (_gameFactory == null)
                {
                    _collision.Init(_position: _position);
                }
            }
            else
            {
                Debug.Log("_collision == null");
            }
        }

        /// <summary>
        /// Спавними відповідну анімації зриву
        /// </summary>
        public void SpawnBonus(Vector3 _position)
        {
            int bonusID = GetAvailableBonusID();

            if (bonusID < 0)
                return;

            BonusController _bonus = _gameFactory != null
                ? _gameFactory.CreateBonus(bonusID, _position)
                : (GetPool(bonusID, NamesPool.Bonuses).GetPooledObject()).GetComponent<BonusController>();

            if (_bonus != null)
            {
                bonuses.Add(_bonus);

                if (_gameFactory == null)
                    _bonus.Init(_position);
            }
            else
            {
                Debug.Log("_collision == null");
            }
        }

        private int GetAvailableBonusID()
        {
            List<int> ammoBonuses = new List<int>();
            List<int> otherBonuses = new List<int>();

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

            if (ammoBonuses.Count > 0 && Random.Range(0, 100) < AmmoBonusDropChance)
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
                WeaponShortInfo weaponInfo = _progressService.GetWeaponShortInfo(bonusID + 1);
                return weaponInfo != null && weaponInfo.isBuy == mySwitch.On;
            }

            if (bonusID >= 4 && bonusID <= 7)
            {
                GrenadeShortInfo grenadeInfo = _progressService.GetGrenadeShortInfo(bonusID - 4);
                return grenadeInfo != null && grenadeInfo.isBuy == mySwitch.On;
            }

            return false;
        }

        /// <summary>
        /// Спавними пулю
        /// </summary>
        public void SpawnBullet(int _bulletId, int _damage = 0, Vector3 _position = default(Vector3), Vector3 _target = default(Vector3), float _offset = 0, string _tag = "Bullet")
        {
            BaseBulletController _bullet = _gameFactory != null
                ? _gameFactory.CreateBullet(_bulletId, _damage, _position, _target, _offset, _tag)
                : (GetPool(_bulletId, NamesPool.Bullets).GetPooledObject()).GetComponent<BaseBulletController>();

            if (_bullet != null)
            {
                if (_gameFactory == null)
                {
                    _bullet.transform.tag = _tag; //Ставив тег, щоб знати що це пуля героя

                    _bullet.Init(_bulletId, _damage, _position, _target, _offset); //Ініціалізація пулі
                }
            }
            else
            {
                Debug.Log("_bullet == null");
            }
        }

        /// <summary>
        /// Вибираєм пулл і повертаєм ім*я префаба
        /// </summary>
        /// <param name="_enemyID"></param>
        /// <param name="_pool"></param>
        /// <returns></returns>
        public Pool GetPool(int _ID, NamesPool _pool)
        {
            if (_poolService != null)
                return _poolService.GetPool(_pool, _ID);

            return null;
        }

        #region GetRandomPath
        /// <summary>
        /// Отримати рандомний шлях
        /// </summary>
        /// <returns>The random path.</returns>
        /// <param name="wManager">W manager.</param>
        public int GetRandomPath()
        {
            if (listRandomPath.Length == 0)
            {
                listRandomPath = new int[] { 0, 1, 2, 3, 4, 5 };
            }

            var numToRemove = Random.Range(0, listRandomPath.Length);
            int index = listRandomPath[numToRemove];
            RemoveAt(ref listRandomPath, numToRemove);
            return index;
        }

        /// <summary>
        /// Видалити певний елемент
        /// </summary>
        /// <param name="arr">Arr.</param>
        /// <param name="index">Index.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }

            System.Array.Resize(ref arr, arr.Length - 1);
        }
        #endregion
    }
}
