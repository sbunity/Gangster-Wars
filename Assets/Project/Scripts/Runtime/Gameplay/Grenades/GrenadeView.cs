using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Grenades
{
    public sealed class GrenadeView : MonoBehaviour
    {
        public Collider2D CollisionCollider { get; private set; }

        public void Initialize()
        {
            CollisionCollider = GetComponent<Collider2D>();
        }
    }
}
