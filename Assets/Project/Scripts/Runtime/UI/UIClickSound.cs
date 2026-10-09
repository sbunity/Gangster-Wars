using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    [DisallowMultipleComponent]
    public sealed class UIClickSound : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private IAudioService _audioService;
        private UISoundsConfig _sounds;
        private Selectable _selectable;
        private bool _pressedWhileInteractable;

        [Inject]
        public void Construct(IAudioService audioService, UISoundsConfig sounds)
        {
            _audioService = audioService;
            _sounds = sounds;
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

            if (_audioService != null && _sounds != null)
                _audioService.Play(_sounds.Click);
        }
    }
}
