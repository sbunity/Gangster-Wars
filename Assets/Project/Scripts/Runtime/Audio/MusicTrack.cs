using UnityEngine;

namespace SBabchuk.Runtime.Audio
{
    [CreateAssetMenu(menuName = "Audio/Create MusicTrack", fileName = "MusicTrack")]
    public sealed class MusicTrack : ScriptableObject
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Min(0f)] private float _fadeInDuration = 0.5f;
        [SerializeField, Min(0f)] private float _fadeOutDuration = 0.5f;

        public AudioClip Clip => _clip;
        public float Volume => _volume;
        public float FadeInDuration => _fadeInDuration;
        public float FadeOutDuration => _fadeOutDuration;
    }
}
