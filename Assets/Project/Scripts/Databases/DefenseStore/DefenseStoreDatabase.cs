using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create DefenseStoreDatabase", fileName = "DefenseStoreDatabase")]
    public class DefenseStoreDatabase : ScriptableObject
    {
        [Header("Перепона")]
        [SerializeField, HideInInspector] public List<Defense> defenses = new List<Defense>();

        /// <summary>
        /// Повертає властивості перепони(по заданому id)
        /// </summary>
        /// <param name="_id">Id шуканої перпони</param>
        /// <returns></returns>
        public Defense GetDefense(int _id)
        {
            int index = defenses.FindIndex(x => x.id == _id);

            return index != -1 ? defenses[index] : null;
        }

        /// <summary>
        /// Повертає апгрейд (по id) для даної перепони (по Defense)
        /// </summary>
        /// <param name="_grenade">Перепона</param>
        /// <param name="_id">Id шуканого апгрейда</param>
        /// <returns></returns>
        public DUpgrade GetUpgrade(Defense _defense, int _id)
        {
            foreach (DUpgrade upgrade in _defense.upgrades)
            {
                if (upgrade.id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертає апгрейд (по id) для даної перепони (по _defenseID)
        /// </summary>
        /// <param name="_defenseID">Id перепони</param>
        /// <param name="_id">Id шуканого апгрейда</param>
        /// <returns></returns>
        public DUpgrade GetUpgrade(int _defenseID, int _id)
        {
            Defense _defense = GetDefense(_defenseID);

            foreach (DUpgrade upgrade in _defense.upgrades)
            {
                if (upgrade.id == _id)
                {
                    return upgrade;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертає ссилку на базу даних
        /// </summary>
        /// <returns></returns>
        public static DefenseStoreDatabase GetDatabase()
        {
#if UNITY_EDITOR
            return Utils.GetAsset<DefenseStoreDatabase>();
#endif

#if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<DefenseStoreDatabase>();
#endif
        }

        /// <summary>
        /// Збереження
        /// </summary>
        void Save()
        {
            if (PersistableSO.Instance)
                PersistableSO.Instance.SaveSO(this);
        }
    }

    [System.Serializable]
    public class Defense
    {
        #region Відображення
        [Header("ID")]
        public int id;

        [Header("Найменування")]
        public string name;

        [Header("Іконка сброї")]
        public Sprite ico;
        #endregion //Відображення

        [Header("Вартість")]
        public int price;

        [Header("Кількість апгрейдів")]
        public int countUpgrades;

        [Header("Апгрейди")]
        public List<DUpgrade> upgrades = new List<DUpgrade>();

        [Header("Параметри")]
        public DefenceSettings settings = new DefenceSettings();

        [Header("Кількість іконок")]
        public int countIcons;

        [Header("Картинки відносно стану")]
        public List<Ico> icons = new List<Ico>();

        public Defense(int _id)
        {
            this.id = _id;
            this.name = "Defense_" + (_id + 1);
        }
    }

    [System.Serializable]
    public class DefenceSettings
    {
        [Header("К-сть життів (без апгрейда)")]
        public int health;
    }

    [System.Serializable]
    public class DUpgrade
    {
        [Header("ID апгрейда")]
        public int id;

        [Header("Найменування апгрейда")]
        public string name;

        [Header("Ціна апгрейда")]
        public int price;

        [Header("Властивості апгрейда")]
        public DefenceSettings settings;

        public DUpgrade(int _id)
        {
            this.id = _id;
            this.name = "Upgrade_" + (_id + 1);
            this.price = 0;
        }
    }

    [System.Serializable]
    public class Ico
    {
        [Header("ID іконки")]
        public int id;

        [Header("Іконка")]
        public Sprite ico;

        public Ico(int _id)
        {
            this.id = _id;
        }
    }
}
