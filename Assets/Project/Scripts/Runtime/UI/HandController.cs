using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.BombStore;

namespace SBabchuk.Runtime.UI
{
    public class HandController : MonoBehaviour, IHandService
    {
        public bool IsHoldingGrenade => _focus;

        [SerializeField, FormerlySerializedAs("scaleToTouch"), Range(1, 2)]
        private float _scaleToTouch = 1f;

        [SerializeField, FormerlySerializedAs("touchCollider")]
        private Collider2D _touchCollider;

        [SerializeField, FormerlySerializedAs("collisionCollider")]
        private Collider2D _collisionCollider;

        private Collider2D _otherCollider;
        private Vector3 _positionForBack;
        private GrenadesName _currentGrenadeName;
        private SpriteRenderer _sRender;
        private GrenadeBttnController _grenade;
        private bool _focus = false;
        private bool _onTrigger;
        private string _tagPlace = "Place";
        private bool _isMovingToBack;
        private Tween _twn;

        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ILevelSpawnService _levelSpawnService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, ILevelSpawnService levelSpawnService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _levelSpawnService = levelSpawnService;
        }

        private void Awake()
        {
            _sRender = GetComponent<SpriteRenderer>();
        }

        public void Init(GrenadesName grenadeName, GrenadeBttnController grenade)
        {
            if (_isMovingToBack)
            {
                _twn?.Kill();
                CompleteMovingToBack();
            }

            _grenade = grenade;
            _currentGrenadeName = grenadeName;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(pos.x, pos.y, 0);
            _positionForBack = transform.position;
            var bombStore = _assetProvider.BombStoreDatabase;
            _sRender.sprite = bombStore.GetGrenade((int)grenadeName).Icon;
            OnTouchDown();
        }

        private void OnEnable()
        {
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        private void OnDisable()
        {
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_onTrigger)
            {
                _onTrigger = false;
                _otherCollider = null;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_otherCollider)
            {
                if (_otherCollider.CompareTag(other.tag))
                {
                    _onTrigger = true;
                }
            }
            else
            {
                if (other.CompareTag(_tagPlace))
                {
                    _otherCollider = other;
                    _onTrigger = true;
                }
            }
        }

        public void OnTouchMove(Gesture gesture)
        {
            if (_focus)
            {
                var pos = gesture.GetTouchToWorldPoint(9f);
                transform.position = pos;
            }
        }

        public void IsOverPlace()
        {
            if (_onTrigger)
                Checked();
            else
                SwimBackOnTable();
        }

        public void Checked()
        {
            _grenade.CheckIco(true);
            _progressService.UseGrenade((int)_currentGrenadeName);
            _levelSpawnService?.SpawnGrenadeOnPlace(_currentGrenadeName, transform.position);
            transform.position = new Vector3(100, 100, 0);
        }

        public void OnTouchDown()
        {
            if (!_focus)
            {
                if (this.gameObject.transform.localScale.x <= 1 * _scaleToTouch)
                    this.gameObject.transform.localScale *= _scaleToTouch;
                _focus = true;
            }
        }

        public void OnTouchUp(Gesture gesture)
        {
            if (_focus)
            {
                IsOverPlace();
                _focus = false;
            }
        }

        public void SwimBackOnTable(float time = 0.5f)
        {
            _twn = transform.DOLocalMove(_positionForBack, time).OnStart(() =>
            {
                _isMovingToBack = true;
                _touchCollider.enabled = false;
                _collisionCollider.enabled = false;
                _focus = false;
            }).OnComplete(() =>
            {
                CompleteMovingToBack();
            });
        }

        private void CompleteMovingToBack()
        {
            _grenade.CheckIco(true);
            transform.position = new Vector3(100, 100, 0);
            _touchCollider.enabled = true;
            _collisionCollider.enabled = true;
            _isMovingToBack = false;
            if (this.gameObject.transform.localScale.x > 1)
                this.gameObject.transform.localScale /= _scaleToTouch;
        }
    }
}
