using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Projectiles
{
    public sealed class ProjectileMovement : MonoBehaviour
    {
        public void Move(Rigidbody2D body, Vector3 origin, Vector2 target, float speed)
        {
            if (body == null)
                return;
                
            body.linearVelocity = (new Vector3(target.x, target.y, origin.z) - origin).normalized * speed;
        }

        public void Rotate(Transform target, Vector3 point)
        {
            var currentPoint = transform.position;
            var direction = point - currentPoint;
            direction.Normalize();
            var rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }
}