using UnityEngine;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Gameplay.Projectiles
{
    public sealed class ProjectileView : MonoBehaviour
    {
        public Rigidbody2D Rigidbody { get; private set; }
        public Collider2D CollisionCollider { get; private set; }
        public SortingEnemy Sorting { get; private set; }

        public void Initialize()
        {
            Rigidbody = GetComponentInChildren<Rigidbody2D>();
            CollisionCollider = GetComponent<Collider2D>();
            Sorting = GetComponent<SortingEnemy>();
        }
    }
}
