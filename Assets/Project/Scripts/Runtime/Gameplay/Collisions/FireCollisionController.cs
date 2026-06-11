using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class FireCollisionController : CollisionController
    {
        private Tween twn;

        /// <summary>
        /// Предастартова ініціалізація
        /// </summary>
        public override void Awake()
        {
        }

        /// <summary>
        /// Стратова ініціалізація
        /// </summary>
        public override void Subscribe()
        {
        }

        private void OnDisable()
        {
            twn?.Kill();
        }

        public override void Init(Vector3 _position, int _damage, float _radius, float _time = 0)
        {
            this.gameObject.SetActive(true);

            transform.position = _position;

            damage = _damage;

            radius = _radius;

            time = _time;

            Action(time);
        }

        private void Action(float _time)
        {
            twn = DOVirtual.DelayedCall(time, () => {
                Pop();
            });
        }
    }
}
