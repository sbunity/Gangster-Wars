using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class NextWaveButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private UIPulseAnimation _pulse;

        private ILevelWaveControlService _waveControl;
        private SignalBus _signalBus;
        private RectTransform _rect;
        private Canvas _canvas;

        [Inject]
        public void Construct(ILevelWaveControlService waveControl, SignalBus signalBus)
        {
            _waveControl = waveControl;
            _signalBus = signalBus;
            RefreshInteractable();
        }

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_pulse == null)
                _pulse = GetComponent<UIPulseAnimation>();

            _rect = (RectTransform)transform;
            _canvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            _button.onClick.AddListener(HandleClick);
            RefreshInteractable();
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);
        }

        private void Update()
        {
            RefreshInteractable();
        }

        private void HandleClick()
        {
            var reward = _waveControl?.StartNextWave() ?? 0;
            if (reward > 0)
                RequestCoinFlight(reward);

            RefreshInteractable();
        }

        private void RequestCoinFlight(int reward)
        {
            if (_signalBus == null || _rect == null)
                return;

            var uiCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;
            var screenOrigin = RectTransformUtility.WorldToScreenPoint(uiCamera, _rect.position);
            _signalBus.Fire(new CoinFlightFromScreenRequestedSignal(screenOrigin, reward));
        }

        private void RefreshInteractable()
        {
            if (_waveControl == null)
                return;

            var canStartNextWave = _waveControl.CanStartNextWave;

            if (_button != null)
                _button.interactable = canStartNextWave;

            if (_pulse != null)
            {
                if (canStartNextWave)
                    _pulse.Play();
                else
                    _pulse.Stop();
            }
        }
    }
}
