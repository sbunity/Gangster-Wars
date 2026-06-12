using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class PatronController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("index")]
        private int _index;

        [SerializeField, FormerlySerializedAs("fullSprite")]
        private Sprite _fullSprite;

        [SerializeField, FormerlySerializedAs("emptySprite")]
        private Sprite _emptySprite;

        private Image _ico;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<LeaderMagazineInitializedSignal>(Init)
                .Add<LeaderPatronsChangedSignal>(Check);
            _signals.Enable();
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void Awake()
        {
            _ico = GetComponentInChildren<Image>();
        }

        private void Init(LeaderMagazineInitializedSignal signal)
        {
            _ico.gameObject.SetActive(_index <= signal.Capacity);
        }

        private void Check(LeaderPatronsChangedSignal signal)
        {
            _ico.sprite = _index <= signal.Count ? _fullSprite : _emptySprite;
        }
    }
}
