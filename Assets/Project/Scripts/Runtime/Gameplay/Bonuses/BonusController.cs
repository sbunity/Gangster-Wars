using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Gameplay.Bonuses;
using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class BonusController : ObjectControllerBase
    {
        private const float FreeAmmoMagazineMultiplier = 0.6f;

        public WeaponsName weaponsName;

        public GrenadesName grenadesName;

        private Tween autoCollectTween;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;
        private BonusView _view;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public override void Subscribe()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public override void UnSubscribe()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
        }

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public void OnEnable()
        {
            Subscribe();
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public void OnDisable()
        {
            UnSubscribe();

            autoCollectTween?.Kill();
        }

        private void Start()
        {
            _view = GetOrAdd<BonusView>();
            _view.Initialize();
            //Init(this.transfor.position);
        }

        /// <summary>
        /// Ініціалізація властивостей
        /// </summary>
        public void Init(Vector3 _position)
        {
            this.gameObject.SetActive(true);

            autoCollectTween?.Kill();

            InitColliders();

            SetPosition(_position);

            autoCollectTween = DOVirtual.DelayedCall(2f, Collect);
        }

        /// <summary>
        /// Нажаття пальцем
        /// </summary>
        public override void OnTouchDown(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                Collect();
            }
        }

        private void Collect()
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (weaponsName != WeaponsName.None)
            {
                Weapon weapon = _assetProvider.WeaponStoreDatabase.GetWeapon((int)weaponsName);
                if (weapon != null)
                {
                    int ammoCount = Mathf.Max(1, Mathf.CeilToInt(weapon.magazine * FreeAmmoMagazineMultiplier));

                    _progressService.SetWeaponAmmo(weaponsName, ammoCount);
                }
            }
            else
            {
                _progressService.BuyGrenade((int)grenadesName, true);
            }

            Pop(); //Повертаємось в пул
        }

        /// <summary>
        /// Повернутись в пул
        /// </summary>
        public void Pop()
        {
            autoCollectTween?.Kill();

            _signalBus.Fire(new BonusPoppedSignal(this));

            this.gameObject.SetActive(false); //Виключаєм об*єкт
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
