using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class PatronController : MonoBehaviour
    {
        [Header("Індекс патрона")]
        public int index;

        [Header("Спрайд повного патрона")]
        public Sprite fullSprite;

        [Header("Спрайд порожнього патрона")]
        public Sprite emptySprite;

        private Image _ico;
        private SignalBus _signalBus;
        private bool _isSubscribed;

        [Inject]
        private void Construct(SignalBus signalBus)
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
            _ico.gameObject.SetActive(index <= signal.Capacity);
        }

        private void Check(LeaderPatronsChangedSignal signal)
        {
            _ico.sprite = index <= signal.Count ? fullSprite : emptySprite;
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
