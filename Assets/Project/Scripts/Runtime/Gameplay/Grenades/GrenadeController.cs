using DG.Tweening;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.BombStore;

namespace SBabchuk.Runtime.Gameplay.Grenades
{
    public class GrenadeController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_parent")]
        private Transform _parent;

        [SerializeField, FormerlySerializedAs("properties")]
        private Grenade _properties;
        public Grenade Properties { get => _properties; set => _properties = value; }

        private Collider2D _collisionCollider;
        private Tween _timerTween;
        private IAssetProvider _assetProvider;
        private IGameFactory _gameFactory;
        private GrenadeView _view;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IGameFactory gameFactory)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
        }

        private void Awake()
        {
            EnsureComponents();
        }

        private void Start()
        {
            EnsureComponents();
        }

        public bool Init(int id = 0, Vector3 position = default(Vector3))
        {
            gameObject.SetActive(true);
            EnsureComponents();

            if (_assetProvider == null || _assetProvider.BombStoreDatabase == null)
            {
                Debug.LogWarning("GrenadeController cannot initialize without BombStoreDatabase.", this);
                gameObject.SetActive(false);
                return false;
            }

            transform.position = position;
            var bombStore = _assetProvider.BombStoreDatabase;
            _properties = bombStore.GetGrenade(id);
            if (_properties == null)
            {
                Debug.LogWarning($"Grenade data with id {id} was not found.", this);
                gameObject.SetActive(false);
                return false;
            }

            if (_collisionCollider == null)
            {
                _collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
                _view.Initialize();
            }

            _collisionCollider.isTrigger = false;
            if (_properties.TriggerType != GrenadeTriggerType.Contact)
                Action(_properties.Delay);
            return true;
        }

        // Detonation request raised when an enemy steps on / touches the grenade.
        // Only grenades whose policy includes contact (mines and contact-fused grenades)
        // react; purely timed grenades ignore it.
        public void NotifyContact()
        {
            if (_properties != null && _properties.TriggerType != GrenadeTriggerType.Timed)
                Action(0);
        }

        public void Action(float delay)
        {
            Timer(delay);
        }

        public void Timer(float delay)
        {
            _timerTween?.Kill();
            _timerTween = DOVirtual.DelayedCall(delay, Explosion);
        }

        public void Explosion()
        {
            Pop();
            _gameFactory.CreateCollision((int)_properties.Collision, transform.position, _properties);
        }

        public void Pop()
        {
            transform.SetParent(_parent);
            _collisionCollider.isTrigger = false;
            this.gameObject.SetActive(false);
            transform.tag = "Grenade";
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }

        private void EnsureComponents()
        {
            if (_view == null)
                _view = GetOrAdd<GrenadeView>();

            _view.Initialize();

            if (_collisionCollider == null)
                _collisionCollider = GetComponent<Collider2D>();

            if (_parent == null)
                _parent = transform.parent;
        }
    }
}
