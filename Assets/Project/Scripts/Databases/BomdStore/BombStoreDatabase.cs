using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create BombStoreDatabase", fileName = "BombStoreDatabase")]
    public class BombStoreDatabase : ScriptableObject
    {
        [Header("Гранати")]
        [SerializeField, HideInInspector] public List<Grenade> grenades = new List<Grenade>();

        /// <summary>
        /// Повертає властивості гранати(по заданому id)
        /// </summary>
        /// <param name="_id">Id шуканої гарнати</param>
        /// <returns></returns>
        public Grenade GetGrenade(int _id)
        {
            int index = grenades.FindIndex(x => x.id == _id);

            return index != -1 ? grenades[index] : null;
        }

    }

    [System.Serializable]
    public class Grenade
    {
        #region Відображення
        [Header("ID")]
        public int id;

        [Header("Найменування")]
        public string name;

        [Header("Іконка")]
        public Sprite ico;
        #endregion //Відображення

        [Header("Вартість (за 1 штуку)")]
        public int price;

        [Header("Урон (без апгрейда)")]
        public int damage;

        [Header("Затримка до зриву")]
        public float delay;

        [Header("Час дії(для молотова)")]
        public float time;

        [Header("Радіус дії")]
        public float radius;

        [Header("Ефект зриву")]
        public CollisionsName collision;

        public Grenade(int _id)
        {
            this.id = _id;
            this.name = "Grenade_" + (_id + 1);
        }
    }
}
