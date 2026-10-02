using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Bonuses
{
    public class BonusController : MonoBehaviour
    {
        private const float AUTO_COLLECT_DELAY = 2f;
        private const float COLLECT_FLY_DURATION = 0.45f;
        private const float COLLECT_LINGER_DURATION = 0.1f;
        private const float COLLECT_END_SCALE_MULTIPLIER = 0.75f;

        [SerializeField, FormerlySerializedAs("weaponsName")]
        private WeaponsName _weaponsName;

        [SerializeField, FormerlySerializedAs("grenadesName")]
        private GrenadesName _grenadesName;

        private Tween _autoCollectTween;
        private Tween _collectTween;
        private IBonusRewardService _rewardService;
        private IBonusCollectTargetRegistry _collectTargetRegistry;
        private SignalBus _signalBus;
        private BonusView _view;
        private Vector3 _defaultScale;
        private bool _isCollecting;
        private bool _isRewardGranted;

        public BonusReward Reward => new(_weaponsName, _grenadesName);

        [Inject]
        public void Construct(
            IBonusRewardService rewardService,
            IBonusCollectTargetRegistry collectTargetRegistry,
            SignalBus signalBus)
        {
            _rewardService = rewardService;
            _collectTargetRegistry = collectTargetRegistry;
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
            _collectTween?.Kill();
            _isCollecting = false;
        }

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _view = GetOrAdd<BonusView>();
            InitColliders();
            _view.Initialize();
        }

        public void Init(Vector3 position)
        {
            this.gameObject.SetActive(true);
            _autoCollectTween?.Kill();
            _collectTween?.Kill();
            _collectTween = null;
            _isCollecting = false;
            _isRewardGranted = false;

            InitColliders();
            _view.Initialize();
            _view.RestoreSorting();
            transform.position = position;
            transform.localScale = _defaultScale;
            SetCollisionEnabled(true);
            _autoCollectTween = DOVirtual.DelayedCall(AUTO_COLLECT_DELAY, Collect);
        }

        private void InitColliders()
        {
            var touchCollider = GetComponent<Collider2D>();
            if (touchCollider == null)
                touchCollider = gameObject.AddComponent<PolygonCollider2D>();

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
            if (!gameObject.activeInHierarchy || _isCollecting)
                return;

            _isCollecting = true;
            _autoCollectTween?.Kill();
            SetCollisionEnabled(false);

            TryGetCollectTarget(out var target);
            PlayCollectAnimation(target);
        }

        private void PlayCollectAnimation(IBonusCollectTarget target)
        {
            var targetPosition = target != null
                ? ScreenToBonusWorldPosition(target.ScreenPosition)
                : transform.position;

            if (target != null)
                _view.RenderAbove(target.Canvas);

            _collectTween?.Kill();
            _collectTween = DOTween.Sequence()
                .Join(transform.DOMove(targetPosition, COLLECT_FLY_DURATION).SetEase(Ease.InOutSine))
                .Join(transform.DOScale(_defaultScale * COLLECT_END_SCALE_MULTIPLIER, COLLECT_FLY_DURATION).SetEase(Ease.InOutSine))
                .AppendInterval(COLLECT_LINGER_DURATION)
                .AppendCallback(() => CompleteCollection(target));
        }

        private void CompleteCollection(IBonusCollectTarget target)
        {
            _collectTween = null;
            GrantReward();
            target?.PlayCollectFeedback();
            Pop();
        }

        public void CollectImmediately()
        {
            if (!gameObject.activeSelf)
                return;

            GrantReward();
            Pop();
        }

        private void GrantReward()
        {
            if (_isRewardGranted)
                return;

            _isRewardGranted = true;
            _rewardService.Grant(Reward);
        }

        public void Pop()
        {
            _autoCollectTween?.Kill();
            _collectTween?.Kill();
            _collectTween = null;
            _signalBus.Fire(new BonusPoppedSignal(this));
            this.gameObject.SetActive(false);
        }

        private bool TryGetCollectTarget(out IBonusCollectTarget target)
        {
            if (_collectTargetRegistry == null)
            {
                target = null;
                return false;
            }

            if (_weaponsName != WeaponsName.None)
                return _collectTargetRegistry.TryGet(ShortInfoName.Weapon, (int)_weaponsName, out target);

            if (_grenadesName != GrenadesName.None)
                return _collectTargetRegistry.TryGet(ShortInfoName.Grenade, (int)_grenadesName, out target);

            target = null;
            return false;
        }

        private Vector3 ScreenToBonusWorldPosition(Vector3 screenPosition)
        {
            var camera = Camera.main;
            if (camera == null)
                return transform.position;

            screenPosition.z = Mathf.Abs(transform.position.z - camera.transform.position.z);
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = transform.position.z;
            return worldPosition;
        }

        private void SetCollisionEnabled(bool isEnabled)
        {
            if (_view?.CollisionCollider != null)
                _view.CollisionCollider.enabled = isEnabled;
        }

        private T GetOrAdd<T>()
            where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }
    }
}
