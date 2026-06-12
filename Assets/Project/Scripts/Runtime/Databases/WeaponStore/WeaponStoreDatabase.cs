using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Databases.WeaponStore
{
    [CreateAssetMenu(menuName = "Databases/Create WeaponStoreDatabase", fileName = "WeaponStoreDatabase")]
    public class WeaponStoreDatabase : ScriptableObject
    {
        [FormerlySerializedAs("weapons")]
        [SerializeField, HideInInspector]
        private List<Weapon> _weapons = new List<Weapon>();
        public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }

        public Weapon GetWeapon(int _id)
        {
            int index = _weapons.FindIndex(x => x.Id == _id);
            return index != -1 ? _weapons[index] : null;
        }

        public WUpgrade GetUpgrade(Weapon _weapon, int _id)
        {
            foreach (WUpgrade upgrade in _weapon.Upgrades)
            {
                if (upgrade.Id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        public WUpgrade GetUpgrade(int _weaponID, int _id)
        {
            Weapon _weapon = GetWeapon(_weaponID);
            foreach (WUpgrade upgrade in _weapon.Upgrades)
            {
                if (upgrade.Id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class Weapon
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
        [FormerlySerializedAs("price")]

        private int _price;
        public int Price { get => _price; set => _price = value; }

        [SerializeField]
        [FormerlySerializedAs("magazine")]
        private int _magazine;
        public int Magazine { get => _magazine; set => _magazine = value; }

        [SerializeField]
        [FormerlySerializedAs("priceMagazine")]
        private int _priceMagazine;
        public int PriceMagazine { get => _priceMagazine; set => _priceMagazine = value; }

        [SerializeField]
        [FormerlySerializedAs("speedReload")]
        private float _speedReload;
        public float SpeedReload { get => _speedReload; set => _speedReload = value; }

        [SerializeField]
        [FormerlySerializedAs("countUpgrades")]
        private int _countUpgrades;
        public int CountUpgrades { get => _countUpgrades; set => _countUpgrades = value; }

        [SerializeField]
        [FormerlySerializedAs("bulletID")]
        private int _bulletId;
        public int BulletId { get => _bulletId; set => _bulletId = value; }

        [SerializeField]
        [FormerlySerializedAs("upgrades")]
        private List<WUpgrade> _upgrades = new List<WUpgrade>();
        public List<WUpgrade> Upgrades { get => _upgrades; set => _upgrades = value; }

        [SerializeField]
        [FormerlySerializedAs("settings")]
        private WeaponSettings _settings = new WeaponSettings();
        public WeaponSettings Settings { get => _settings; set => _settings = value; }

        public Weapon(int _id)
        {
            this._id = _id;
            this._name = "Weapon_" + _id;
        }

        public Weapon(Weapon _weapon)
        {
            this._id = _weapon.Id;
            this._name = _weapon.Name;
            this._icon = _weapon.Icon;
            this._magazine = _weapon.Magazine;
            this._priceMagazine = _weapon.PriceMagazine;
            this._settings = _weapon.Settings;
            this._upgrades = _weapon.Upgrades;
        }
    }

    [System.Serializable]
    public class WeaponSettings
    {
        [SerializeField]
        [FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }
    }

    [System.Serializable]
    public class WUpgrade
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
        [FormerlySerializedAs("price")]
        private int _price;
        public int Price { get => _price; set => _price = value; }

        [SerializeField]
        [FormerlySerializedAs("speedAtack")]
        private float _attackSpeed;
        public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }

        [SerializeField]
        [FormerlySerializedAs("settings")]
        private WeaponSettings _settings;
        public WeaponSettings Settings { get => _settings; set => _settings = value; }

        public WUpgrade(int _id)
        {
            this._id = _id;
            this._name = "Upgrade_" + (_id + 1);
            this._price = 0;
        }
    }
}
