using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class ExplosionCollisionController : CollisionController
    {
        private Tween twn;
        [SerializeField, FormerlySerializedAs("timeParticle"), Range(0, 3)]

        private float _timeParticle;

        private void OnDisable()
        {
            twn?.Kill();
        }

        public override void Init(Vector3 _position, int _damage, float _radius, float _time = 2)
        {
            this.gameObject.SetActive(true);
            transform.position = _position;
            Damage = _damage;
            Radius = _radius;
            Time = _time;
            Action(Time);
        }

        private void Action(float _time)
        {
            twn = DOVirtual.DelayedCall(_timeParticle, () =>
            {
                Pop();
            });
        }
    }
}
