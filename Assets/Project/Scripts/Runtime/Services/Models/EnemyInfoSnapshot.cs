using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Services.Models
{
    public sealed class EnemyInfoSnapshot
    {
        public EnemyInfoSnapshot(
            string name,
            string kind,
            string description,
            Sprite icon,
            IReadOnlyList<EnemyInfoStatSnapshot> stats)
        {
            Name = name;
            Kind = kind;
            Description = description;
            Icon = icon;
            Stats = stats;
        }

        public string Name { get; }
        public string Kind { get; }
        public string Description { get; }
        public Sprite Icon { get; }
        public IReadOnlyList<EnemyInfoStatSnapshot> Stats { get; }
    }

    public sealed class EnemyInfoStatSnapshot
    {
        public EnemyInfoStatSnapshot(string name, float value, float maxValue)
        {
            Name = name;
            Value = value;
            MaxValue = Mathf.Max(1f, maxValue);
        }

        public string Name { get; }
        public float Value { get; }
        public float MaxValue { get; }
        public float NormalizedValue => Mathf.Clamp01(Value / MaxValue);
    }
}
