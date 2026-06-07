using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class ObjectControllerBase : MonoBehaviour
    {
        /// <summary>
        /// Колайдер колізій
        /// </summary>
        [HideInInspector] public Collider2D collisionCollider;

        /// <summary>
        /// Колайдер для тачаів
        /// </summary>
        [HideInInspector] public Collider2D touchCollider;

        /// <summary>
        /// Колайдер з яким відбувалсь колізія
        /// </summary>
        [HideInInspector] public Collider2D otherCollider;

        /// <summary>
        /// Компонент Rigidbody2D
        /// </summary>
        [HideInInspector] public Rigidbody2D rigidbody2D;

        /// <summary>
        /// Чи під фокусом, тобто нажали на ньому
        /// </summary>
        [HideInInspector] public bool focus = false;

        /// <summary>
        /// Чи відбувається колізія
        /// </summary>
        //[HideInInspector]
        [HideInInspector] public bool onTrigger;

        /// <summary>
        /// Позиція куди варто повернутись при потребі
        /// </summary>
        [HideInInspector] public Vector3 positionForBack;

        /// <summary>
        /// Компонент SpriteRenderer
        /// </summary>
        [HideInInspector] public SpriteRenderer sRender;

        /// <summary>
        /// Слой, на якому знаходиться
        /// </summary>
        [HideInInspector] public string sortingLayer = "Default";

        /// <summary>
        /// Індек на слої
        /// </summary>
        [HideInInspector] public int orderInLayer;

        /// <summary>
        /// Компонент Transform
        /// </summary>
        [HideInInspector] public Transform transfor;

        /// <summary>
        /// Завершеність дії при поверненні
        /// </summary>
        [HideInInspector] public bool complete;

        /// <summary>
        /// Скільки разів ми вже проскейлили об*єкт
        /// </summary>
        private int countScale = 0;

        [Header("На скыльки варто збыльшити об*экт при його активносты")]
        [HideInInspector] public float scaleToTouch = 1f;

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public virtual void Subscribe()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public virtual void UnSubscribe()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        public virtual void Awake()
        {
            if (sRender == null)
                sRender = GetComponent<SpriteRenderer>();

            transfor = GetComponent<Transform>();

            //collisionCollider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Ініціалізація властивостей
        /// </summary>
        public virtual void Init()
        {
            InitColliders();
        }

        /// <summary>
        /// Ініціалізація колайдерів
        /// </summary>
        public void InitColliders()
        {
            touchCollider = gameObject.AddComponent<PolygonCollider2D>();

            touchCollider.isTrigger = true;

            collisionCollider = touchCollider;

            if (rigidbody2D == null)
                rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        /// <summary>
        /// Встановлення картинки
        /// </summary>
        public virtual void SetSprite(Sprite _sprite)
        {
            sRender.sprite = _sprite;
        }

        /// <summary>
        /// Встановлення позиції
        /// </summary>
        public virtual void SetPosition(Vector3 _position)
        {
            transfor.position = _position;
        }

        /// <summary>
        /// Встановлення SortingLayer
        /// </summary>
        public virtual void SetSortingLayer(string _layer)
        {
            sRender.sortingLayerName = _layer;
        }

        /// <summary>
        /// Встановлення SortingOrder
        /// </summary>
        public virtual void SetOrderInLayer(int _order)
        {
            sRender.sortingOrder = _order;
        }

        /// <summary>
        /// Raises the trigger exit2 d event.
        /// </summary>
        /// <param name="other">Other.</param>
        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (onTrigger)
            {
                onTrigger = false;

                otherCollider = null;
            }
        }

        /// <summary>
        /// Поки є колізії
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerStay2D(Collider2D other)
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

                //if (other.tag == confessionTag)
                //if (currentStickersName == other.GetComponent<StickerController>().stickersName)
                //{
                //    otherCollider = other;
                //    onTrigger = true;
                //}
            }
        }

        /// <summary>
        /// Рухаєм тачом
        /// </summary>
        public virtual void OnTouchMove(Gesture gesture)
        {
            if (focus)
            {
                Vector3 pos = gesture.GetTouchToWorldPoint(9f);
                transform.position = pos;
            }
        }

        /// <summary>
        /// Нажаття пальцем
        /// </summary>
        public virtual void OnTouchDown(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                if (!focus)
                {
                    Sorting(true);

                    positionForBack = transform.position;

                    if (this.gameObject.transform.localScale.x <= 1 * scaleToTouch)
                        this.gameObject.transform.localScale *= scaleToTouch;

                    focus = true;
                }
            }
        }

        /// <summary>
        /// Привідпусканні пальця
        /// </summary>
        public virtual void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                if (focus)
                {
                    IsOverPlace();
                    focus = false;
                }
            }
        }

        /// <summary>
        /// Перевірка на знаходження
        /// </summary>
        public virtual void IsOverPlace()
        {
            if (onTrigger)
            {
                Checked();
            }
            else
            {
                SwimBackOnTable();//napevno cey rjadochok zayvuy
            }
        }

        /// <summary>
        /// Перевірка, тут треба зробити дії при правильній позиції
        /// </summary>
        public virtual void Checked()
        {
            if (otherCollider)
            {
                //otherCollider.gameObject.GetComponent<StickerController>().Check();
            }

            transform.position = new Vector3(100, 100, 0);
        }
       
        /// <summary>
        /// Повертаэм на місце
        /// </summary>
        public virtual void SwimBackOnTable(float time = 0.5f)
        {
            transform.DOLocalMove(positionForBack, time)
                .OnStart(() => {
                    touchCollider.enabled = false;
                    collisionCollider.enabled = false;
                    focus = false;
                })
                .OnComplete(() =>
                {
                    Sorting(false);

                    if (!complete)
                    {
                        touchCollider.enabled = true;
                        collisionCollider.enabled = true;
                    }

                    if (this.gameObject.transform.localScale.x > 1)
                        this.gameObject.transform.localScale /= scaleToTouch;
                });
        }

        /// <summary>
        /// Сортування
        /// </summary>
        /// <param name="value"></param>
        public virtual void Sorting(bool value)
        {
            if (value)
            {
                if (sRender)
                {
                    countScale++;
                    sRender.sortingOrder += 10;
                }
            }
            else
            {
                if (sRender)
                {
                    sRender.sortingOrder -= 10 * countScale;
                    countScale = 0;
                }
            }
        }
    }
}

