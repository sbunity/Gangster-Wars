using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadWeaponUIInfo : MonoBehaviour, IBonusCollectTarget
    {
        [SerializeField, FormerlySerializedAs("type")]
        private WeaponsName _type;
        public WeaponsName Type { get => _type; set => _type = value; }

        [SerializeField, FormerlySerializedAs("count")]
        private Text _count;

        [SerializeField, FormerlySerializedAs("bttn")]
        private Button _bttn;

        private SignalSubscriptions _signals;
        private IBonusCollectTargetRegistry _collectTargets;
        private CountPulse _countPulse;
        private bool _isRegistered;

        public Vector3 ScreenPosition => GetScreenPosition();

        [Inject]
        public void Construct(SignalBus signalBus, IBonusCollectTargetRegistry collectTargets)
        {
            _collectTargets = collectTargets;
            _signals = new SignalSubscriptions(signalBus)
                .Add<WeaponAmmoChangedSignal>(OnWeaponAmmoChanged);

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
            _signals?.Enable();
            TryRegister();
        }

        private void OnDisable()
        {
            Unregister();
            _signals?.Disable();
        }

        private void OnWeaponAmmoChanged(WeaponAmmoChangedSignal signal)
        {
            UpdateWeaponPatrons(signal.Weapon, signal.Count);
        }

        public void Initialized(int count)
        {
            _count.text = count.ToString();
        }

        public void UpdateWeaponPatrons(WeaponsName weaponsName, int count)
        {
            if (_type == weaponsName)
                _count.text = count.ToString();
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

            _collectTargets.Register(ShortInfoName.Weapon, (int)_type, this);
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered || _collectTargets == null)
                return;

            _collectTargets.Unregister(ShortInfoName.Weapon, (int)_type, this);
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
