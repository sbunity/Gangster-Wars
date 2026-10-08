using UnityEngine;

namespace SBabchuk.Runtime.Audio
{
    [CreateAssetMenu(menuName = "Audio/Create SoundConfig", fileName = "SoundConfig")]
    public sealed class SoundConfig : ScriptableObject
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] private Vector2 _pitchRange = new(0.95f, 1.05f);
        [SerializeField, Min(1)] private int _maxInstances = 4;
        [SerializeField, Min(0f)] private float _minInterval;

        public float Volume => _volume;
        public int MaxInstances => _maxInstances;
        public float MinInterval => _minInterval;

        public AudioClip GetRandomClip()
            => _clips == null || _clips.Length == 0 ? null : _clips[Random.Range(0, _clips.Length)];

        public float GetRandomPitch()
            => Random.Range(_pitchRange.x, _pitchRange.y);
    }
}
