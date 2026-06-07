using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create WeaponStoreDatabase", fileName = "WeaponStoreDatabase")]
    public class WeaponStoreDatabase : ScriptableObject
    {
        [Header("Зброя")]
        [SerializeField, HideInInspector] public List<Weapon> weapons = new List<Weapon>();

        public Weapon GetWeapon(int _id)
        {
            int index = weapons.FindIndex(x => x.id == _id);

            return index != -1 ? weapons[index] : null;
        }

        public WUpgrade GetUpgrade(Weapon _weapon, int _id)
        {
            foreach (WUpgrade upgrade in _weapon.upgrades)
            {
                if (upgrade.id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        public WUpgrade GetUpgrade(int _weaponID, int _id)
        {
            Weapon _weapon = GetWeapon(_weaponID);

            foreach (WUpgrade upgrade in _weapon.upgrades)
            {
                if (upgrade.id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }


        public static WeaponStoreDatabase GetDatabase()
        {
        #if UNITY_EDITOR
            return Utils.GetAsset<WeaponStoreDatabase>();
        #endif

        #if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<WeaponStoreDatabase>();
        #endif
        }

        void Save()
        {
            if (PersistableSO.Instance)
                PersistableSO.Instance.SaveSO(this);
        }
    }

    [System.Serializable]
    public class Weapon
    {
        #region Відображення
        [Header("ID зброї")]
        public int id;

        [Header("Найменування зброї")]
        public string name;

        [Header("Іконка сброї")]
        public Sprite ico;
        #endregion //Відображення

        [Header("Вартість зброї")]
        public int price;

        [Header("Максимальна кількість патронів в магазині")]
        public int magazine;

        [Header("Вартість магазина патронів")]
        public int priceMagazine;

        [Header("Швидкість перезарядки 1 патрона")]
        public float speedReload;

        [Header("Кількість апгрейдів")]
        public int countUpgrades;

        [Header("Якими пулями стріляє (id)")]
        public int bulletID;

        [Header("Апгрейди")]
        public List<WUpgrade> upgrades = new List<WUpgrade>();

        [Header("Параметри")]
        public WeaponSettings settings = new WeaponSettings();

        public Weapon(int _id)
        {
            this.id = _id;
            this.name = "Weapon_" + _id;
        }

        public Weapon(Weapon _weapon)
        {
            this.id = _weapon.id;
            this.name = _weapon.name;
            this.ico = _weapon.ico;
            this.magazine = _weapon.magazine;
            this.priceMagazine = _weapon.priceMagazine;
            this.settings = _weapon.settings;
            this.upgrades = _weapon.upgrades;
        }
    }

    [System.Serializable]
    public class WeaponSettings
    {
        [Header("Урон")]
        public int damage;
    }

    [System.Serializable]
    public class WUpgrade
    {
        [Header("ID апгрейда")]
        public int id;

        [Header("Найменування апгрейда")]
        public string name;

        [Header("Ціна апгрейда")]
        public int price;

        [Header("Швидкість атаки")]
        public float speedAtack;

        [Header("Властивості апгрейда")]
        public WeaponSettings settings;

        public WUpgrade(int _id)
        {
            this.id = _id;
            this.name = "Upgrade_" + (_id + 1);
            this.price = 0;
        }
    }
}
