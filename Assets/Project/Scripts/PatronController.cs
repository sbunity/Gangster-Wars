using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
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
        private SignalBus _signalBus;
        private bool _isSubscribed;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

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

        private void Subscribe()
        {
            if (_isSubscribed || _signalBus == null)
                return;

            _signalBus.Subscribe<LeaderMagazineInitializedSignal>(Init);
            _signalBus.Subscribe<LeaderPatronsChangedSignal>(Check);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _signalBus == null)
                return;
                
            _signalBus.Unsubscribe<LeaderMagazineInitializedSignal>(Init);
            _signalBus.Unsubscribe<LeaderPatronsChangedSignal>(Check);
            _isSubscribed = false;
        }
    }
}
