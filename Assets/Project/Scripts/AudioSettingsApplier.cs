using System.Collections;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using SBabchuk.Runtime.Architecture;

namespace SBabchuk
{
    public class AudioSettingsApplier : MonoBehaviour
    {
        private const float REFRESH_DELAY = 0.5f;

        [SerializeField] private AudioSource[] _audioSources;

        private Coroutine _refreshCoroutine;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _signalBus.Subscribe<AudioSettingsChangedSignal>(ApplyInternal);
            ApplyInternal();
            StartRefresh();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _signalBus.Unsubscribe<AudioSettingsChangedSignal>(ApplyInternal);

            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
        {
            ApplyInternal();
            StartRefresh();
        }

        private void StartRefresh()
        {
            if (_refreshCoroutine != null)
                StopCoroutine(_refreshCoroutine);

            _refreshCoroutine = StartCoroutine(RefreshRoutine());
        }

        private IEnumerator RefreshRoutine()
        {
            while (true)
            {
                ApplyInternal();
                yield return new WaitForSecondsRealtime(REFRESH_DELAY);
            }
        }

        private void ApplyInternal()
        {
            var preferences = _progressService.Preferences;
            var isMusicEnabled = preferences.IsMusicEnabled();
            var isSoundEnabled = preferences.IsSoundEnabled();
            var audioSources = _audioSources != null && _audioSources.Length > 0 ? _audioSources : GetComponentsInChildren<AudioSource>(true);
            
            foreach (var source in audioSources)
            {
                if (!source)
                    continue;
                    
                source.mute = source.loop ? !isMusicEnabled : !isSoundEnabled;
            }
        }
    }
}