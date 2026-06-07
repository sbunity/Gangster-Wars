using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class ExplosionCollisionController : CollisionController
    {
        private Tween twn;

        [Header("Час життя")]
        [Range(0, 3)]
        public float timeParticle;

        private SortingEnemy sortingEnemy;

        /// <summary>
        /// Предастартова ініціалізація
        /// </summary>
        public override void Awake()
        {
            sortingEnemy = GetComponent<SortingEnemy>();
        }

        /// <summary>
        /// Стратова ініціалізація
        /// </summary>
        public override void Subscribe()
        {
        }

        private void OnDisable()
        {
            Utils.StopTween(twn);
        }

        public override void Init(Vector3 _position, int _damage, float _radius, float _time = 2)
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
            twn = DOVirtual.DelayedCall(timeParticle, () => {
                Pop();
            });
        }
    }
}
