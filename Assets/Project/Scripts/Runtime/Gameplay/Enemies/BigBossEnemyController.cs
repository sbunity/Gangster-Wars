using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BigBossEnemyController : EnemyControllerBase
    {
        [SerializeField, FormerlySerializedAs("mixDuration"), Range(0, 1)]
        private float _mixDuration = 0.2f;

        [SerializeField, FormerlySerializedAs("bulletPoint")]
        private Center _bulletPoint;

        private Tween _correctionMixDuration;

        public override void Attack()
        {
            IsAttacked = true;
            Animation.SetAnimation(AnimationsName.Attack);
            _correctionMixDuration = DOVirtual.DelayedCall(_mixDuration, () =>
            {
                _correctionMixDuration = DOVirtual.DelayedCall(0.1f, () =>
                {
                    GiveDamage();
                }).SetLoops(4);
            });
        }

        public override void Attacked()
        {
            WaitNextAttack(Properties.AttackSpeed);
        }

        public override void GiveDamage()
        {
            if (_bulletPoint == null)
                return;

            var bulletPosition = _bulletPoint.GetPosition();
            var offset = Vector2.Distance(Center.GetPosition(), bulletPosition);
            LevelSpawnService?.SpawnBullet(Properties.BulletId, Properties.Damage, bulletPosition, new Vector3(bulletPosition.x - 20, bulletPosition.y, 0), offset);
        }

        public override void StopAllTweens()
        {
            base.StopAllTweens();
            _correctionMixDuration?.Kill();
        }
    }
}
