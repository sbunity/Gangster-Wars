using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create BulletDatabase", fileName = "BulletDatabase")]
    public class BulletDatabase : ScriptableObject
    {
        [Header("Пулі")]
        [SerializeField, HideInInspector] public List<Bullet> bullets = new List<Bullet>();

        public Bullet GetBullet(int id)
        {
            int index = bullets.FindIndex(x => x.id == id);

            return index != -1 ? bullets[index] : null;
        }

        /// <summary>
        /// Повертає ссилку на базу даних
        /// </summary>
        /// <returns></returns>
        public static BulletDatabase GetDatabase()
        {
            #if UNITY_EDITOR
            return Utils.GetAsset<BulletDatabase>();
            #endif

            #if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<BulletDatabase>();
            #endif
        }

        void SaveData()
        {
            if (PersistableSO.Instance)
                PersistableSO.Instance.SaveSO(this);
        }
    }

    [System.Serializable]
    public class Bullet
    {
        [Header("ID Пулі")]
        public int id;

        [Header("Найменування пулі")]
        public string name;

        [Header("Іконка пулі")]
        public Sprite ico;

        [Header("Швидкість руху")]
        public float speedMove;

        [Header("Урон")]
        public int damage;

        public Bullet(int _id)
        {
            this.id = _id;
            this.name = "Bullet_" + (_id +1);
        }

        //public Bullet (Bullet _bullet){
        //	this.id = _bullet.id;
        //	this.name = _bullet.name;
        //	this.ico = _bullet.ico;
        //	this.speedMove = _bullet.speedMove;
        //	this.damage = _bullet.damage;
        //}
    }
}
