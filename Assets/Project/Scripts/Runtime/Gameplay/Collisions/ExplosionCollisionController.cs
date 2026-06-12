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
