using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    [CreateAssetMenu(menuName = "Databases/Create PlayerPrefsDatabase", fileName = "PlayerPrefsDatabase")]
    public class PlayerPrefsDatabase : ScriptableObject
    {
        [FormerlySerializedAs("PlayerPrefs")]
        [SerializeField, HideInInspector]
        private PlayerPrefs _playerPrefs = new PlayerPrefs();
        public PlayerPrefs PlayerPrefs { get => _playerPrefs; set => _playerPrefs = value; }

        public bool IsMusicEnabled()
        {
            return _playerPrefs.Music == mySwitch.On;
        }

        public bool IsSoundEnabled()
        {
            return _playerPrefs.Sound == mySwitch.On;
        }

        public bool OpportunityBuy(int price)
        {
            return _playerPrefs.Coin >= price;
        }

        private const float ThreeStarHealthThreshold = 0.75f;
        private const float TwoStarHealthThreshold = 0.50f;

        public void SetStars(LevelShortInfo levelShortInfo, float value)
        {
            if (value >= ThreeStarHealthThreshold)
                levelShortInfo.Stars = Mathf.Max(3, levelShortInfo.Stars);
            else if (value >= TwoStarHealthThreshold)
                levelShortInfo.Stars = Mathf.Max(2, levelShortInfo.Stars);
            else
                levelShortInfo.Stars = Mathf.Max(1, levelShortInfo.Stars);
        }
    }

    [System.Serializable]
    public class PlayerPrefs
    {
        [SerializeField]
        [FormerlySerializedAs("musik")]
        private mySwitch _music;
        public mySwitch Music { get => _music; set => _music = value; }

        [SerializeField]
        [FormerlySerializedAs("sound")]
        private mySwitch _sound;
        public mySwitch Sound { get => _sound; set => _sound = value; }

        [SerializeField]
        [FormerlySerializedAs("coin")]
        private int _coin;
        public int Coin { get => _coin; set => _coin = value; }

        [SerializeField]
        [FormerlySerializedAs("levelID")]
        private int _levelId;
        public int LevelId { get => _levelId; set => _levelId = value; }

        [SerializeField]
        [FormerlySerializedAs("chapterID")]
        private int _chapterId;
        public int ChapterId { get => _chapterId; set => _chapterId = value; }

        [SerializeField]
        [FormerlySerializedAs("selectedWeaponID")]
        private int _selectedWeaponId = 0;
        public int SelectedWeaponId { get => _selectedWeaponId; set => _selectedWeaponId = value; }

        [SerializeField]
        [FormerlySerializedAs("selectedGrenadeID")]
        private int _selectedGrenadeId = 0;
        public int SelectedGrenadeId { get => _selectedGrenadeId; set => _selectedGrenadeId = value; }

        [SerializeField]
        [FormerlySerializedAs("selectedDefenceID")]
        private int _selectedDefenceId = 0;
        public int SelectedDefenceId { get => _selectedDefenceId; set => _selectedDefenceId = value; }

        [SerializeField]
        [FormerlySerializedAs("weapons")]
        private List<WeaponShortInfo> _weapons = new List<WeaponShortInfo>();
        public List<WeaponShortInfo> Weapons { get => _weapons; set => _weapons = value; }

        [SerializeField]
        [FormerlySerializedAs("grenades")]
        private List<GrenadeShortInfo> _grenades = new List<GrenadeShortInfo>();
        public List<GrenadeShortInfo> Grenades { get => _grenades; set => _grenades = value; }

        [SerializeField]
        [FormerlySerializedAs("defences")]
        private List<DefenceShortInfo> _defences = new List<DefenceShortInfo>();
        public List<DefenceShortInfo> Defences { get => _defences; set => _defences = value; }

        [SerializeField]
        [FormerlySerializedAs("personages")]
        private List<PersonageShortInfo> _personages = new List<PersonageShortInfo>();
        public List<PersonageShortInfo> Personages { get => _personages; set => _personages = value; }

        [SerializeField]
        [FormerlySerializedAs("levels")]
        private List<LevelShortInfo> _levels = new List<LevelShortInfo>();
        public List<LevelShortInfo> Levels { get => _levels; set => _levels = value; }

        [SerializeField]
        [FormerlySerializedAs("chapters")]
        private List<ChapterShortInfo> _chapters = new List<ChapterShortInfo>();
        public List<ChapterShortInfo> Chapters { get => _chapters; set => _chapters = value; }

        [SerializeField] private List<int> _seenEnemyIds = new();
        public List<int> SeenEnemyIds { get => _seenEnemyIds; set => _seenEnemyIds = value; }

        public WeaponShortInfo GetWeaponShortInfo(int id)
        {
            if (_weapons == null)
                return null;

            foreach (var weapon in _weapons)
            {
                if (weapon != null && weapon.Id == id)
                    return weapon;
            }

            return null;
        }

        public GrenadeShortInfo GetGrenadeShortInfo(int id)
        {
            if (_grenades == null)
                return null;

            foreach (var grenade in _grenades)
            {
                if (grenade != null && grenade.Id == id)
                    return grenade;
            }

            return null;
        }

        public DefenceShortInfo GetDefenceShortInfo(int id)
        {
            if (_defences == null)
                return null;

            foreach (var defence in _defences)
            {
                if (defence != null && defence.Id == id)
                    return defence;
            }

            return null;
        }

        public PersonageShortInfo GetPersonageShortInfo(int id)
        {
            if (_personages == null)
                return null;

            foreach (var personage in _personages)
            {
                if (personage != null && personage.Id == id)
                    return personage;
            }

            return null;
        }

        public LevelShortInfo GetLevelShortInfo(int id)
        {
            if (_levels == null)
                return null;

            foreach (var info in _levels)
            {
                if (info != null && info.Id == id)
                    return info;
            }

            return null;
        }

        public ChapterShortInfo GetChapterShortInfo(int id)
        {
            if (_chapters == null)
                return null;

            foreach (var info in _chapters)
            {
                if (info != null && info.Id == id)
                    return info;
            }

            return null;
        }

        public bool HasSeenEnemy(int id)
            => _seenEnemyIds != null && _seenEnemyIds.Contains(id);
    }

    [System.Serializable]
    public class WeaponShortInfo
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
        [FormerlySerializedAs("isBuy")]
        private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField]
        [FormerlySerializedAs("upgradeID")]
        private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        [SerializeField]
        [FormerlySerializedAs("countPatrons")]
        private int _ammoCount;
        public int AmmoCount { get => _ammoCount; set => _ammoCount = value; }

        public WeaponShortInfo(Weapon weapon)
        {
            _id = weapon.Id;
            _name = weapon.Name;
            _isBuy = mySwitch.Off;
            _upgradeId = -1;
        }
    }

    [System.Serializable]
    public class GrenadeShortInfo
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("isBuy")]
        private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField]
        [FormerlySerializedAs("count")]
        private int _count;
        public int Count { get => _count; set => _count = value; }

        public GrenadeShortInfo(Grenade value)
        {
            _id = value.Id;
            _isBuy = mySwitch.Off;
            _count = 0;
        }
    }

    [System.Serializable]
    public class DefenceShortInfo
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("isBuy")]
        private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField]
        [FormerlySerializedAs("upgradeID")]
        private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        public DefenceShortInfo(Defense value)
        {
            _id = value.Id;
            _isBuy = mySwitch.Off;
            _upgradeId = -1;
        }
    }

    [System.Serializable]
    public class PersonageShortInfo
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("isBuy")]
        private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField]
        [FormerlySerializedAs("upgradeID")]
        private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        public PersonageShortInfo(Personage value)
        {
            _id = value.Id;
            _isBuy = mySwitch.Off;
        }
    }

    [System.Serializable]
    public class LevelShortInfo
    {
        [SerializeField]
        [FormerlySerializedAs("chapterId")]
        private int _chapterId;
        public int ChapterId { get => _chapterId; set => _chapterId = value; }

        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("isCompleted")]
        private mySwitch _isCompleted;
        public mySwitch IsCompleted { get => _isCompleted; set => _isCompleted = value; }

        [SerializeField]
        [FormerlySerializedAs("stars")]
        private int _stars;
        public int Stars { get => _stars; set => _stars = value; }

        public LevelShortInfo(Level value, int chapterId = 0)
        {
            _chapterId = chapterId;
            _id = value.Id;
            _isCompleted = mySwitch.Off;
            _stars = 0;
        }
    }

    [System.Serializable]
    public class ChapterShortInfo
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
        [FormerlySerializedAs("isUnlocked")]
        private mySwitch _isUnlocked;
        public mySwitch IsUnlocked { get => _isUnlocked; set => _isUnlocked = value; }

        [SerializeField]
        [FormerlySerializedAs("isCompleted")]
        private mySwitch _isCompleted;
        public mySwitch IsCompleted { get => _isCompleted; set => _isCompleted = value; }

        public ChapterShortInfo(ChapterDatabase value)
        {
            _id = value.Id;
            _name = value.Name;
            _isUnlocked = value.Id == 0 ? mySwitch.On : mySwitch.Off;
            _isCompleted = mySwitch.Off;
        }
    }
}
