using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace SBabchuk
{
    public class LevelController : MonoBehaviour
    {
        private const int AmmoBonusDropChance = 35;

        public delegate void GameOver(Panels _panel);
        public static event GameOver OnGameOver;

        public delegate void Event(float _value);
        public static event Event OnUpdateCountEnemies;

        [Header("Інстенс")]
        public static LevelController Instance;

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

        /// <summary>
        /// Чи всі вже вороги із хвилі проспавнились
        /// </summary>
        public bool waveIsFull;

        /// + Для конструктора

        //Щоб отримати рівень
        [HideInInspector] public static int c_levelID;

        //Щоб отримати рівень
        [HideInInspector] public static int c_currentWaveID;

        /// - Для конструктора

        public void OnEnable()
        {
            EnemyControllerBase.OnDie += DeleteEnemy;
            BonusController.OnPoped += PopedBonus;
            BarricadeController.OnGameOver += HandleGameOver;
        }

        public void OnDisable()
        {
            EnemyControllerBase.OnDie -= DeleteEnemy;
            BonusController.OnPoped -= PopedBonus;
            BarricadeController.OnGameOver -= HandleGameOver;

            StopSpawnTweens();
        }

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        public void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }

            //Список потрібний, щоб був точний рандом
            listRandomPath = new int[] { 0, 1, 2, 3, 4, 5 };
        }

        /// <summary>
		/// Старт
		/// </summary>
		public void Start()
        {
            database = PersistableSO.Instance.Level;

            pPrefs = PersistableSO.Instance.PlayerPrefs;

            Debug.Log(pPrefs.PlayerPrefs.levelID);
            c_levelID = pPrefs.PlayerPrefs.levelID;

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

            background.sprite = properties.ico;

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
            //SceneLoader.Instance.SwitchToScene(Scenes.Game);
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
                c_currentWaveID = currentWave;
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

            EnemyControllerBase enemy = (GetPool(_enemyOfWave.enemyID, NamesPool.Enemies).GetPooledObject()).GetComponent<EnemyControllerBase>();

            if (enemy != null)
            {
                enemies.Add(enemy);

                int path = GetRandomPath();

                enemy.Init(_enemyOfWave, spawnPoints[path], targetsPoints[path], _enemyOfWave.changeCraft);

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
                        PersistableSO.Instance.PlayerPrefs.SetLevelCompleted((float)BarricadeController.Instance.currentHealth/(float)BarricadeController.Instance.maxHealth);

                        HandleGameOver(Panels.Win);
                        OnGameOver?.Invoke(Panels.Win);
                    }
                }
            }
        }

        private void HandleGameOver(Panels _panel)
        {
            isLevelFinished = true;
            StopSpawnTweens();
        }

        private void StopSpawnTweens()
        {
            Utils.StopTween(twn);
            Utils.StopTween(twn1);
            Utils.StopTween(twn2);
            Utils.StopTween(twn3);
        }

        /// <summary>
        /// Видалити бонус, який взяли
        /// </summary>
        /// <param name="_id"></param>
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
            GrenadeController grenade = (GetPool((int)_grenadesName, NamesPool.Grenades).GetPooledObject()).GetComponent<GrenadeController>();

            if (grenade != null)
            {
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
            CollisionController _collision = (GetPool(_collisionID, NamesPool.Collisions).GetPooledObject()).GetComponent<CollisionController>();

            if (_collision != null)
            {
                if (_properties != null)
                {
                    _collision.Init(_position: _position, _damage: _properties.damage, _radius: _properties.radius, _time: _properties.time);
                }
                else
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

            BonusController _bonus = (GetPool(bonusID, NamesPool.Bonuses).GetPooledObject()).GetComponent<BonusController>();

            if (_bonus != null)
            {
                bonuses.Add(_bonus);

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
            if (PersistableSO.Instance == null || PersistableSO.Instance.PlayerPrefs == null)
                return false;

            if (bonusID >= 0 && bonusID <= 3)
            {
                WeaponShortInfo weaponInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo(bonusID + 1);
                return weaponInfo != null && weaponInfo.isBuy == mySwitch.On;
            }

            if (bonusID >= 4 && bonusID <= 7)
            {
                GrenadeShortInfo grenadeInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetGrenadeShortInfo(bonusID - 4);
                return grenadeInfo != null && grenadeInfo.isBuy == mySwitch.On;
            }

            return false;
        }

        /// <summary>
        /// Спавними пулю
        /// </summary>
        public void SpawnBullet(int _bulletId, int _damage = 0, Vector3 _position = default(Vector3), Vector3 _target = default(Vector3), float _offset = 0, string _tag = "Bullet")
        {
            BaseBulletController _bullet = (GetPool(_bulletId, NamesPool.Bullets).GetPooledObject()).GetComponent<BaseBulletController>();

            if (_bullet != null)
            {
                _bullet.transform.tag = _tag; //Ставив тег, щоб знати що це пуля героя

                _bullet.Init(_bulletId, _damage, _position, _target, _offset); //Ініціалізація пулі
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
            if (_pool == NamesPool.Enemies)
                return PoolManager.GetPoolByName("Enemy_" + (_ID + 1));
            else if (_pool == NamesPool.Grenades)
                return PoolManager.GetPoolByName("Grenade_" + (_ID + 1));
            else if (_pool == NamesPool.Collisions)
                return PoolManager.GetPoolByName("Collision_" + (_ID + 1));
            else if (_pool == NamesPool.Bonuses)
                return PoolManager.GetPoolByName("Bonus_" + (_ID + 1));
            else if (_pool == NamesPool.Bullets)
                return PoolManager.GetPoolByName("Bullet_" + (_ID + 1));
            else
                return PoolManager.GetPoolByName("Enemy_" + (_ID + 1));
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
