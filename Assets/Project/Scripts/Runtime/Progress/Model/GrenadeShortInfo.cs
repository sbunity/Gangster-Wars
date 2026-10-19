using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class GrenadeShortInfo
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField] private int _count;
        public int Count { get => _count; set => _count = value; }

        public GrenadeShortInfo(int id)
        {
            _id = id;
            _isBuy = mySwitch.Off;
            _count = 0;
        }
    }
}
