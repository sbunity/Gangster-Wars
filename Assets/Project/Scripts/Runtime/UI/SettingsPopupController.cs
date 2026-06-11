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

        private float previousTimeScale = 1f;
        private bool isOpened;
        private bool isInitialized;
        private IAudioSettingsService _audioSettingsService;

        [Inject]
        private void Construct(IAudioSettingsService audioSettingsService)
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

            if (pauseGameOnOpen && !isOpened)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            isOpened = true;

            if (panel)
                panel.SetActive(true);
        }

        public void Close()
        {
            if (pauseGameOnOpen && isOpened)
                Time.timeScale = previousTimeScale;

            isOpened = false;
            CloseWithoutTimeScale();
        }

        public void SetMusicEnabled(bool _value)
        {
            if (!isInitialized)
                return;

            _audioSettingsService?.SetMusicEnabled(_value);
        }

        public void SetSoundEnabled(bool _value)
        {
            if (!isInitialized)
                return;

            _audioSettingsService?.SetSoundEnabled(_value);
        }

        private void RefreshView()
        {
            if (musicToggle)
                musicToggle.SetIsOnWithoutNotify(_audioSettingsService == null || _audioSettingsService.IsMusicEnabled);

            if (soundToggle)
                soundToggle.SetIsOnWithoutNotify(_audioSettingsService == null || _audioSettingsService.IsSoundEnabled);

            isInitialized = true;
        }

        private void CloseWithoutTimeScale()
        {
            if (panel)
                panel.SetActive(false);
        }
    }
}
