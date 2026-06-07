using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace SBabchuk
{
    public class BigBossEnemyController : EnemyControllerBase
    {
        [Header("Поправка враховуючи мікс")]
        [Range(0, 1)]
        public float mixDuration = 0.2f;

        [Header("Центр спавна пуль")]
        public Center bulletPoint;

        Tween correctionMixDuration;

        /// <summary>
        /// Метод самої атаки
        /// </summary>
        public override void Attack()
        {
            isAttacked = true; //Зараз ми в стані атаки

            e_animation.SetAnimation(AnimationsName.Attack); //Переключаємось в анімацію атаки

            correctionMixDuration = DOVirtual.DelayedCall(mixDuration, () =>
            {
                correctionMixDuration = DOVirtual.DelayedCall(0.1f, ()=>{
                    GiveDamage();
                }).SetLoops(4);
            });
        }

        /// <summary>
        /// Запускаєм нову атаку
        /// </summary>
        public override void Attacked()
        {
            WaitNextAttack(properties.speedAtack);
        }

        /// <summary>
        /// Нанесення шкоди
        /// </summary>
        public override void GiveDamage()
        {
            float offset = Vector2.Distance(center.GetPosition(), bulletPoint.GetPosition());
            LevelController.Instance.SpawnBullet(properties.bulletID, properties.damage, bulletPoint.GetPosition(), new Vector3(bulletPoint.GetPosition().x - 20, bulletPoint.GetPosition().y, 0), offset);
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
