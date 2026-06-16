using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class WeaponMagazine
    {
        public WeaponsName Weapon { get; }
        public int Capacity { get; }
        public int Current { get; private set; }

        public WeaponMagazine(WeaponsName weapon, int capacity, int current)
        {
            Weapon = weapon;
            Capacity = Mathf.Max(0, capacity);
            Current = Mathf.Clamp(current, 0, Capacity);
        }

        public bool IsEmpty => Current <= 0;
        public bool IsFull => Current >= Capacity;

        public bool TryConsume()
        {
            if (Current <= 0)
                return false;

            Current--;
            return true;
        }

        public bool TryLoadRound(int reserve)
        {
            if (Current >= Capacity || Current >= reserve)
                return false;

            Current++;
            return true;
        }
    }
}
