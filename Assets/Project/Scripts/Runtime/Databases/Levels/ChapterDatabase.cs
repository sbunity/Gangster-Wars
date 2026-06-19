using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Databases.Levels
{
    [CreateAssetMenu(menuName = "Databases/Create ChapterDatabase", fileName = "ChapterDatabase")]
    public class ChapterDatabase : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("name")]
        private string _name = "Chapter";
        public string Name { get => _name; set => _name = value; }

        [FormerlySerializedAs("levels")]
        [SerializeField, HideInInspector]
        private List<Level> _levels = new List<Level>();
        public List<Level> Levels { get => _levels; set => _levels = value; }

        public Level GetLevel(int id)
        {
            if (_levels == null)
                return null;

            int index = _levels.FindIndex(x => x != null && x.Id == id);
            return index != -1 ? _levels[index] : null;
        }

        public Waves GetWave(Level level, int waveId)
        {
            if (level?.Waves == null)
                return null;

            foreach (Waves waves in level.Waves)
            {
                if (waves.Id == waveId)
                    return waves;
            }

            return null;
        }

        public Waves GetWave(int levelId, int waveId)
        {
            Level level = GetLevel(levelId);
            return GetWave(level, waveId);
        }

        public int CreateWave(int levelId, int time = -1)
        {
            Level level = GetLevel(levelId);
            if (level == null)
                return -1;

            if (time != -1 && level.Waves.Count != 0)
                level.Waves[level.Waves.Count - 1].Delay = time;

            level.Waves.Add(new Waves(level.Waves.Count));
            SaveData();
            return level.Waves.Count - 1;
        }

        public int CreateEnemyOnWave(int levelId, int waveId, int enemyId, int count = 1, int time = -1)
        {
            Waves wave = GetWave(levelId, waveId);
            if (wave == null)
                return -1;

            wave.Enemies.Add(new EnemyOfWave(enemyId, count, time, 0));
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
}
