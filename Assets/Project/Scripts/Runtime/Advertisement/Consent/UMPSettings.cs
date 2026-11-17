using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class UMPSettings
    {
        [SerializeField] private bool _isUMPEnabled = true;
        public bool IsUMPEnabled => _isUMPEnabled;

        [Tooltip("Set TagForUnderAgeOfConsent (TFUA) to indicate whether a user is under the age of consent. Consent is not requested from the user when TFUA is set to true. Mixed audience apps should set this parameter for child users to ensure consent is not requested.")]
        [SerializeField] private bool _umpTagForUnderAgeOfConsent;
        public bool UMPTagForUnderAgeOfConsent => _umpTagForUnderAgeOfConsent;

        [Space]
        [SerializeField] private bool _umpDebugMode;
        public bool UMPDebugMode => _umpDebugMode;

        [SerializeField] private DebugGeography _umpDebugGeography;
        public DebugGeography UMPDebugGeography => _umpDebugGeography;

        [SerializeField] private List<string> _testDevices;
        public List<string> TestDevices => _testDevices;

        public enum DebugGeography
        {
            Disabled = 0,
            EEA = 1,
            NotEEA = 2
        }
    }
}
