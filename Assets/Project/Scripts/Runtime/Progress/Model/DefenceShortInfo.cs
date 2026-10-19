using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class DefenceShortInfo
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField] private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        public DefenceShortInfo(int id, bool isBought)
        {
            _id = id;
            _isBuy = isBought ? mySwitch.On : mySwitch.Off;
            _upgradeId = -1;
        }
    }
}
