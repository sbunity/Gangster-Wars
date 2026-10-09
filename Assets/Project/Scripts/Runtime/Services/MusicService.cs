using System;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SBabchuk.Runtime.Services
{
    public sealed class MusicService : IMusicService, IInitializable, IDisposable
    {
        private readonly IAudioSettingsService _audioSettings;
        private readonly SignalSubscriptions _signals;

        private GameObject _root;
        private AudioSource _source;
        private MusicTrack _current;
        private float _fadeOutDuration;
        private Sequence _fade;

        public MusicService(IAudioSettingsService audioSettings, SignalBus signalBus)
        {
            _audioSettings = audioSettings;
            _signals = new SignalSubscriptions(signalBus)
                .Add<AudioSettingsChangedSignal>(ApplySettings);
        }

        private AudioSource Source => _source != null ? _source : _source = CreateSource();

        public void Initialize()
        {
            _signals.Enable();
            ApplySettings();
        }

        public void Dispose()
        {
            _signals.Disable();
            _fade?.Kill();

            if (_root != null)
                Object.Destroy(_root);
        }

        public void Play(MusicTrack track)
        {
            if (track == null || track.Clip == null || track == _current)
                return;

            _current = track;

            var sequence = BeginFade();
            AppendFadeOut(sequence);
            sequence.AppendCallback(() => StartClip(track));
            sequence.Append(FadeTo(track.Volume, track.FadeInDuration));

            _fadeOutDuration = track.FadeOutDuration;
        }

        public void Stop()
        {
            if (_current == null)
                return;

            _current = null;

            var sequence = BeginFade();
            AppendFadeOut(sequence);
            sequence.AppendCallback(Source.Stop);
        }

        private Sequence BeginFade()
        {
            _fade?.Kill();
            _fade = DOTween.Sequence().SetUpdate(true);
            return _fade;
        }

        private void AppendFadeOut(Sequence sequence)
        {
            if (Source.isPlaying)
                sequence.Append(FadeTo(0f, _fadeOutDuration));
        }

        private void StartClip(MusicTrack track)
        {
            Source.clip = track.Clip;
            Source.volume = 0f;
            Source.Play();
        }

        private Tween FadeTo(float volume, float duration)
            => DOTween.To(() => Source.volume, value => Source.volume = value, volume, duration);

        private void ApplySettings()
        {
            Source.mute = !_audioSettings.IsMusicEnabled;
        }

        private AudioSource CreateSource()
        {
            _root = new GameObject("[Music]");
            Object.DontDestroyOnLoad(_root);

            var source = _root.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            source.volume = 0f;
            return source;
        }
    }
}
