using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Bonuses
{
    public sealed class BonusView : MonoBehaviour
    {
        public Collider2D CollisionCollider { get; private set; }

        public void Initialize()
        {
            CollisionCollider = GetComponent<Collider2D>();
        }
    }
}