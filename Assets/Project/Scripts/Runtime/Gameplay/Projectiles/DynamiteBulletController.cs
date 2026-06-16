using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;
using SBabchuk.Runtime.Gameplay.Collisions;

namespace SBabchuk.Runtime.Gameplay.Projectiles
{
    public class DynamiteBulletController : BaseBulletController
    {
        public override void StartMove(Vector2 _target)
        {
            if (Rigidbody2D)
            {
                Rigidbody2D.linearVelocity = (new Vector3(_target.x, _target.y, transform.position.z) - transform.position).normalized * Random.Range(Properties.SpeedMove - 2, Properties.SpeedMove + 2);
            }
            else
            {
                Debug.LogWarning("Dynamite Rigidbody2D is missing.");
            }
        }

        public override CollisionController Pop()
        {
            var impact = base.Pop();
            LevelSpawnService?.SpawnCollision(4, transform.position);
            return impact;
        }

        void FixedUpdate()
        {
            Rigidbody2D.rotation += Random.Range(5, 8);
        }
    }
}
