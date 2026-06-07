using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
{
    public class SniperGangsterController : GangsterControllerBase
    {
        public static SniperGangsterController Instance;

        [Header("Aim - кострейнт")]
        public Transform aim;

        [Header("Швидкість поворота для стрильби")]
        [Range(0, 2)]
        public float speedRotate;

        [HideInInspector]
        public Transform target;

        [Header("Властивості")]
        private PersonageSettings properties;

        private Personage personage;

        private Vector3 aimDefaultPosition;

        /// Передстартова ініціалізація
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
                Instance = this;

            base.Awake();
        }

        /// <summary>
        /// Ініціалізація героя
        /// </summary>
        /// <param name="_id">Id героя</param>
        public override void Init()
        {
            e_animation.SetAnimation(AnimationsName.Idle);

            PersonageShortInfo _shortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetPersonageShortInfo((int)PersonagesName.Sniper);

            personage = PersistableSO.Instance.PlayerStore.GetPersonage((int)PersonagesName.Sniper);

            PUpgrade _upgrade = PersistableSO.Instance.PlayerStore.GetUpgrade((int)PersonagesName.Sniper, _shortInfo.upgradeID);

            if (_upgrade != null)
            {
                properties = _upgrade.settings;
            }
            else
            {
                properties = PersistableSO.Instance.PlayerStore.GetPersonage((int)PersonagesName.Sniper).settings;
            }

            aimDefaultPosition = aim.position;

            StartAttack();
        }

        public void StartAttack()
        {
            twnAtack = DOVirtual.DelayedCall(properties.speedAtack + speedRotate*2, () => { //Запускаєм зациклення атаки
                if (FindEnemy())
                {
                    aim.DOMove(target.position, speedRotate).OnComplete(()=>
                    {
                        Attack(); //Атакуєм
                    });
                }
            }, false).SetLoops(-1);
        }

        /// <summary>
        /// Метод атаки
        /// </summary>
        public override void Attack()
        {
            e_animation.SetAnimation(AnimationsName.Shoot_prev); //Переключаємось в анімацію атаки
        }

        public override void SpawnBullet()
        {
            //float offset = Vector2.Distance(center.GetPosition(), bulletPoints[index].GetPosition());
            LevelController.Instance.SpawnBullet(personage.bulletID, properties.damage, createBulletPoint.GetPosition(), target.position, 0, "BulletHero");
        }

        public override void AttackEnded()
        {
            aim.DOMove(aimDefaultPosition, speedRotate);
        }

        public bool FindEnemy()
        {
            target = LevelController.Instance.GetRandomEnemy();

            return target != null;
        }
    }
}
