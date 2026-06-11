using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create BombStoreDatabase", fileName = "BombStoreDatabase")]
    public class BombStoreDatabase : ScriptableObject
    {
        [FormerlySerializedAs("grenades")]
        [SerializeField, HideInInspector]
        private List<Grenade> _grenades = new List<Grenade>();
        public List<Grenade> Grenades { get => _grenades; set => _grenades = value; }

        public Grenade GetGrenade(int _id)
        {
            int index = _grenades.FindIndex(x => x.Id == _id);
            return index != -1 ? _grenades[index] : null;
        }
    }

    [System.Serializable]
    public class Grenade
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
        [FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }

        [SerializeField]
        [FormerlySerializedAs("delay")]
        private float _delay;
        public float Delay { get => _delay; set => _delay = value; }

        [SerializeField]
        [FormerlySerializedAs("time")]
        private float _time;
        public float Time { get => _time; set => _time = value; }

        [SerializeField]
        [FormerlySerializedAs("radius")]
        private float _radius;
        public float Radius { get => _radius; set => _radius = value; }

        [SerializeField]
        [FormerlySerializedAs("collision")]
        private CollisionsName _collision;
        public CollisionsName Collision { get => _collision; set => _collision = value; }

        public Grenade(int _id)
        {
            this._id = _id;
            this._name = "Grenade_" + (_id + 1);
        }
    }
}
