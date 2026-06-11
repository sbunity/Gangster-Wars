using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class HandController : MonoBehaviour, IHandService
    {
        public bool IsHoldingGrenade => focus;

        [Header("Скейл іконки вже в руці")]
        [Range(1, 2)]
        public float scaleToTouch = 1f;

        [Header("Тач колайдер")]
        public Collider2D touchCollider;

        [Header("Солайдер для колізій")]
        public Collider2D collisionCollider;

        /// <summary>
        /// Колайдер з яким взаємодієм
        /// </summary>
        private Collider2D otherCollider;

        /// <summary>
        /// Позиція, куди варто повернутись
        /// </summary>
        private Vector3 positionForBack;

        /// <summary>
        /// поточне ім*я гранати
        /// </summary>
        private GrenadesName currentGrenadeName;

        /// <summary>
        /// SpriteRenderer - компонент
        /// </summary>
        private SpriteRenderer sRender;

        /// <summary>
        /// Вказівник на кнопку, по нажаття якої ми тут
        /// </summary>
        private GrenadeBttnController grenade;

        /// <summary>
        /// Чи здійснений фокус
        /// </summary>
        private bool focus = false;

        /// <summary>
        /// Чи є зіткнення з колайдером
        /// </summary>
        private bool onTrigger;

        /// <summary>
        /// Тег для Place
        /// </summary>
        private string tagPlace = "Place";

        /// <summary>
        /// Чи зараз в русі
        /// </summary>
        private bool isMovingToBack;

        /// <summary>
        /// Тимчасова змінна для твіна
        /// </summary>
        Tween twn;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ILevelRuntimeService _levelRuntimeService;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            ILevelRuntimeService levelRuntimeService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _levelRuntimeService = levelRuntimeService;
        }

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        private void Awake()
        {
            sRender = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Ініціалізація
        /// </summary>
        public void Init(GrenadesName _grenadeName, GrenadeBttnController _grenade)
        {
            if (isMovingToBack)
            {
                twn?.Kill();
                CompleteMovingToBack();
            }

            grenade = _grenade;

            currentGrenadeName = _grenadeName;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector3(pos.x, pos.y, 0);

            positionForBack = transform.position;

            var bombStore = _assetProvider.BombStoreDatabase;
            sRender.sprite = bombStore.GetGrenade((int)_grenadeName).ico;

            OnTouchDown();
        }

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public void OnEnable()
        {
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public void OnDisable()
        {
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        /// <summary>
        /// Закінчилась взаємодія по колайдерам
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerExit2D(Collider2D other)
        {
            if (onTrigger)
            {
                onTrigger = false;

                otherCollider = null;
            }
        }

        /// <summary>
        /// Взаємодія по колайдерам
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerStay2D(Collider2D other)
        {
            if (otherCollider)
            {
                if (other.tag == otherCollider.tag)
                {
                    onTrigger = true;
                }
            }
            else
            {
                if (other.tag == tagPlace)
                {
                    otherCollider = other;

                    onTrigger = true;
                }
            }
        }

        /// <summary>
        /// Рухаєм
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        public void OnTouchMove(Gesture gesture)
        {
            if (focus)
            {
                Vector3 pos = gesture.GetTouchToWorldPoint(9f);
                transform.position = pos;
            }
        }

        /// <summary>
        /// Чи ми зараз над місцем призначення
        /// </summary>
        public void IsOverPlace()
        {
            if (onTrigger)
            {
                Checked(); //правильно поставили
            }
            else
            {
                SwimBackOnTable(); //повертаєм назад, якщо неправильно поставили
            }
        }

        /// <summary>
        /// Перевірка
        /// </summary>
        public void Checked()
        {
            Debug.Log("Checked");

            grenade.CheckIco(true);

            _progressService.UseGrenade((int)currentGrenadeName);

            if (_levelRuntimeService != null)
                _levelRuntimeService.SpawnGrenadeOnPlace(currentGrenadeName, transform.position);

            transform.position = new Vector3(100, 100, 0); //ховаєм руку
        }

        /// <summary>
        /// Метод, що імітує нажаття
        /// </summary>
        public void OnTouchDown()
        {
            if (!focus)
            {
                if (this.gameObject.transform.localScale.x <= 1 * scaleToTouch)
                    this.gameObject.transform.localScale *= scaleToTouch;

                focus = true;
            }
        }

        /// <summary>
        /// Коли відпустили тач
        /// </summary>
        /// <param name="gesture"></param>
        public void OnTouchUp(Gesture gesture)
        {
             if (focus)
             {
                 IsOverPlace();

                 focus = false;
             }
        }

        /// <summary>
        /// Повертаэм на місце
        /// </summary>
        /// <param name="time">Time.</param>
        public void SwimBackOnTable(float time = 0.5f)
        {
            twn = transform.DOLocalMove(positionForBack, time)
                .OnStart(() => 
                {
                    isMovingToBack = true;

                    touchCollider.enabled = false;

                    collisionCollider.enabled = false;
                    
                    focus = false;
                })
                .OnComplete(() => 
                {
                    CompleteMovingToBack();
                });
        }

        /// <summary>
        /// Що треба зробити, коли повернулись на місце
        /// </summary>
        private void CompleteMovingToBack()
        {
            grenade.CheckIco(true);

            transform.position = new Vector3(100, 100, 0);

            touchCollider.enabled = true;

            collisionCollider.enabled = true;

            isMovingToBack = false;

            if (this.gameObject.transform.localScale.x > 1)
                this.gameObject.transform.localScale /= scaleToTouch;
        }
    }
}
