using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyView : MonoBehaviour
    {
        public EnemyAnimationControllerBase Animation { get; private set; }
        public Collider2D CollisionCollider { get; private set; }
        public Center Center { get; private set; }
        public Transform Transform => transform;

        public void Initialize()
        {
            Animation = GetComponent<EnemyAnimationControllerBase>();
            CollisionCollider = GetComponent<Collider2D>();
            Center = GetComponentInChildren<Center>();
        }

        public void SetColliderEnabled(bool value)
        {
            if (CollisionCollider == null)
                CollisionCollider = GetComponent<Collider2D>();

            if (CollisionCollider == null)
                return;
                
            CollisionCollider.enabled = value;
            CollisionCollider.isTrigger = value;
        }

        public void SetAnimation(AnimationsName animation)
        {
            if (Animation != null)
                Animation.SetAnimation(animation);
        }
    }
}