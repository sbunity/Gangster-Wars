using UnityEngine;
using DG.Tweening;

namespace SBabchuk.Runtime.Gameplay.Collisions
{
    public class FireCollisionController : CollisionController
    {
        private const float DamageInterval = 1f;

        private Tween _twn;

        private void OnDisable()
        {
            _twn?.Kill();
        }

        public override void Init(Vector3 _position, int _damage, float _radius, float _time = 0)
        {
            base.Init(_position, _damage, _radius, _time);
            Action(Time);
        }

        public override void Pop()
        {
            Release();
        }

        private void Action(float time)
        {
            _twn?.Kill();

            if (time <= 0)
            {
                Pop();
                return;
            }

            FireDamage();

            var tickCount = Mathf.FloorToInt(time / DamageInterval);
            var sequence = DOTween.Sequence();

            for (var i = 0; i < tickCount; i++)
            {
                sequence.AppendInterval(DamageInterval);
                sequence.AppendCallback(FireDamage);
            }

            var remainingTime = Mathf.Max(0f, time - tickCount * DamageInterval);
            if (remainingTime > 0)
                sequence.AppendInterval(remainingTime);

            sequence.AppendCallback(Pop);
            _twn = sequence;
        }
    }
}
