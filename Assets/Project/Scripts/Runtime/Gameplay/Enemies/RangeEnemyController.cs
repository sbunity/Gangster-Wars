using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SBabchuk
{
    public class RangeEnemyController : EnemyControllerBase
    {
        [Header("Shot delay")]
        [Range(0, 1)]
        public float mixDuration = 0.2f;

        [Header("Bullet spawn points")]
        public List<Center> bulletPoints;

        private int index = 0;

        private Tween correctionMixDuration;

        public override void Attacked()
        {
            index = 0;

            WaitNextAttack(properties.speedAtack);
        }

        public override void GiveDamage()
        {
            if (bulletPoints == null || bulletPoints.Count == 0)
                return;

            correctionMixDuration = DOVirtual.DelayedCall(mixDuration, () =>
            {
                Center bulletPoint = bulletPoints[index % bulletPoints.Count];
                Vector3 bulletPosition = bulletPoint.GetPosition();
                float offset = Vector2.Distance(center.GetPosition(), bulletPosition);
                if (LevelRuntimeService != null)
                    LevelRuntimeService.SpawnBullet(properties.bulletID, properties.damage, bulletPosition, new Vector3(bulletPosition.x - 20, bulletPosition.y, 0), offset);

                index++;
            });
        }

        public override void StopAllTweens()
        {
            base.StopAllTweens();

            correctionMixDuration?.Kill();
        }
    }
}
