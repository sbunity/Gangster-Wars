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
            if (_button != null && _waveControl != null)
                _button.interactable = _waveControl.CanStartNextWave;
        }
    }
}
