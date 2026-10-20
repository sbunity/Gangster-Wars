using System;
using UnityEngine;

namespace SBabchuk.Runtime.Bootstrap
{
    [Serializable]
    public sealed class BootstrapSettings
    {
        [SerializeField, Min(0f)]
        private float _minDuration = 5f;

        [SerializeField, Min(0f)]
        private float _overtimeTail = 1f;

        [SerializeField]
        private Scene _firstScene = Scene.MainScene;

        public TimeSpan MinDuration => TimeSpan.FromSeconds(_minDuration);
        public TimeSpan OvertimeTail => TimeSpan.FromSeconds(_overtimeTail);
        public Scene FirstScene => _firstScene;
    }
}
