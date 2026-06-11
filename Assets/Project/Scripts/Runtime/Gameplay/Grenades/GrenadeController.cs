using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class GrenadeController : MonoBehaviour
    {
        [HideInInspector]
        public Transform _parent;

        [Header("Колайдер для зіткнення")]
        private Collider2D collisionCollider;

        [HideInInspector]
        public Grenade properties;

        private Tween twn;
        private IAssetProvider _assetProvider;
        private IGameFactory _gameFactory;
        private ILevelRuntimeService _levelRuntimeService;
        private SignalBus _signalBus;
        private GrenadeView _view;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IGameFactory gameFactory,
            ILevelRuntimeService levelRuntimeService,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _levelRuntimeService = levelRuntimeService;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Стартова ініціазізація
        /// </summary>
        void Start()
        {
            _view = GetOrAdd<GrenadeView>();
            _view.Initialize();
            _parent = transform.parent;
        }

        /// <summary>
        /// Ініціалізація
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_damage"></param>
        /// <param name="_position"></param>
        /// <param name="_target"></param>
        public void Init(int _id = 0, int _damage = 0, Vector3 _position = default(Vector3), Transform _target = null)
        {
            this.gameObject.SetActive(true);

            transform.position = _position;

            var bombStore = _assetProvider.BombStoreDatabase;
            properties = bombStore.GetGrenade(_id);

            if (collisionCollider == null)
            {
                collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
                _view.Initialize();
            }

            collisionCollider.isTrigger = false;

            Action(properties.delay);
        }

       /// <summary>
       /// Запускаєм дію
       /// </summary>
        public void Action(float _value)
        {
            Timer(_value);
        }

        /// <summary>
        /// Таймер
        /// </summary>
        /// <param name="_delay">Затримка до зриву</param>
        public void Timer(float _delay)
        {
            twn?.Kill();

            twn = DOVirtual.DelayedCall(_delay, ()=> {
                Explosion();
            });
        }

        /// <summary>
        /// Добавлення ефекта зриву
        /// </summary>
        public void Explosion()
        {
            Pop();

            if (_gameFactory != null)
                _gameFactory.CreateCollision((int)properties.collision, transform.position, properties);
            else if (_levelRuntimeService != null)
                _levelRuntimeService.SpawnCollision((int)properties.collision, transform.position, properties);
        }

        /// <summary>
		/// Повернення в пуш
		/// </summary>
        public void Pop()
        {
            _signalBus.Fire(new GrenadeDamageSignal(transform.position, properties.damage, properties.radius));
            
            transform.SetParent(_parent);
            
            collisionCollider.isTrigger = false;
            
            this.gameObject.SetActive(false);

            transform.tag = "Grenade";
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
