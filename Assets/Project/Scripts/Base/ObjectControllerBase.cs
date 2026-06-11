using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class ObjectControllerBase : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("collisionCollider")]
        private Collider2D _collisionCollider;

        [SerializeField, FormerlySerializedAs("touchCollider")]
        private Collider2D _touchCollider;

        [SerializeField, FormerlySerializedAs("otherCollider")]
        private Collider2D _otherCollider;

        [SerializeField, FormerlySerializedAs("rigidbody2D")]
        private Rigidbody2D _rigidbody2D;

        [SerializeField, FormerlySerializedAs("focus")]
        private bool _focus = false;

        [SerializeField, FormerlySerializedAs("onTrigger")]
        private bool _onTrigger;

        [SerializeField, FormerlySerializedAs("positionForBack")]
        private Vector3 _positionForBack;

        [SerializeField, FormerlySerializedAs("sRender")]
        private SpriteRenderer _spriteRenderer;

        [SerializeField, FormerlySerializedAs("sortingLayer")]
        private string _sortingLayer = "Default";

        [SerializeField, FormerlySerializedAs("orderInLayer")]
        private int _orderInLayer;

        [SerializeField, FormerlySerializedAs("transfor")]
        private Transform _transform;

        [SerializeField, FormerlySerializedAs("complete")]
        private bool _complete;

        private int countScale = 0;
        [SerializeField, FormerlySerializedAs("scaleToTouch")]
        private float _scaleToTouch = 1f;

        public virtual void Subscribe()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        public virtual void UnSubscribe()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        public virtual void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_transform == null)
                _transform = GetComponent<Transform>();
        }

        public virtual void Init()
        {
            InitColliders();
        }

        public void InitColliders()
        {
            _touchCollider = gameObject.AddComponent<PolygonCollider2D>();
            _touchCollider.isTrigger = true;
            _collisionCollider = _touchCollider;
            if (_rigidbody2D == null)
                _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        public virtual void SetSprite(Sprite _sprite)
        {
            _spriteRenderer.sprite = _sprite;
        }

        public virtual void SetPosition(Vector3 _position)
        {
            _transform.position = _position;
        }

        public virtual void SetSortingLayer(string _layer)
        {
            _spriteRenderer.sortingLayerName = _layer;
        }

        public virtual void SetOrderInLayer(int _order)
        {
            _spriteRenderer.sortingOrder = _order;
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (_onTrigger)
            {
                _onTrigger = false;
                _otherCollider = null;
            }
        }

        public virtual void OnTriggerStay2D(Collider2D other)
        {
            if (_otherCollider)
            {
                if (other.tag == _otherCollider.tag)
                {
                    _onTrigger = true;
                }
            }
            else
            {
            }
        }

        public virtual void OnTouchMove(Gesture gesture)
        {
            if (_focus)
            {
                Vector3 pos = gesture.GetTouchToWorldPoint(9f);
                transform.position = pos;
            }
        }

        public virtual void OnTouchDown(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                if (!_focus)
                {
                    Sorting(true);
                    _positionForBack = transform.position;
                    if (this.gameObject.transform.localScale.x <= 1 * _scaleToTouch)
                        this.gameObject.transform.localScale *= _scaleToTouch;
                    _focus = true;
                }
            }
        }

        public virtual void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                if (_focus)
                {
                    IsOverPlace();
                    _focus = false;
                }
            }
        }

        public virtual void IsOverPlace()
        {
            if (_onTrigger)
            {
                Checked();
            }
            else
            {
                SwimBackOnTable();
            }
        }

        public virtual void Checked()
        {
            if (_otherCollider)
            {
            }

            transform.position = new Vector3(100, 100, 0);
        }

        public virtual void SwimBackOnTable(float time = 0.5f)
        {
            transform.DOLocalMove(_positionForBack, time).OnStart(() =>
            {
                _touchCollider.enabled = false;
                _collisionCollider.enabled = false;
                _focus = false;
            }).OnComplete(() =>
            {
                Sorting(false);
                if (!_complete)
                {
                    _touchCollider.enabled = true;
                    _collisionCollider.enabled = true;
                }

                if (this.gameObject.transform.localScale.x > 1)
                    this.gameObject.transform.localScale /= _scaleToTouch;
            });
        }

        public virtual void Sorting(bool value)
        {
            if (value)
            {
                if (_spriteRenderer)
                {
                    countScale++;
                    _spriteRenderer.sortingOrder += 10;
                }
            }
            else
            {
                if (_spriteRenderer)
                {
                    _spriteRenderer.sortingOrder -= 10 * countScale;
                    countScale = 0;
                }
            }
        }
    }
}
