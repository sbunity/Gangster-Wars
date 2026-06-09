using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace SBabchuk
{
    public class BomberBossEnemyController : EnemyControllerBase
    {
        [Header ("Поправка враховуючи мікс")]
        [Range(0,1)]
        public float mixDuration = 0.2f;

        [Header("Центр спавна пуль")]
        public List<Center> bulletPoints;

        private int index = 0;

        Tween correctionMixDuration;

        /// <summary>
        /// Запускаєм нову атаку
        /// </summary>
        public override void Attacked()
        {
            index = 0;

            WaitNextAttack(properties.speedAtack);
        }

        /// <summary>
        /// Нанесення шкоди
        /// </summary>
        public override void GiveDamage()
        {
            if (bulletPoints == null || bulletPoints.Count == 0)
                return;

            correctionMixDuration = DOVirtual.DelayedCall(mixDuration, () =>
            {
                Center bulletPoint = bulletPoints[index % bulletPoints.Count];
                Vector3 bulletPosition = bulletPoint.GetPosition();
                float offset = Vector2.Distance(center.GetPosition(), bulletPosition);
                LevelController.Instance.SpawnBullet(properties.bulletID, properties.damage, bulletPosition, target.position, offset);

                index++;

                bulletPoint = bulletPoints[index % bulletPoints.Count];
                bulletPosition = bulletPoint.GetPosition();
                offset = Vector2.Distance(center.GetPosition(), bulletPosition);
                LevelController.Instance.SpawnBullet(properties.bulletID, properties.damage, bulletPosition, target.position, offset);

            });
           
        }

        /// <summary>
        /// Зупинаяєм всі твіни
        /// </summary>
        public override void StopAllTweens()
        {
            base.StopAllTweens();

            Utils.StopTween(correctionMixDuration);
        }
    }
}
