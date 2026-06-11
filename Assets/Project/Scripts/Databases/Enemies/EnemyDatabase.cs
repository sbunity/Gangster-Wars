using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create EnemyDatabase", fileName = "EnemyDatabase")]
    public class EnemyDatabase : ScriptableObject
    {
        [FormerlySerializedAs("enemies")]
        [SerializeField, HideInInspector]
        private List<Enemy> _enemies = new List<Enemy>();
        public List<Enemy> Enemies { get => _enemies; set => _enemies = value; }

        public Enemy GetEnemy(int id)
        {
            int index = _enemies.FindIndex(x => x.Id == id);
            return index != -1 ? _enemies[index] : null;
        }
    }

    [System.Serializable]
    public class Enemy
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
        [FormerlySerializedAs("health")]

        private int _health;
        public int Health { get => _health; set => _health = value; }

        [SerializeField]
        [FormerlySerializedAs("gold")]
        private int _gold;
        public int Gold { get => _gold; set => _gold = value; }

        [SerializeField]
        [FormerlySerializedAs("speedMove")]
        private float _speedMove;
        public float SpeedMove { get => _speedMove; set => _speedMove = value; }

        [SerializeField]
        [FormerlySerializedAs("speedAtack")]
        private float _attackSpeed;
        public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }

        [SerializeField]
        [FormerlySerializedAs("radiusAtack")]
        private float _attackRadius;
        public float AttackRadius { get => _attackRadius; set => _attackRadius = value; }

        [SerializeField]
        [FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }

        [SerializeField]
        [FormerlySerializedAs("bulletID")]
        private int _bulletId;
        public int BulletId { get => _bulletId; set => _bulletId = value; }

        [SerializeField]
        [FormerlySerializedAs("changeCraft")]
        private int _dropChance;
        public int DropChance { get => _dropChance; set => _dropChance = value; }

        [SerializeField]
        [FormerlySerializedAs("bonusID")]
        private int _bonusId;
        public int BonusId { get => _bonusId; set => _bonusId = value; }

        public Enemy(int _id)
        {
            this._id = _id;
            this._name = "Enemy_" + (_id + 1);
            this._health = 0;
            this._gold = 0;
            this._speedMove = 0;
            this._attackSpeed = 0;
            this._damage = 0;
            this._attackRadius = 0;
            this._bulletId = -1;
            this._bonusId = -1;
        }

        public Enemy(Enemy enemy)
        {
            this._id = enemy.Id;
            this._name = enemy.Name;
            this._icon = enemy.Icon;
            this._gold = enemy.Gold;
            this._health = enemy.Health;
            this._speedMove = enemy.SpeedMove;
            this._attackSpeed = enemy.AttackSpeed;
            this._damage = enemy.Damage;
            this._attackRadius = enemy.AttackRadius;
            this._bulletId = enemy.BulletId;
            this._bonusId = -1;
        }
    }
}
