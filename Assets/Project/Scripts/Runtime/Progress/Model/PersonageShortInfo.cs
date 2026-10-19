using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class PersonageShortInfo
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField] private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        public PersonageShortInfo(int id)
        {
            _id = id;
            _isBuy = mySwitch.Off;
            _upgradeId = 0;
        }
    }
}
