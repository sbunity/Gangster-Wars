using System;
using UnityEngine;

namespace SBabchuk.Runtime.Vibration
{
    [Serializable]
    public sealed class VibrationPulse
    {
        [SerializeField, Min(0)] private int _delayMs;
        [SerializeField, Min(1)] private int _durationMs = 40;
        [SerializeField, Range(0f, 1f)] private float _intensity = 0.7f;
        [SerializeField, Range(0f, 1f)] private float _sharpness = 0.5f;

        public int DelayMs => _delayMs;
        public int DurationMs => _durationMs;
        public float Intensity => _intensity;
        public float Sharpness => _sharpness;
    }
}
