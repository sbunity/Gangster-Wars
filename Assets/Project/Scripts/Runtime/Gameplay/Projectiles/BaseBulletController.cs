using UnityEngine;
using SBabchuk.Runtime.Gameplay.Projectiles;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BaseBulletController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("rigidbody2d")]
        private Rigidbody2D _rigidbody2D;
        public Rigidbody2D Rigidbody2D { get => _rigidbody2D; set => _rigidbody2D = value; }

        [SerializeField, FormerlySerializedAs("_parent")]
        private Transform _parent;

        [SerializeField, FormerlySerializedAs("properties")]
        private Bullet _properties;
        public Bullet Properties { get => _properties; set => _properties = value; }

        [SerializeField, FormerlySerializedAs("damage")]
        private int _damage;
        public int Damage { get => _damage; set => _damage = value; }

        [SerializeField, FormerlySerializedAs("collisionCollider")]
        private Collider2D _collisionCollider;

        protected ILevelSpawnService LevelSpawnService => _levelSpawnService;

        private SortingEnemy _sorting;
        private IAssetProvider _assetProvider;
        private IGameFactory _gameFactory;
        private ILevelSpawnService _levelSpawnService;
        private IAimService _aimService;
        private ProjectileView _view;
        private ProjectileMovement _movement;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IGameFactory gameFactory, ILevelSpawnService levelSpawnService, IAimService aimService)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _levelSpawnService = levelSpawnService;
            _aimService = aimService;
        }

        public virtual void Awake()
        {
            _view = GetOrAdd<ProjectileView>();
            _movement = GetOrAdd<ProjectileMovement>();
            _view.Initialize();
            _rigidbody2D = _view.Rigidbody;
            _sorting = _view.Sorting;
        }

        public virtual void Start()
        {
            _parent = transform.parent;
        }

        public virtual void Init(int id = 0, int damage = 0, Vector3 position = default(Vector3), Vector3 target = default(Vector3), float offset = 0)
        {
            this.gameObject.SetActive(true);
            if (_sorting)
                _sorting.AnchorOffset = -offset;

            transform.position = position;
            _damage = damage;
            var bulletDatabase = _assetProvider.BulletDatabase;
            _properties = bulletDatabase.GetBullet(id);
            StartMove((target != default(Vector3)) ? (Vector2)target : _aimService.CurrentAimPosition);
            
            if (_collisionCollider == null)
            {
                _collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
                _view.Initialize();
            }

            _collisionCollider.isTrigger = false;
        }

        public virtual void StartMove(Vector2 _target)
        {
            if (_rigidbody2D)
            {
                _movement.Move(_rigidbody2D, transform.position, _target, _properties.SpeedMove);
                RotationBullet(_target);
            }
            else
            {
                Debug.LogWarning("Bullet Rigidbody2D is missing.");
            }
        }

        public virtual void RotationBullet(Vector3 _target)
        {
            var currPoint = transform.position;
            var currDir = _target - currPoint;
            currDir.Normalize();
            var rotationZ = Mathf.Atan2(currDir.y, currDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        public virtual void Pop()
        {
            transform.SetParent(_parent);
            if (_collisionCollider)
                _collisionCollider.isTrigger = false;

            this.gameObject.SetActive(false);
            transform.tag = "Bullet";

            _gameFactory.CreateCollision(CollisionEffectId.Impact, this.transform.position);
        }

        public virtual void OnBecameInvisible()
        {
            Pop();
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }
    }
}
