using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create MainPlayerDatabase", fileName = "MainPlayerDatabase")]
    public class MainPlayerDatabase : ScriptableObject
    {
        [FormerlySerializedAs("personages")]
        [SerializeField, HideInInspector]
        private List<Personage> _personages = new List<Personage>();
        public List<Personage> Personages { get => _personages; set => _personages = value; }

        public Personage GetPersonage(int _id)
        {
            int index = _personages.FindIndex(x => x.Id == _id);
            return index != -1 ? _personages[index] : null;
        }

        public PUpgrade GetUpgrade(Personage _personage, int _id)
        {
            foreach (PUpgrade upgrade in _personage.Upgrades)
            {
                if (upgrade.Id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        public PUpgrade GetUpgrade(int _personageID, int _id)
        {
            Personage _personage = GetPersonage(_personageID);
            foreach (PUpgrade upgrade in _personage.Upgrades)
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
    public class Personage
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
        [FormerlySerializedAs("countUpgrades")]
        private int _countUpgrades;
        public int CountUpgrades { get => _countUpgrades; set => _countUpgrades = value; }

        [SerializeField]
        [FormerlySerializedAs("bulletID")]
        private int _bulletId;
        public int BulletId { get => _bulletId; set => _bulletId = value; }

        [SerializeField]
        [FormerlySerializedAs("upgrades")]
        private List<PUpgrade> _upgrades = new List<PUpgrade>();
        public List<PUpgrade> Upgrades { get => _upgrades; set => _upgrades = value; }

        [SerializeField]
        [FormerlySerializedAs("settings")]
        private PersonageSettings _settings = new PersonageSettings();
        public PersonageSettings Settings { get => _settings; set => _settings = value; }

        public Personage(int _id)
        {
            this._id = _id;
            this._name = "Personage_" + (_id + 1);
            this._bulletId = -1;
        }
    }

    [System.Serializable]
    public class PersonageSettings
    {
        [SerializeField]
        [FormerlySerializedAs("speedAtack")]
        private float _attackSpeed;
        public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }

        [SerializeField]
        [FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }
    }

    [System.Serializable]
    public class PUpgrade
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
        [FormerlySerializedAs("settings")]
        private PersonageSettings _settings;
        public PersonageSettings Settings { get => _settings; set => _settings = value; }

        public PUpgrade(int _id)
        {
            this._id = _id;
            this._name = "Upgrade_" + (_id + 1);
            this._price = 0;
        }
    }
}
