using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadGrenadeInfoUI : MonoBehaviour, IBonusCollectTarget
    {
        [SerializeField, FormerlySerializedAs("type")]
        private GrenadesName _type;
        public GrenadesName Type { get => _type; set => _type = value; }

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("count")]
        private Text _count;

        private IBonusCollectTargetRegistry _collectTargets;
        private CountPulse _countPulse;
        private bool _isRegistered;

        public Vector3 ScreenPosition => GetScreenPosition();

        [Inject]
        public void Construct(IBonusCollectTargetRegistry collectTargets)
        {
            _collectTargets = collectTargets;
            TryRegister();
        }

        private void Awake()
        {
            _countPulse = GetOrAdd<CountPulse>();
            if (_count != null)
                _countPulse.SetTarget(_count.transform);
        }

        private void OnEnable()
        {
            TryRegister();
        }

        private void OnDisable()
        {
            Unregister();
        }

        public void Initialized(Sprite icon, int count)
        {
            _count.text = count.ToString();
            _icon.sprite = icon;
        }

        public void PlayCollectFeedback()
        {
            _countPulse ??= GetOrAdd<CountPulse>();
            if (_count != null)
                _countPulse.SetTarget(_count.transform);
            _countPulse.Play();
        }

        private void TryRegister()
        {
            if (_isRegistered || _collectTargets == null || !isActiveAndEnabled)
                return;

            _collectTargets.Register(ShortInfoName.Grenade, (int)_type, this);
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered || _collectTargets == null)
                return;

            _collectTargets.Unregister(ShortInfoName.Grenade, (int)_type, this);
            _isRegistered = false;
        }

        private Vector3 GetScreenPosition()
        {
            var rectTransform = _count != null ? _count.rectTransform : transform as RectTransform;
            if (rectTransform == null)
                return transform.position;

            var canvas = rectTransform.GetComponentInParent<Canvas>();
            var camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            return RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);
        }

        private T GetOrAdd<T>()
            where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
