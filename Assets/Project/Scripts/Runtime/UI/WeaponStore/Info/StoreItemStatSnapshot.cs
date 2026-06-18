using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.UI.WeaponStore.Info
{
    public sealed class StoreItemStatsSnapshot
    {
        public StoreItemStatsSnapshot(string title, Sprite icon, IReadOnlyList<StoreStatSnapshot> stats)
        {
            Title = title;
            Icon = icon;
            Stats = stats;
        }

        public string Title { get; }
        public Sprite Icon { get; }
        public IReadOnlyList<StoreStatSnapshot> Stats { get; }
    }

    public sealed class StoreStatSnapshot
    {
        public StoreStatSnapshot(string name, float currentValue, float upgradedValue, float maxValue)
        {
            Name = name;
            CurrentValue = currentValue;
            UpgradedValue = upgradedValue;
            MaxValue = Mathf.Max(1f, maxValue);
        }

        public string Name { get; }
        public float CurrentValue { get; }
        public float UpgradedValue { get; }
        public float MaxValue { get; }
        public float UpgradeDelta => Mathf.Max(0f, UpgradedValue - CurrentValue);
        public bool HasUpgradeDelta => UpgradeDelta > 0.001f;
    }
}
