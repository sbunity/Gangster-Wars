using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
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

        public override void Pop()
        {
            base.Pop();
            LevelRuntimeService?.SpawnCollision(4, transform.position);
        }

        void FixedUpdate()
        {
            Rigidbody2D.rotation += Random.Range(5, 8);
        }
    }
}
