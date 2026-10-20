using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SBabchuk.Runtime.Services
{
    public sealed class AudioService : IAudioService, IInitializable, IDisposable
    {
        private const int PrewarmVoices = 8;
        private const int MaxVoices = 24;

        private readonly IAudioSettingsService _audioSettings;
        private readonly SignalSubscriptions _signals;
        private readonly Dictionary<SoundConfig, float> _lastPlayTimes = new();

        private GameObject _root;
        private AudioSourcePool _pool;

        public AudioService(IAudioSettingsService audioSettings, SignalBus signalBus)
        {
            _audioSettings = audioSettings;
            _signals = new SignalSubscriptions(signalBus)
                .Add<AudioSettingsChangedSignal>(ApplySettings);
        }

        private AudioSourcePool Pool => _pool ??= CreatePool();

        public void Initialize()
            => _signals.Enable();

        public void Dispose()
        {
            _signals.Disable();

            if (_root != null)
                Object.Destroy(_root);
        }

        public void Play(SoundConfig sound)
        {
            if (sound == null || !_audioSettings.IsSoundEnabled || !TryPassInterval(sound))
                return;

            var clip = sound.GetRandomClip();
            if (clip == null)
                return;

            Pool.Play(sound, clip);
        }

        private bool TryPassInterval(SoundConfig sound)
        {
            if (sound.MinInterval <= 0f)
                return true;

            var now = Time.unscaledTime;
            if (_lastPlayTimes.TryGetValue(sound, out var lastTime) && now - lastTime < sound.MinInterval)
                return false;

            _lastPlayTimes[sound] = now;
            return true;
        }

        private void ApplySettings()
        {
            if (_pool != null)
                ApplySettings(_pool);
        }

        private void ApplySettings(AudioSourcePool pool)
            => pool.SetMuted(!_audioSettings.IsSoundEnabled);

        private AudioSourcePool CreatePool()
        {
            _root = new GameObject("[Audio]");
            Object.DontDestroyOnLoad(_root);
            var pool = new AudioSourcePool(_root.transform, PrewarmVoices, MaxVoices);
            ApplySettings(pool);
            return pool;
        }
    }
}
