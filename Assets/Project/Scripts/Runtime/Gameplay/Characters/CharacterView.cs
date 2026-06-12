using UnityEngine;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class CharacterView : MonoBehaviour
    {
        public GangsterAnimationController Animation { get; private set; }
        public Center BulletPoint { get; private set; }
        public Center Center { get; private set; }

        public void Initialize()
        {
            Animation = GetComponentInChildren<GangsterAnimationController>();
            BulletPoint = GetComponentInChildren<Center>();
            Center = BulletPoint;
        }

        public void SetAnimation(AnimationsName animation)
        {
            if (Animation != null)
                Animation.SetAnimation(animation);
        }
    }
}
