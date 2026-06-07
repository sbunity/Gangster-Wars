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
        /// <summary>
        /// Запускаєм рух
        /// </summary>
        public override void StartMove(Vector2 _target)
        {
            if (rigidbody2d)
            {
                rigidbody2d.linearVelocity = (new Vector3(_target.x, _target.y, transform.position.z) - transform.position).normalized * Random.Range(properties.speedMove - 2, properties.speedMove + 2);
            }
            else
            {
                Debug.Log("rigidbody2d == null");
            }
        }

        /// <summary>
		/// Повернення в пуш
		/// </summary>
		public override void Pop()
        {
            base.Pop();

            LevelController.Instance.SpawnCollision(4, this.transform.position);
        }

        void FixedUpdate()
        {
            rigidbody2d.rotation += Random.Range(5, 8);
        }
    }
}
