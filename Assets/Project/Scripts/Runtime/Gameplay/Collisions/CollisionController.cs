using UnityEngine;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Collisions
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

        private SortingEnemy _sorting;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected virtual void Awake()
        {
            EnsureParent();
            _sorting = GetComponent<SortingEnemy>();
        }

        public void SortInFrontOf(SortingEnemy target)
        {
            if (target != null)
                _sorting?.FollowInFrontOf(target);
        }

        public virtual void Start()
        {
            EnsureParent();
        }

        public virtual void Init(Vector3 position, int damage = 0, float radius = 00, float time = 0)
        {
            gameObject.SetActive(true);
            EnsureParent();
            _sorting?.ClearFollow();
            transform.position = position;
            _damage = damage;
            _radius = radius;
            _time = time;
        }

        public virtual void Pop()
        {
            FireDamage();
            Release();
        }

        protected void FireDamage()
        {
            _signalBus?.Fire(new GrenadeDamageSignal(transform.position, _damage, _radius));
        }

        protected void Release()
        {
            transform.SetParent(_parent);
            gameObject.SetActive(false);
            transform.tag = "Collision";
        }

        private void EnsureParent()
        {
            if (_parent == null)
                _parent = transform.parent;
        }
    }
}
