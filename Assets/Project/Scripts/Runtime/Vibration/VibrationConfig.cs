using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Vibration
{
    [CreateAssetMenu(menuName = "Vibration/Create VibrationConfig", fileName = "VibrationConfig")]
    public sealed class VibrationConfig : ScriptableObject
    {
        [SerializeField] private VibrationPulse[] _pulses = { new() };
        [SerializeField, Min(0)] private int _priority;

        public IReadOnlyList<VibrationPulse> Pulses => _pulses;
        public int Priority => _priority;
        public bool IsEmpty => _pulses == null || _pulses.Length == 0;

        public int TotalDurationMs
        {
            get
            {
                var total = 0;
                if (_pulses == null)
                    return total;

                foreach (var pulse in _pulses)
                    total += pulse.DelayMs + pulse.DurationMs;

                return total;
            }
        }
    }
}
