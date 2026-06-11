using UnityEngine;
using UnityEngine.UI;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class SettingsPopupController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool pauseGameOnOpen;

        private float _previousTimeScale = 1f;
        private bool _isOpened;
        private bool _isInitialized;
        private IAudioSettingsService _audioSettingsService;

        [Inject]
        public void Construct(IAudioSettingsService audioSettingsService)
        {
            _audioSettingsService = audioSettingsService;
        }

        private void Awake()
        {
            if (closeButton)
                closeButton.onClick.AddListener(Close);
            
            if (musicToggle)
                musicToggle.onValueChanged.AddListener(SetMusicEnabled);
            
            if (soundToggle)
                soundToggle.onValueChanged.AddListener(SetSoundEnabled);
        }

        private void Start()
        {
            RefreshView();
        }

        private void OnDestroy()
        {
            if (closeButton)
                closeButton.onClick.RemoveListener(Close);
            
            if (musicToggle)
                musicToggle.onValueChanged.RemoveListener(SetMusicEnabled);
            
            if (soundToggle)
                soundToggle.onValueChanged.RemoveListener(SetSoundEnabled);
        }

        public void Open()
        {
            RefreshView();
            if (pauseGameOnOpen && !_isOpened)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            _isOpened = true;
            if (panel)
                panel.SetActive(true);
        }

        public void Close()
        {
            if (pauseGameOnOpen && _isOpened)
                Time.timeScale = _previousTimeScale;
            
            _isOpened = false;
            CloseWithoutTimeScale();
        }

        public void SetMusicEnabled(bool _value)
        {
            if (!_isInitialized)
                return;
            
            _audioSettingsService?.SetMusicEnabled(_value);
        }

        public void SetSoundEnabled(bool _value)
        {
            if (!_isInitialized)
                return;
            
            _audioSettingsService?.SetSoundEnabled(_value);
        }

        private void RefreshView()
        {
            if (musicToggle)
                musicToggle.SetIsOnWithoutNotify(_audioSettingsService == null || _audioSettingsService.IsMusicEnabled);
            
            if (soundToggle)
                soundToggle.SetIsOnWithoutNotify(_audioSettingsService == null || _audioSettingsService.IsSoundEnabled);
            
            _isInitialized = true;
        }

        private void CloseWithoutTimeScale()
        {
            if (panel)
                panel.SetActive(false);
        }
    }
}