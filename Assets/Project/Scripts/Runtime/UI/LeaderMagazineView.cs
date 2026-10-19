using SBabchuk.Runtime.Architecture;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class LeaderMagazineView : MonoBehaviour
    {
        [SerializeField] private PatronController[] _patrons;

        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<LeaderMagazineInitializedSignal>(OnMagazineInitialized)
                .Add<LeaderPatronsChangedSignal>(OnPatronsChanged);
            _signals.Enable();
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void OnMagazineInitialized(LeaderMagazineInitializedSignal signal)
        {
            foreach (var patron in _patrons)
                patron.gameObject.SetActive(patron.Index <= signal.Capacity);
        }

        private void OnPatronsChanged(LeaderPatronsChangedSignal signal)
        {
            foreach (var patron in _patrons)
                patron.SetFilled(patron.Index <= signal.Count);
        }
    }
}
