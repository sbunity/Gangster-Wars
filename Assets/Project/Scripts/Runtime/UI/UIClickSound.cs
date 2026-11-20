using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public enum UIClickSoundType
    {
        Click,
        Purchase,
        Supply
    }

    [DisallowMultipleComponent]
    public sealed class UIClickSound : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private UIClickSoundType _type = UIClickSoundType.Click;
        [SerializeField] private SoundConfig _customSound;

        private IAudioService _audioService;
        private UISoundsConfig _sounds;
        private Selectable _selectable;
        private bool _pressedWhileInteractable;

        public bool DeferSound { get; set; }

        [Inject]
        public void Construct(IAudioService audioService, UISoundsConfig sounds)
        {
            _audioService = audioService;
            _sounds = sounds;
        }

        public void PlayDeferredSound()
        {
            if (_sounds != null)
                Play(GetSound());
        }

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressedWhileInteractable = _selectable == null || _selectable.IsInteractable();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !_pressedWhileInteractable)
                return;

            _pressedWhileInteractable = false;

            if (_sounds != null)
                Play(DeferSound ? _sounds.Click : GetSound());
        }

        private void Play(SoundConfig sound)
        {
            if (_audioService != null)
                _audioService.Play(sound);
        }

        private SoundConfig GetSound()
        {
            if (_customSound != null)
                return _customSound;

            return _type switch
            {
                UIClickSoundType.Purchase => _sounds.Purchase,
                UIClickSoundType.Supply => _sounds.Supply,
                _ => _sounds.Click,
            };

        }
    }
}
