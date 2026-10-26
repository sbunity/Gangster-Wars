using System;

namespace SBabchuk.Runtime.Services.Models
{
    public readonly struct TutorialTargetKey : IEquatable<TutorialTargetKey>
    {
        public TutorialTargetKey(TutorialTargetId id, int index = 0)
        {
            Id = id;
            Index = index;
        }

        public TutorialTargetId Id { get; }
        public int Index { get; }

        public static implicit operator TutorialTargetKey(TutorialTargetId id)
            => new(id);

        public static TutorialTargetKey WeaponSlot(WeaponsName weapon)
            => new(TutorialTargetId.WeaponSlot, (int)weapon);

        public static TutorialTargetKey GrenadeSlot(GrenadesName grenade)
            => new(TutorialTargetId.GrenadeSlot, (int)grenade);

        public bool Equals(TutorialTargetKey other)
            => Id == other.Id && Index == other.Index;

        public override bool Equals(object obj)
            => obj is TutorialTargetKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Id * 397) ^ Index;
            }
        }
    }
}
