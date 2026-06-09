using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SBabchuk
{
    public class BonusController : ObjectControllerBase
    {
        public delegate void Poped (BonusController _value);
        public static event Poped OnPoped;

        public WeaponsName weaponsName;

        public GrenadesName grenadesName;

        private Tween autoCollectTween;

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

            Utils.StopTween(autoCollectTween);
        }

        private void Start()
        {
            //Init(this.transfor.position);
        }

        /// <summary>
        /// Ініціалізація властивостей
        /// </summary>
        public void Init(Vector3 _position)
        {
            this.gameObject.SetActive(true);

            Utils.StopTween(autoCollectTween);

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
                PersistableSO.Instance.PlayerPrefs.BuyMagazine((int)weaponsName, true);
            }
            else
            {
                PersistableSO.Instance.PlayerPrefs.BuyGrenade((int)grenadesName, true);
            }

            Pop(); //Повертаємось в пул
        }

        /// <summary>
        /// Повернутись в пул
        /// </summary>
        public void Pop()
        {
            Utils.StopTween(autoCollectTween);

            OnPoped?.Invoke(this);

            this.gameObject.SetActive(false); //Виключаєм об*єкт
        }
    }
}
