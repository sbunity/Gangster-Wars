using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Vibration;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Barricades
{
    [DisallowMultipleComponent]
    public sealed class BarricadeDamageVibrationFeedback : MonoBehaviour
    {
        [SerializeField] private VibrationConfig _vibration;

        private IVibrationService _vibrationService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(IVibrationService vibrationService, SignalBus signalBus)
        {
            _vibrationService = vibrationService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<BarricadeDamagedSignal>(OnBarricadeDamaged);
        }

        private void OnEnable()
        {
            _signals?.Enable();
        }

        private void OnDisable()
        {
            _signals?.Disable();
        }

        private void OnBarricadeDamaged()
        {
            _vibrationService?.Vibrate(_vibration);
        }
    }
}
