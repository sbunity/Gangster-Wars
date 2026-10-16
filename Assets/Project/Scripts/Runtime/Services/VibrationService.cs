using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Vibration;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class VibrationService : IVibrationService
    {
        private const float MillisecondsPerSecond = 1000f;

        private readonly IVibrationSettingsService _settings;
        private readonly IHapticDevice _device;

        private float _busyUntil;
        private int _currentPriority;

        public VibrationService(IVibrationSettingsService settings, IHapticDevice device)
        {
            _settings = settings;
            _device = device;
        }

        public void Vibrate(VibrationConfig vibration)
        {
            if (vibration == null || vibration.IsEmpty || !_settings.IsVibrationEnabled || !_device.IsSupported)
                return;

            var now = Time.unscaledTime;
            var isBusy = now < _busyUntil;
            if (isBusy && vibration.Priority <= _currentPriority)
                return;

            if (isBusy)
                _device.Cancel();

            _busyUntil = now + vibration.TotalDurationMs / MillisecondsPerSecond;
            _currentPriority = vibration.Priority;
            _device.Play(vibration);
        }
    }
}
