using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class WeaponShortInfo
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private string _name;
        public string Name { get => _name; set => _name = value; }

        [SerializeField] private mySwitch _isBuy;
        public mySwitch IsBuy { get => _isBuy; set => _isBuy = value; }

        [SerializeField] private int _upgradeId;
        public int UpgradeId { get => _upgradeId; set => _upgradeId = value; }

        [SerializeField] private int _ammoCount;
        public int AmmoCount { get => _ammoCount; set => _ammoCount = value; }

        public WeaponShortInfo(int id, string name, bool isBought, int ammoCount)
        {
            _id = id;
            _name = name;
            _isBuy = isBought ? mySwitch.On : mySwitch.Off;
            _upgradeId = -1;
            _ammoCount = ammoCount;
        }
    }
}
