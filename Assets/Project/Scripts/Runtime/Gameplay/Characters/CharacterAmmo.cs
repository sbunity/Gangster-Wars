using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class CharacterAmmo : MonoBehaviour
    {
        public int Current { get; private set; }
        public int Magazine { get; private set; }

        public void Initialize(int current, int magazine)
        {
            Current = Mathf.Clamp(current, 0, magazine);
            Magazine = Mathf.Max(0, magazine);
        }

        public void Add(int value)
        {
            Current = Mathf.Clamp(Current + value, 0, Magazine);
        }
    }
}