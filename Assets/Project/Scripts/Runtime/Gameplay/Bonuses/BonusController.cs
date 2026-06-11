using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BonusController : MonoBehaviour
    {
        private const float FREE_AMMO_MAGAZINE_MULTIPLIER = 0.6f;

        [SerializeField, FormerlySerializedAs("weaponsName")]
        private WeaponsName _weaponsName;

        [SerializeField, FormerlySerializedAs("grenadesName")]
        private GrenadesName _grenadesName;

        private Tween _autoCollectTween;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;
        private BonusView _view;
        
        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void Subscribe()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
        }

        private void UnSubscribe()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
            _autoCollectTween?.Kill();
        }

        private void Start()
        {
            _view = GetOrAdd<BonusView>();
            _view.Initialize();
        }

        public void Init(Vector3 position)
        {
            this.gameObject.SetActive(true);
            _autoCollectTween?.Kill();
            InitColliders();
            transform.position = position;
            _autoCollectTween = DOVirtual.DelayedCall(2f, Collect);
        }

        // The bonus needs a kinematic trigger collider so EasyTouch can pick it and trigger collection.
        private void InitColliders()
        {
            var touchCollider = gameObject.AddComponent<PolygonCollider2D>();
            touchCollider.isTrigger = true;

            var body = GetComponent<Rigidbody2D>();
            if (body == null)
                body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnTouchDown(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
                Collect();
        }

        private void Collect()
        {
            if (!gameObject.activeInHierarchy)
                return;
            if (_weaponsName != WeaponsName.None)
            {
                var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon((int)_weaponsName);
                if (weapon != null)
                {
                    var ammoCount = Mathf.Max(1, Mathf.CeilToInt(weapon.Magazine * FREE_AMMO_MAGAZINE_MULTIPLIER));
                    _progressService.SetWeaponAmmo(_weaponsName, ammoCount);
                }
            }
            else
            {
                _progressService.BuyGrenade((int)_grenadesName, true);
            }

            Pop();
        }

        public void Pop()
        {
            _autoCollectTween?.Kill();
            _signalBus.Fire(new BonusPoppedSignal(this));
            this.gameObject.SetActive(false);
        }

        private T GetOrAdd<T>()
            where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }
    }
}