using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;
using SBabchuk.Runtime.Architecture;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_parent")]
        private Transform _parent;

        private SignalBus _signalBus;
        [SerializeField, FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }

        [SerializeField, FormerlySerializedAs("radius")]
        private float _radius;
        public float Radius { get => _radius; set => _radius = value; }

        [SerializeField, FormerlySerializedAs("time")]
        private float _time;
        public float Time { get => _time; set => _time = value; }

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public virtual void Start()
        {
            _parent = transform.parent;
        }

        public virtual void Init(Vector3 position, int damage = 0, float radius = 00, float time = 0)
        {
            this.gameObject.SetActive(true);
            transform.position = position;
            _damage = damage;
            _radius = radius;
            _time = time;
        }

        public void Pop()
        {
            _signalBus.Fire(new GrenadeDamageSignal(transform.position, _damage, _radius));
            transform.SetParent(_parent);
            this.gameObject.SetActive(false);
            transform.tag = "Collision";
        }
    }
}
