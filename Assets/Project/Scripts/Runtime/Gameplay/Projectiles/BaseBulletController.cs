using UnityEngine;
using SBabchuk.Runtime.Gameplay.Projectiles;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class BaseBulletController : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rigidbody2d;

        [HideInInspector]
        public Transform _parent;

        [HideInInspector]
        public Bullet properties;

        [HideInInspector]
        public int damage;

        [HideInInspector]
        public Collider2D collisionCollider;

        private SortingEnemy sorting;
        private IAssetProvider _assetProvider;
        private IGameFactory _gameFactory;
        private ILevelRuntimeService _levelRuntimeService;
        private IAimService _aimService;
        private ProjectileView _view;
        private ProjectileMovement _movement;
        protected ILevelRuntimeService LevelRuntimeService => _levelRuntimeService;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IGameFactory gameFactory,
            ILevelRuntimeService levelRuntimeService,
            IAimService aimService)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _levelRuntimeService = levelRuntimeService;
            _aimService = aimService;
        }

        public virtual void Awake()
        {
            _view = GetOrAdd<ProjectileView>();
            _movement = GetOrAdd<ProjectileMovement>();
            _view.Initialize();

            rigidbody2d = _view.Rigidbody;

            sorting = _view.Sorting;
        }

        public virtual void Start()
        {
            _parent = transform.parent;
        }

        public virtual void Init(int _id = 0, int _damage = 0, Vector3 _position = default(Vector3), Vector3 _target = default(Vector3), float _offset = 0)
        {
            this.gameObject.SetActive(true);

            if (sorting)
                sorting.AnchorOffset = -_offset;

            transform.position = _position;

            damage = _damage;

            var bulletDatabase = _assetProvider.BulletDatabase;
            properties = bulletDatabase.GetBullet(_id);

            StartMove((_target != default(Vector3)) ? (Vector2)_target : _aimService.CurrentAimPosition);

            if (collisionCollider == null)
            {
                collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
                _view.Initialize();
            }

            collisionCollider.isTrigger = false;
        }

        public virtual void StartMove(Vector2 _target)
        {
            if (rigidbody2d)
            {
                _movement.Move(rigidbody2d, transform.position, _target, properties.speedMove);

                RotationBullet(_target);
            }
            else
            {
                Debug.Log("rigidbody2d == null");
            }
        }

        public virtual void RotationBullet(Vector3 _target)
        {
            Vector3 currPoint = transform.position;

            Vector3 currDir = _target - currPoint;

            currDir.Normalize();

            float rotationZ = Mathf.Atan2(currDir.y, currDir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        public virtual void Pop()
        {
            transform.SetParent(_parent);

            if (collisionCollider)
                collisionCollider.isTrigger = false;

            this.gameObject.SetActive(false);

            transform.tag = "Bullet";

            if (_gameFactory != null)
                _gameFactory.CreateCollision(7, this.transform.position);
            else if (_levelRuntimeService != null)
                _levelRuntimeService.SpawnCollision(7, this.transform.position);
        }

        public virtual void OnBecameInvisible()
        {
            Pop();
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
