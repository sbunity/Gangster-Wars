using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create EnemyDatabase", fileName = "EnemyDatabase")]
    public class EnemyDatabase : ScriptableObject
    {
        [Header("Вороги")]
        [SerializeField, HideInInspector] public List<Enemy> enemies = new List<Enemy>();

        public Enemy GetEnemy(int id)
        {
            int index = enemies.FindIndex(x => x.id == id);

            return index != -1 ? enemies[index] : null;
        }

        public static EnemyDatabase GetDatabase()
        {
            #if UNITY_EDITOR
            return Utils.GetAsset<EnemyDatabase>();
            #endif

            #if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<EnemyDatabase>();
            #endif
        }

        void SaveData()
        {
            if (PersistableSO.Instance)
                PersistableSO.Instance.SaveSO(this);
        }
    }

    [System.Serializable]
    public class Enemy
    {
        #region Ідентифікування юніта
        [Header("ID Юніта")]
        public int id;

        [Header("Найменування юніта")]
        public string name;

        [Header("Іконка юніта, щоб в редакторі було видно хто це")]
        public Sprite ico;
        #endregion //Ідентифікування юніта

        [Header("Кількість життів")]
        public int health;

        [Header("Кошти за смерть")]
        public int gold;

        [Header("Швидкість руху")]
        public float speedMove;

        [Header("Швидкість атаки")]
        public float speedAtack;

        [Header("Радіус атаки")]
        public float radiusAtack;

        [Header("Урон")]
        public int damage;

        [Header("Якими пулями стріляє (id)")]
        public int bulletID;

        [Header("Йморівність випадання гранат (%)")]
        public int changeCraft;

        [Header("Яка граната випаде після смерті (id) //налаштовується у хвилях")]
        public int bonusID;

        public Enemy(int _id)
        {
            this.id = _id;
            this.name = "Enemy_" + (_id + 1);
            this.health = 0;
            this.gold = 0;
            this.speedMove = 0;
            this.speedAtack = 0;
            this.damage = 0;
            this.radiusAtack = 0;
            this.bulletID = -1;
            this.bonusID = -1;
        }

        public Enemy(Enemy enemy)
        {
            this.id = enemy.id;
            this.name = enemy.name;
            this.ico = enemy.ico;
            this.gold = enemy.gold;
            this.health = enemy.health;
            this.speedMove = enemy.speedMove;
            this.speedAtack = enemy.speedAtack;
            this.damage = enemy.damage;
            this.radiusAtack = enemy.radiusAtack;
            this.bulletID = enemy.bulletID;
            this.bonusID = -1;
        }
    }
}
