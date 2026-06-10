using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SBabchuk
{
    public class AudioSettingsApplier : MonoBehaviour
    {
        private const float RefreshDelay = 0.5f;

        private static AudioSettingsApplier instance;

        private Coroutine refreshCoroutine;

        private PlayerPrefsDatabase Preferences
        {
            get
            {
                return PersistableSO.Instance ? PersistableSO.Instance.PlayerPrefs : PlayerPrefsDatabase.GetDatabase();
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Apply();
            StartRefresh();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public static void Apply()
        {
            if (instance)
                instance.ApplyInternal();
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
        {
            ApplyInternal();
            StartRefresh();
        }

        private void StartRefresh()
        {
            if (refreshCoroutine != null)
                StopCoroutine(refreshCoroutine);

            refreshCoroutine = StartCoroutine(RefreshRoutine());
        }

        private IEnumerator RefreshRoutine()
        {
            while (true)
            {
                ApplyInternal();
                yield return new WaitForSecondsRealtime(RefreshDelay);
            }
        }

        private void ApplyInternal()
        {
            var preferences = Preferences;
            var isMusicEnabled = !preferences || preferences.IsMusicEnabled();
            var isSoundEnabled = !preferences || preferences.IsSoundEnabled();
            var audioSources = FindObjectsOfType<AudioSource>(true);

            foreach (var source in audioSources)
            {
                if (!source)
                    continue;

                source.mute = source.loop ? !isMusicEnabled : !isSoundEnabled;
            }
        }
    }
}
