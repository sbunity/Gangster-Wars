using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create BulletDatabase", fileName = "BulletDatabase")]
    public class BulletDatabase : ScriptableObject
    {
        [FormerlySerializedAs("bullets")]
        [SerializeField, HideInInspector]
        private List<Bullet> _bullets = new List<Bullet>();
        public List<Bullet> Bullets { get => _bullets; set => _bullets = value; }

        public Bullet GetBullet(int id)
        {
            int index = _bullets.FindIndex(x => x.Id == id);
            return index != -1 ? _bullets[index] : null;
        }
    }

    [System.Serializable]
    public class Bullet
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
        [FormerlySerializedAs("speedMove")]
        private float _speedMove;
        public float SpeedMove { get => _speedMove; set => _speedMove = value; }

        [SerializeField]
        [FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }

        public Bullet(int _id)
        {
            this._id = _id;
            this._name = "Bullet_" + (_id + 1);
        }
    }
}
