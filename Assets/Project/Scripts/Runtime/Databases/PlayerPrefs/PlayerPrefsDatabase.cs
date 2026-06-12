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

        public void SetStars(LevelShortInfo levelShortInfo, float value)
        {
            if (value >= 0.75f)
                levelShortInfo.Stars = Mathf.Max(3, levelShortInfo.Stars);
            else if (value >= 0.50f)
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

        public WeaponShortInfo GetWeaponShortInfo(int id)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.Id == id)
                    return weapon;
            }

            return null;
        }

        public GrenadeShortInfo GetGrenadeShortInfo(int id)
        {
            foreach (var grenade in _grenades)
            {
                if (grenade.Id == id)
                    return grenade;
            }

            return null;
        }

        public DefenceShortInfo GetDefenceShortInfo(int id)
        {
            foreach (var defence in _defences)
            {
                if (defence.Id == id)
                    return defence;
            }

            return null;
        }

        public PersonageShortInfo GetPersonageShortInfo(int id)
        {
            foreach (var personage in _personages)
            {
                if (personage.Id == id)
                    return personage;
            }

            return null;
        }

        public LevelShortInfo GetLevelShortInfo(int id)
        {
            foreach (var info in _levels)
            {
                if (info.Id == id)
                    return info;
            }

            return null;
        }
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

        public LevelShortInfo(Level value)
        {
            _id = value.Id;
            _isCompleted = mySwitch.Off;
            _stars = 0;
        }
    }
}
