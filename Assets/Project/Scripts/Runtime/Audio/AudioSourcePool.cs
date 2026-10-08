using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Audio
{
    public sealed class AudioSourcePool
    {
        private readonly Transform _root;
        private readonly int _capacity;
        private readonly List<Voice> _voices = new();
        private bool _muted;

        public AudioSourcePool(Transform root, int prewarmCount, int capacity)
        {
            _root = root;
            _capacity = Mathf.Max(1, capacity);

            var count = Mathf.Min(prewarmCount, _capacity);
            for (var i = 0; i < count; i++)
                _voices.Add(CreateVoice());
        }

        public void Play(SoundConfig sound, AudioClip clip)
        {
            Acquire(sound).Play(sound, clip);
        }

        public void SetMuted(bool muted)
        {
            _muted = muted;
            foreach (var voice in _voices)
                voice.SetMuted(muted);
        }

        private Voice Acquire(SoundConfig sound)
        {
            Voice free = null;
            Voice oldest = null;
            Voice oldestSameSound = null;
            var sameSoundCount = 0;

            foreach (var voice in _voices)
            {
                if (!voice.IsPlaying)
                {
                    free ??= voice;
                    continue;
                }

                if (voice.Sound == sound)
                {
                    sameSoundCount++;
                    if (oldestSameSound == null || voice.StartTime < oldestSameSound.StartTime)
                        oldestSameSound = voice;
                }

                if (oldest == null || voice.StartTime < oldest.StartTime)
                    oldest = voice;
            }

            if (sameSoundCount >= sound.MaxInstances)
                return oldestSameSound;

            if (free != null)
                return free;

            if (_voices.Count < _capacity)
            {
                var voice = CreateVoice();
                _voices.Add(voice);
                return voice;
            }

            return oldest;
        }

        private Voice CreateVoice()
        {
            var gameObject = new GameObject("Voice_" + _voices.Count);
            gameObject.transform.SetParent(_root, false);

            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.mute = _muted;

            return new Voice(source);
        }

        private sealed class Voice
        {
            private readonly AudioSource _source;

            public Voice(AudioSource source)
            {
                _source = source;
            }

            public SoundConfig Sound { get; private set; }
            public float StartTime { get; private set; }
            public bool IsPlaying => _source.isPlaying;

            public void Play(SoundConfig sound, AudioClip clip)
            {
                Sound = sound;
                StartTime = Time.unscaledTime;

                _source.Stop();
                _source.clip = clip;
                _source.volume = sound.Volume;
                _source.pitch = sound.GetRandomPitch();
                _source.Play();
            }

            public void SetMuted(bool muted)
            {
                _source.mute = muted;
            }
        }
    }
}
