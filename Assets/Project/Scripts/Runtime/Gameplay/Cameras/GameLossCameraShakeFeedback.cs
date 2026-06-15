using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Cameras
{
    [DisallowMultipleComponent]
    public sealed class GameLossCameraShakeFeedback : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _shakeStrength = 0.18f;
        [SerializeField, Min(0.01f)] private float _shakeDuration = 0.35f;

        private ICameraShakeService _cameraShakeService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(ICameraShakeService cameraShakeService, SignalBus signalBus)
        {
            _cameraShakeService = cameraShakeService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        private void OnEnable()
        {
            _signals?.Enable();
        }

        private void OnDisable()
        {
            _signals?.Disable();
        }

        private void OnGameFinished(GameFinishedSignal signal)
        {
            if (signal.Panel != Panels.Lose)
                return;

            _cameraShakeService?.Shake(_shakeStrength, _shakeDuration);
        }
    }
}
