using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Databases.DefenseStore
{
    [CreateAssetMenu(menuName = "Databases/Create DefenseStoreDatabase", fileName = "DefenseStoreDatabase")]
    public class DefenseStoreDatabase : ScriptableObject
    {
        [FormerlySerializedAs("defenses")]
        [SerializeField, HideInInspector]
        private List<Defense> _defenses = new List<Defense>();
        public List<Defense> Defenses { get => _defenses; set => _defenses = value; }

        public Defense GetDefense(int _id)
        {
            int index = _defenses.FindIndex(x => x.Id == _id);
            return index != -1 ? _defenses[index] : null;
        }

        public DUpgrade GetUpgrade(Defense _defense, int _id)
        {
            foreach (DUpgrade upgrade in _defense.Upgrades)
            {
                if (upgrade.Id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        public DUpgrade GetUpgrade(int _defenseID, int _id)
        {
            Defense _defense = GetDefense(_defenseID);
            foreach (DUpgrade upgrade in _defense.Upgrades)
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
    public class Defense
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
        [FormerlySerializedAs("upgrades")]
        private List<DUpgrade> _upgrades = new List<DUpgrade>();
        public List<DUpgrade> Upgrades { get => _upgrades; set => _upgrades = value; }

        [SerializeField]
        [FormerlySerializedAs("settings")]
        private DefenceSettings _settings = new DefenceSettings();
        public DefenceSettings Settings { get => _settings; set => _settings = value; }

        [SerializeField]
        [FormerlySerializedAs("countIcons")]
        private int _countIcons;
        public int CountIcons { get => _countIcons; set => _countIcons = value; }

        [SerializeField]
        [FormerlySerializedAs("icons")]
        private List<Ico> _icons = new List<Ico>();
        public List<Ico> Icons { get => _icons; set => _icons = value; }

        public Defense(int _id)
        {
            this._id = _id;
            this._name = "Defense_" + (_id + 1);
        }
    }

    [System.Serializable]
    public class DefenceSettings
    {
        [SerializeField]
        [FormerlySerializedAs("health")]
        private int _health;
        public int Health { get => _health; set => _health = value; }
    }

    [System.Serializable]
    public class DUpgrade
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
        private DefenceSettings _settings;
        public DefenceSettings Settings { get => _settings; set => _settings = value; }

        public DUpgrade(int _id)
        {
            this._id = _id;
            this._name = "Upgrade_" + (_id + 1);
            this._price = 0;
        }
    }

    [System.Serializable]
    public class Ico
    {
        [SerializeField]
        [FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        [FormerlySerializedAs("ico")]
        private Sprite _icon;
        public Sprite Icon { get => _icon; set => _icon = value; }

        public Ico(int _id)
        {
            this._id = _id;
        }
    }
}
