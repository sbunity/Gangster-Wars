using UnityEngine;
using DG.Tweening;

namespace SBabchuk
{
    public class BigBossEnemyController : EnemyControllerBase
    {
        [Header("Shot delay")]
        [Range(0, 1)]
        public float mixDuration = 0.2f;

        [Header("Bullet spawn point")]
        public Center bulletPoint;

        private Tween correctionMixDuration;

        public override void Attack()
        {
            isAttacked = true;

            e_animation.SetAnimation(AnimationsName.Attack);

            correctionMixDuration = DOVirtual.DelayedCall(mixDuration, () =>
            {
                correctionMixDuration = DOVirtual.DelayedCall(0.1f, () =>
                {
                    GiveDamage();
                }).SetLoops(4);
            });
        }

        public override void Attacked()
        {
            WaitNextAttack(properties.speedAtack);
        }

        public override void GiveDamage()
        {
            if (bulletPoint == null)
                return;

            Vector3 bulletPosition = bulletPoint.GetPosition();
            float offset = Vector2.Distance(center.GetPosition(), bulletPosition);
            if (LevelRuntimeService != null)
                LevelRuntimeService.SpawnBullet(properties.bulletID, properties.damage, bulletPosition, new Vector3(bulletPosition.x - 20, bulletPosition.y, 0), offset);
        }

        public override void StopAllTweens()
        {
            base.StopAllTweens();

            correctionMixDuration?.Kill();
        }
    }
}
