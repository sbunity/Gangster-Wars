using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create LevelDatabase", fileName = "LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        [FormerlySerializedAs("levels")]
        [SerializeField, HideInInspector]
        private List<Level> _levels = new List<Level>();
        public List<Level> Levels { get => _levels; set => _levels = value; }

        public Level GetLevel(int id)
        {
            int index = _levels.FindIndex(x => x.Id == id);
            return index != -1 ? _levels[index] : null;
        }

        public Waves GetWave(Level _level, int _waveID)
        {
            foreach (Waves _waves in _level.Waves)
            {
                if (_waves.Id == _waveID)
                {
                    return _waves;
                }
            }

            return null;
        }

        public Waves GetWave(int _levelID, int _waveID)
        {
            Level _level = GetLevel(_levelID);
            foreach (Waves _waves in _level.Waves)
            {
                if (_waves.Id == _waveID)
                {
                    return _waves;
                }
            }

            return null;
        }

        public int CreateWave(int _levelID, int _time = -1)
        {
            Level level = GetLevel(_levelID);
            if (_time != -1)
            {
                if (level.Waves.Count != 0)
                    level.Waves[level.Waves.Count - 1].Delay = _time;
            }

            level.Waves.Add(new Waves(level.Waves.Count));
            SaveData();
            return level.Waves.Count - 1;
        }

        public int CreateEnemyOnWave(int _levelID, int _waveID, int _enemyId, int _count = 1, int _time = -1)
        {
            Waves wave = GetWave(_levelID, _waveID);
            wave.Enemies.Add(new EnemyOfWave(_enemyId, _count, _time, 0));
            SaveData();
            return wave.Enemies.Count - 1;
        }

        public void SaveData()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    [System.Serializable]
    public class Level
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("name")]
        private string _name;
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        [FormerlySerializedAs("ico")]
        private Sprite _icon;
        public Sprite Icon { get => _icon; set => _icon = value; }

        [SerializeField]
        [FormerlySerializedAs("waves")]
        private List<Waves> _waves = new List<Waves>();
        public List<Waves> Waves { get => _waves; set => _waves = value; }

        public Level(int _id)
        {
            this._id = _id;
            this._name = "Р В РЎвЂ“Р Р†Р ВµР Р…РЎРЉ_" + (_id + 1);
        }
    }

    [System.Serializable]
    public class Waves
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("startDelay")]
        private float _startDelay;
        public float StartDelay { get => _startDelay; set => _startDelay = value; }

        [SerializeField]
        [FormerlySerializedAs("delay")]
        private float _delay;
        public float Delay { get => _delay; set => _delay = value; }

        [SerializeField]
        [FormerlySerializedAs("enemies")]
        private List<EnemyOfWave> _enemies = new List<EnemyOfWave>();
        public List<EnemyOfWave> Enemies { get => _enemies; set => _enemies = value; }

        public Waves(int _id)
        {
            this._id = _id;
        }
    }

    [System.Serializable]
    public class EnemyOfWave
    {
        [SerializeField]
        [FormerlySerializedAs("enemyID")]
        private int _enemyId;
        public int EnemyId { get => _enemyId; set => _enemyId = value; }

        [SerializeField]
        [FormerlySerializedAs("countEnemy")]
        private int _countEnemy;
        public int CountEnemy { get => _countEnemy; set => _countEnemy = value; }

        [SerializeField]
        [FormerlySerializedAs("interval")]
        private float _interval;
        public float Interval { get => _interval; set => _interval = value; }

        [SerializeField]
        [FormerlySerializedAs("changeCraft")]
        private int _dropChance;
        public int DropChance { get => _dropChance; set => _dropChance = value; }

        public EnemyOfWave()
        {
            this._countEnemy = 1;
        }

        public EnemyOfWave(int _enemyID, int _countEnemy, float _interval, int _changeCraft)
        {
            this._enemyId = _enemyID;
            this._countEnemy = _countEnemy;
            this._interval = _interval;
            this._dropChance = _changeCraft;
        }
    }
}
