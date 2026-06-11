using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
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
        private ILevelRuntimeService _levelRuntimeService;
        private SignalBus _signalBus;
        private GrenadeView _view;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IGameFactory gameFactory, ILevelRuntimeService levelRuntimeService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _levelRuntimeService = levelRuntimeService;
            _signalBus = signalBus;
        }

        private void Start()
        {
            _view = GetOrAdd<GrenadeView>();
            _view.Initialize();
            _parent = transform.parent;
        }

        public void Init(int id = 0, int damage = 0, Vector3 position = default(Vector3), Transform target = null)
        {
            this.gameObject.SetActive(true);
            transform.position = position;
            var bombStore = _assetProvider.BombStoreDatabase;
            _properties = bombStore.GetGrenade(id);
            if (_collisionCollider == null)
            {
                _collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
                _view.Initialize();
            }

            _collisionCollider.isTrigger = false;
            Action(_properties.Delay);
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
            if (_gameFactory != null)
                _gameFactory.CreateCollision((int)_properties.Collision, transform.position, _properties);
            else
                _levelRuntimeService?.SpawnCollision((int)_properties.Collision, transform.position, _properties);
        }

        public void Pop()
        {
            _signalBus.Fire(new GrenadeDamageSignal(transform.position, _properties.Damage, _properties.Radius));
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
    }
}