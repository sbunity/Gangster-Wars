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

        [Inject]
        public void Construct(ILevelWaveControlService waveControl)
        {
            _waveControl = waveControl;
            RefreshInteractable();
        }

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_pulse == null)
                _pulse = GetComponent<UIPulseAnimation>();
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
            _waveControl?.StartNextWave();
            RefreshInteractable();
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
