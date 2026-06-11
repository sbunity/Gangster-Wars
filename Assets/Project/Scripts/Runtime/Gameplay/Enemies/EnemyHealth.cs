using System;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyHealth : MonoBehaviour
    {
        public event Action<float> Changed;
        public event Action Died;

        public int Current { get; private set; }
        public int Max { get; private set; }
        public bool IsDead { get; private set; }

        public void Initialize(int health)
        {
            Max = Mathf.Max(1, health);
            Current = Max;
            IsDead = false;
            Changed?.Invoke(1f);
        }

        public void ApplyDamage(int damage)
        {
            if (IsDead)
                return;

            Current = Mathf.Max(0, Current - Mathf.Max(0, damage));
            Changed?.Invoke((float)Current / Max);

            if (Current > 0)
                return;

            IsDead = true;
            Died?.Invoke();
        }
    }
}
