using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Audio
{
    [DisallowMultipleComponent]
    public sealed class SceneMusic : MonoBehaviour
    {
        [SerializeField] private MusicTrack _track;

        private IMusicService _musicService;

        [Inject]
        public void Construct(IMusicService musicService)
        {
            _musicService = musicService;
        }

        private void Start()
        {
            _musicService?.Play(_track);
        }
    }
}
