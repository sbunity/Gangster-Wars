using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create MainPlayerDatabase", fileName = "MainPlayerDatabase")]
    public class MainPlayerDatabase : ScriptableObject
    {

        [Header("Персонажі")]
        [SerializeField, HideInInspector] public List<Personage> personages = new List<Personage>();

        /// <summary>
        /// Повертає властивості персонажа
        /// </summary>
        /// <param name="_id">Id шуканого персонажа</param>
        /// <returns></returns>
        public Personage GetPersonage(int _id)
        {
            int index = personages.FindIndex(x => x.id == _id);

            return index != -1 ? personages[index] : null;
        }

        public PUpgrade GetUpgrade(Personage _personage, int _id)
        {
            foreach (PUpgrade upgrade in _personage.upgrades)
            {
                if (upgrade.id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        public PUpgrade GetUpgrade(int _personageID, int _id)
        {
            Personage _personage = GetPersonage(_personageID);

            foreach (PUpgrade upgrade in _personage.upgrades)
            {
                if (upgrade.id == _id)
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
        #region Відображення
        [Header("ID")]
        public int id;

        [Header("Найменування")]
        public string name;

        [Header("Іконка")]
        public Sprite ico;
        #endregion //Відображення

        [Header("Вартість зброї")]
        public int price;

        [Header("Кількість апгрейдів")]
        public int countUpgrades;

        [Header("Якими пулями стріляє (id)")]
        public int bulletID;

        [Header("Апгрейди")]
        public List<PUpgrade> upgrades = new List<PUpgrade>();

        [Header("Параметри")]
        public PersonageSettings settings = new PersonageSettings();

        public Personage(int _id)
        {
            this.id = _id;
            this.name = "Personage_" + (_id + 1);
            this.bulletID = -1;
        }
    }

    [System.Serializable]
    public class PersonageSettings
    {
        [Header("Швидкість атаки")]
        public float speedAtack;

        [Header("Урон")]
        public int damage;
    }

    [System.Serializable]
    public class PUpgrade
    {
        [Header("ID апгрейда")]
        public int id;

        [Header("Найменування апгрейда")]
        public string name;

        [Header("Ціна апгрейда")]
        public int price;

        [Header("Властивості апгрейда")]
        public PersonageSettings settings;

        public PUpgrade(int _id)
        {
            this.id = _id;
            this.name = "Upgrade_" + (_id + 1);
            this.price = 0;
        }
    }
}
