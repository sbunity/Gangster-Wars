using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Databases.Levels
{
    [CreateAssetMenu(menuName = "Databases/Create LevelDatabase", fileName = "LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        [FormerlySerializedAs("levels")]
        [SerializeField, HideInInspector]
        private List<Level> _legacyLevels = new List<Level>();

        [FormerlySerializedAs("chapters")]
        [SerializeField]
        private List<ChapterDatabase> _chapters = new List<ChapterDatabase>();
        public List<ChapterDatabase> Chapters { get => _chapters; set => _chapters = value; }

        public List<Level> LegacyLevels => _legacyLevels;

        public List<Level> Levels
        {
            get
            {
                var levels = new List<Level>();

                if (_chapters != null)
                {
                    foreach (var chapter in _chapters)
                    {
                        if (chapter?.Levels == null)
                            continue;

                        levels.AddRange(chapter.Levels);
                    }
                }

                if (levels.Count == 0 && _legacyLevels != null)
                    levels.AddRange(_legacyLevels);

                return levels;
            }
        }

        public Level GetLevel(int id)
        {
            var chapter = GetChapterByLevelId(id);
            if (chapter != null)
                return chapter.GetLevel(id);

            if (_legacyLevels == null)
                return null;

            int index = _legacyLevels.FindIndex(x => x != null && x.Id == id);
            return index != -1 ? _legacyLevels[index] : null;
        }

        public ChapterDatabase GetChapter(int id)
        {
            if (_chapters == null)
                return null;

            int index = _chapters.FindIndex(x => x != null && x.Id == id);
            return index != -1 ? _chapters[index] : null;
        }

        public ChapterDatabase GetChapterByLevelId(int levelId)
        {
            if (_chapters == null)
                return null;

            foreach (var chapter in _chapters)
            {
                if (chapter != null && chapter.GetLevel(levelId) != null)
                    return chapter;
            }

            return null;
        }

        public int GetChapterIdByLevelId(int levelId)
        {
            var chapter = GetChapterByLevelId(levelId);
            return chapter != null ? chapter.Id : 0;
        }

        public Waves GetWave(Level _level, int _waveID)
        {
            if (_level?.Waves == null)
                return null;

            foreach (Waves _waves in _level.Waves)
            {
                if (_waves.Id == _waveID)
                    return _waves;
            }

            return null;
        }

        public Waves GetWave(int _levelID, int _waveID)
        {
            var chapter = GetChapterByLevelId(_levelID);
            return chapter != null ? chapter.GetWave(_levelID, _waveID) : GetWave(GetLevel(_levelID), _waveID);
        }

        public int CreateWave(int _levelID, int _time = -1)
        {
            var chapter = GetChapterByLevelId(_levelID);
            if (chapter != null)
                return chapter.CreateWave(_levelID, _time);

            Level level = GetLevel(_levelID);
            if (level == null)
                return -1;

            if (_time != -1 && level.Waves.Count != 0)
                level.Waves[level.Waves.Count - 1].Delay = _time;

            level.Waves.Add(new Waves(level.Waves.Count));
            SaveData();
            return level.Waves.Count - 1;
        }

        public int CreateEnemyOnWave(int _levelID, int _waveID, int _enemyId, int _count = 1, int _time = -1)
        {
            var chapter = GetChapterByLevelId(_levelID);
            if (chapter != null)
                return chapter.CreateEnemyOnWave(_levelID, _waveID, _enemyId, _count, _time);

            Waves wave = GetWave(_levelID, _waveID);
            if (wave == null)
                return -1;

            wave.Enemies.Add(new EnemyOfWave(_enemyId, _count, _time, 0));
            SaveData();
            return wave.Enemies.Count - 1;
        }

        public void ClearLegacyLevels()
        {
            _legacyLevels?.Clear();
            SaveData();
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
            this._name = "Час на проходження" + (_id + 1);
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
