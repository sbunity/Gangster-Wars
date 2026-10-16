#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Vibration
{
    public sealed class AndroidHapticDevice : IHapticDevice, IDisposable
    {
        private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
        private const string BuildVersionClass = "android.os.Build$VERSION";
        private const string VibrationEffectClass = "android.os.VibrationEffect";
        private const string VibratorServiceName = "vibrator";
        private const int VibrationEffectMinSdk = 26;
        private const int NoRepeat = -1;
        private const int SilentAmplitude = 0;
        private const int MinAmplitude = 1;
        private const int MaxAmplitude = 255;

        private readonly Dictionary<VibrationConfig, Waveform> _waveforms = new();

        private AndroidJavaObject _vibrator;
        private AndroidJavaClass _vibrationEffect;
        private bool _hasAmplitudeControl;
        private bool _isInitialized;
        private bool _isSupported;

        public bool IsSupported
        {
            get
            {
                EnsureInitialized();
                return _isSupported;
            }
        }

        public void Play(VibrationConfig vibration)
        {
            if (!IsSupported)
                return;

            var waveform = GetWaveform(vibration);

            if (_vibrationEffect == null)
            {
                _vibrator.Call("vibrate", waveform.OnOffTimings, NoRepeat);
                return;
            }

            using var effect = _hasAmplitudeControl
                ? _vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", waveform.Timings, waveform.Amplitudes, NoRepeat)
                : _vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", waveform.OnOffTimings, NoRepeat);
            _vibrator.Call("vibrate", effect);
        }

        public void Cancel()
        {
            if (IsSupported)
                _vibrator.Call("cancel");
        }

        public void Dispose()
        {
            _vibrator?.Dispose();
            _vibrationEffect?.Dispose();
            _vibrator = null;
            _vibrationEffect = null;
        }

        private Waveform GetWaveform(VibrationConfig vibration)
        {
            if (!_waveforms.TryGetValue(vibration, out var waveform))
            {
                waveform = new Waveform(vibration.Pulses);
                _waveforms[vibration] = waveform;
            }

            return waveform;
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            try
            {
                using var unityPlayer = new AndroidJavaClass(UnityPlayerClass);
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", VibratorServiceName);
                _isSupported = _vibrator != null && _vibrator.Call<bool>("hasVibrator");

                if (!_isSupported)
                    return;

                using var buildVersion = new AndroidJavaClass(BuildVersionClass);
                if (buildVersion.GetStatic<int>("SDK_INT") < VibrationEffectMinSdk)
                    return;

                _vibrationEffect = new AndroidJavaClass(VibrationEffectClass);
                _hasAmplitudeControl = _vibrator.Call<bool>("hasAmplitudeControl");
            }
            catch (Exception exception)
            {
                _isSupported = false;
                Debug.LogWarning($"{nameof(AndroidHapticDevice)} is unavailable: {exception.Message}");
            }
        }

        private sealed class Waveform
        {
            public Waveform(IReadOnlyList<VibrationPulse> pulses)
            {
                var timings = new List<long>();
                var amplitudes = new List<int>();
                var onOffTimings = new List<long>();

                foreach (var pulse in pulses)
                {
                    if (pulse.DelayMs > 0)
                    {
                        timings.Add(pulse.DelayMs);
                        amplitudes.Add(SilentAmplitude);
                    }

                    timings.Add(pulse.DurationMs);
                    amplitudes.Add(ToAmplitude(pulse.Intensity));
                    onOffTimings.Add(pulse.DelayMs);
                    onOffTimings.Add(pulse.DurationMs);
                }

                Timings = timings.ToArray();
                Amplitudes = amplitudes.ToArray();
                OnOffTimings = onOffTimings.ToArray();
            }

            public long[] Timings { get; }
            public int[] Amplitudes { get; }
            public long[] OnOffTimings { get; }

            private int ToAmplitude(float intensity)
                => Mathf.Clamp(Mathf.RoundToInt(intensity * MaxAmplitude), MinAmplitude, MaxAmplitude);
        }
    }
}
#endif
