using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Gameplay.Levels;
using UnityEngine;

namespace SBabchuk.Runtime.Databases.Levels
{
    [CreateAssetMenu(menuName = "Databases/Create LevelBackgroundDatabase", fileName = "LevelBackgroundDatabase")]
    public class LevelBackgroundDatabase : ScriptableObject
    {
        [SerializeField] private LevelBackground _defaultBackground;
        [SerializeField] private List<LevelBackgroundEntry> _levels = new();

        public LevelBackground DefaultBackground => _defaultBackground;
        public IReadOnlyList<LevelBackgroundEntry> Levels => _levels;

        public LevelBackground GetBackground(int levelId)
        {
            var entry = _levels.Find(x => x != null && x.LevelId == levelId);
            return entry != null && entry.Background != null ? entry.Background : _defaultBackground;
        }

        public void SetBackground(int levelId, LevelBackground background)
        {
            var entry = _levels.Find(x => x != null && x.LevelId == levelId);
            if (entry == null)
                _levels.Add(new LevelBackgroundEntry(levelId, background));
            else
                entry.Background = background;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    [Serializable]
    public class LevelBackgroundEntry
    {
        [SerializeField] private int _levelId;
        [SerializeField] private LevelBackground _background;

        public int LevelId => _levelId;
        public LevelBackground Background { get => _background; set => _background = value; }

        public LevelBackgroundEntry(int levelId, LevelBackground background)
        {
            _levelId = levelId;
            _background = background;
        }
    }
}
