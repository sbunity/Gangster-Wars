#if UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SBabchuk.Runtime.Vibration
{
    public sealed class IosHapticDevice : IHapticDevice
    {
        private const float MillisecondsPerSecond = 1000f;

        private readonly Dictionary<VibrationConfig, Pattern> _patterns = new();

        public bool IsSupported => true;

        public void Play(VibrationConfig vibration)
        {
            var pattern = GetPattern(vibration);
            PlayPattern(pattern.StartTimes, pattern.Durations, pattern.Intensities, pattern.Sharpnesses, pattern.Count);
        }

        public void Cancel()
        {
            StopPattern();
        }

        private Pattern GetPattern(VibrationConfig vibration)
        {
            if (!_patterns.TryGetValue(vibration, out var pattern))
            {
                pattern = new Pattern(vibration.Pulses);
                _patterns[vibration] = pattern;
            }

            return pattern;
        }

        [DllImport("__Internal", EntryPoint = "_GangsterWarsPlayHapticPattern")]
        private static extern void PlayPattern(float[] startTimes, float[] durations, float[] intensities, float[] sharpnesses, int count);

        [DllImport("__Internal", EntryPoint = "_GangsterWarsStopHaptics")]
        private static extern void StopPattern();

        private sealed class Pattern
        {
            public Pattern(IReadOnlyList<VibrationPulse> pulses)
            {
                Count = pulses.Count;
                StartTimes = new float[Count];
                Durations = new float[Count];
                Intensities = new float[Count];
                Sharpnesses = new float[Count];

                var time = 0f;
                for (var i = 0; i < Count; i++)
                {
                    var pulse = pulses[i];
                    time += pulse.DelayMs / MillisecondsPerSecond;
                    StartTimes[i] = time;
                    Durations[i] = pulse.DurationMs / MillisecondsPerSecond;
                    Intensities[i] = pulse.Intensity;
                    Sharpnesses[i] = pulse.Sharpness;
                    time += Durations[i];
                }
            }

            public int Count { get; }
            public float[] StartTimes { get; }
            public float[] Durations { get; }
            public float[] Intensities { get; }
            public float[] Sharpnesses { get; }
        }
    }
}
#endif
