using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class AdSave
    {
        [SerializeField] private double _forcedAdDisabledUntil;
        public double ForcedAdDisabledUntil { get => _forcedAdDisabledUntil; set => _forcedAdDisabledUntil = value; }

        [SerializeField] private bool _isFirstLaunchPassed;
        public bool IsFirstLaunchPassed { get => _isFirstLaunchPassed; set => _isFirstLaunchPassed = value; }
    }
}
