using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create LevelDatabase", fileName = "LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        [Header("Рівні")]
        [SerializeField, HideInInspector] public List<Level> levels = new List<Level>();

        /// <summary>
        /// Отримуєм рівень
        /// </summary>
        /// <param name="id">ІД шуканого рівня</param>
        /// <returns>Повертаєм рівень</returns>
        public Level GetLevel(int id)
        {
            int index = levels.FindIndex(x => x.id == id);

            return index != -1 ? levels[index] : null;
        }

        /// <summary>
        /// Знаходимо хвилю, для рівня
        /// </summary>
        /// <param name="_level">Рівень</param>
        /// <param name="_waveID">ІД хвилі</param>
        /// <returns>Хвиля</returns>
        public Waves GetWave(Level _level, int _waveID)
        {
            foreach (Waves _waves in _level.waves)
            {
                if (_waves.id == _waveID)
                {
                    return _waves;
                }
            }

            return null;
        }

        /// <summary>
        /// Знаходимо хвилю, для рівня
        /// </summary>
        /// <param name="_levelID">ІД рівня</param>
        /// <param name="_waveID">ІД хвилі</param>
        /// <returns>Хвиля</returns>
        public Waves GetWave(int _levelID, int _waveID)
        {
            Level _level = GetLevel(_levelID);

            foreach (Waves _waves in _level.waves)
            {
                if (_waves.id == _waveID)
                {
                    return _waves;
                }
            }

            return null;
        }

        /// <summary>
        /// Створюєм, для даного рівня нову хвилю
        /// </summary>
        /// <param name="_levelID"> ІД Рівня</param>
        /// <param name="_time">Час на проходження попередньої хвилі</param>
        /// <returns></returns>
        public int CreateWave(int _levelID, int _time = -1)
        {
            Level level = GetLevel(_levelID);

            if (_time != -1)
            {
                if (level.waves.Count != 0)
                    level.waves[level.waves.Count - 1].delay = _time;
            }

            level.waves.Add(new Waves(level.waves.Count));

            SaveData();

            return level.waves.Count - 1;
        }

        /// <summary>
        /// Добавляєм для рівня в хвилю нового юніта
        /// </summary>
        /// <param name="_levelID">ІД рівня</param>
        /// <param name="_waveID">ІД хвилі</param>
        /// <param name="_enemyId">ІД юніта</param>
        /// <param name="_count">К-сть юнітів</param>
        /// <param name="_time">Час коли виходить</param>
        /// <returns></returns>
        public int CreateEnemyOnWave(int _levelID, int _waveID, int _enemyId, int _count = 1, int _time = -1)
        {
            Waves wave = GetWave(_levelID, _waveID);

            wave.enemies.Add(new EnemyOfWave(_enemyId, _count, _time, 0));

            SaveData();

            return wave.enemies.Count - 1;
        }

        public static LevelDatabase GetDatabase()
        {
            #if UNITY_EDITOR
            return Utils.GetAsset<LevelDatabase>();
            #endif

            #if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<LevelDatabase>();
            #endif
        }

        public void SaveData()
        {
            if (PersistableSO.Instance)
                PersistableSO.Instance.SaveSO(this);
        }
    }

    [System.Serializable]
    public class Level
    {
        [Header("ID рівень")]
        public int id;

        [Header("Найменування рівня")]
        public string name;

        [Header("Іконка левела")]
        public Sprite ico;

        [Header("Хвилі юнітів")]
        public List<Waves> waves = new List<Waves>();

        public Level(int _id)
        {
            this.id = _id;
            this.name = "Рівень_" + (_id + 1);
        }
    }

    [System.Serializable]
    public class Waves
    {
        [Header("ID хвилі")]
        public int id;

        [Header("Затримка при старті хвилі(не помню що це)")]
        public float startDelay;

        [Header("Час на проходження")]
        public float delay;

        [Header("Юніти, що виходитимуть в даній хвилі")]
        public List<EnemyOfWave> enemies = new List<EnemyOfWave>();

        public Waves(int _id)
        {
            this.id = _id;
        }
    }

    [System.Serializable]
    public class EnemyOfWave
    {
        [Header("Тип юніта")]
        public int enemyID;

        [Header("Кількість")]
        public int countEnemy;

        [Header("Інтервал")]
        public float interval;

        [Header("Йморівність випадання монетки (%)")]
        public int changeCraft;

        public EnemyOfWave()
        {
            this.countEnemy = 1;
        }

        public EnemyOfWave(int _enemyID, int _countEnemy, float _interval, int _changeCraft)
        {
            this.enemyID = _enemyID;
            this.countEnemy = _countEnemy;
            this.interval = _interval;
            this.changeCraft = _changeCraft;
        }
    }
}
