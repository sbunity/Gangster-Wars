using System;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class ATTSettings
    {
        [SerializeField] private bool _isATTEnabled = true;
        public bool IsATTEnabled => _isATTEnabled;
    }
}
