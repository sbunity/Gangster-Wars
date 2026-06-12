using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Collisions
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
            base.Init(_position, _damage, _radius, _time);
            FireDamage();
            ScheduleRelease();
        }

        // Damage is dealt the instant the explosion appears (in Init); the particle stays on
        // screen for _timeParticle, after which the object returns to the pool. Pop is
        // overridden to release only, so it never deals damage a second time.
        public override void Pop()
        {
            Release();
        }

        private void ScheduleRelease()
        {
            twn?.Kill();
            twn = DOVirtual.DelayedCall(_timeParticle, Release);
        }
    }
}
