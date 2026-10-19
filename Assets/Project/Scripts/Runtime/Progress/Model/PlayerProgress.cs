using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class PlayerProgress
    {
        [SerializeField] private mySwitch _music;
        public mySwitch Music { get => _music; set => _music = value; }

        [SerializeField] private mySwitch _sound;
        public mySwitch Sound { get => _sound; set => _sound = value; }

        [SerializeField] private mySwitch _vibration;
        public mySwitch Vibration { get => _vibration; set => _vibration = value; }

        [SerializeField] private int _coin;
        public int Coin { get => _coin; set => _coin = value; }

        [SerializeField] private int _levelId;
        public int LevelId { get => _levelId; set => _levelId = value; }

        [SerializeField] private int _chapterId;
        public int ChapterId { get => _chapterId; set => _chapterId = value; }

        [SerializeField] private int _selectedWeaponId;
        public int SelectedWeaponId { get => _selectedWeaponId; set => _selectedWeaponId = value; }

        [SerializeField] private int _selectedGrenadeId;
        public int SelectedGrenadeId { get => _selectedGrenadeId; set => _selectedGrenadeId = value; }

        [SerializeField] private int _selectedDefenceId;
        public int SelectedDefenceId { get => _selectedDefenceId; set => _selectedDefenceId = value; }

        [SerializeField] private List<WeaponShortInfo> _weapons = new();
        public List<WeaponShortInfo> Weapons { get => _weapons; set => _weapons = value; }

        [SerializeField] private List<GrenadeShortInfo> _grenades = new();
        public List<GrenadeShortInfo> Grenades { get => _grenades; set => _grenades = value; }

        [SerializeField] private List<DefenceShortInfo> _defences = new();
        public List<DefenceShortInfo> Defences { get => _defences; set => _defences = value; }

        [SerializeField] private List<PersonageShortInfo> _personages = new();
        public List<PersonageShortInfo> Personages { get => _personages; set => _personages = value; }

        [SerializeField] private List<LevelShortInfo> _levels = new();
        public List<LevelShortInfo> Levels { get => _levels; set => _levels = value; }

        [SerializeField] private List<ChapterShortInfo> _chapters = new();
        public List<ChapterShortInfo> Chapters { get => _chapters; set => _chapters = value; }

        [SerializeField] private List<int> _seenEnemyIds = new();
        public List<int> SeenEnemyIds { get => _seenEnemyIds; set => _seenEnemyIds = value; }

        public WeaponShortInfo GetWeaponShortInfo(int id)
            => Find(_weapons, entry => entry.Id == id);

        public GrenadeShortInfo GetGrenadeShortInfo(int id)
            => Find(_grenades, entry => entry.Id == id);

        public DefenceShortInfo GetDefenceShortInfo(int id)
            => Find(_defences, entry => entry.Id == id);

        public PersonageShortInfo GetPersonageShortInfo(int id)
            => Find(_personages, entry => entry.Id == id);

        public LevelShortInfo GetLevelShortInfo(int id)
            => Find(_levels, entry => entry.Id == id);

        public ChapterShortInfo GetChapterShortInfo(int id)
            => Find(_chapters, entry => entry.Id == id);

        public bool HasSeenEnemy(int id)
            => _seenEnemyIds != null && _seenEnemyIds.Contains(id);

        public bool RefreshChapterCompletion(int chapterId, IReadOnlyCollection<int> levelIds)
        {
            var chapter = GetChapterShortInfo(chapterId);
            if (chapter == null || levelIds == null || levelIds.Count == 0)
                return false;

            var isCompleted = true;
            foreach (var levelId in levelIds)
            {
                if (GetLevelShortInfo(levelId)?.IsCompleted != mySwitch.On)
                {
                    isCompleted = false;
                    break;
                }
            }

            var state = isCompleted ? mySwitch.On : mySwitch.Off;
            if (chapter.IsCompleted == state)
                return false;

            chapter.IsCompleted = state;
            return true;
        }

        private static T Find<T>(List<T> entries, Predicate<T> match) where T : class
        {
            if (entries == null)
                return null;

            foreach (var entry in entries)
            {
                if (entry != null && match(entry))
                    return entry;
            }

            return null;
        }
    }
}
