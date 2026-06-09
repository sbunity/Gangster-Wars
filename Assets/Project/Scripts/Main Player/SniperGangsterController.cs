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

        [Header("Поріг відстані aim до цілі для пострілу")]
        [Range(0.01f, 1f)]
        public float aimFireThreshold = 0.15f;

        [Header("Властивості")]
        private PersonageSettings properties;

        private Personage personage;

        private Vector3 aimDefaultPosition;

        private bool readyToFire;

        private float aimTimer;

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
            twnAtack = DOVirtual.DelayedCall(properties.speedAtack, () => { //Запускаєм зациклення атаки
                if (FindEnemy())
                    readyToFire = true;
            }, false).SetLoops(-1);
        }

        public override void Update()
        {
            base.Update();

            Vector3 dest = (target != null && target.gameObject.activeInHierarchy)
                ? target.position
                : aimDefaultPosition;

            aim.position = Vector3.Lerp(aim.position, dest, Time.deltaTime / Mathf.Max(speedRotate, 0.01f));

            if (readyToFire)
            {
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    aimTimer += Time.deltaTime;
                    if (aimTimer >= speedRotate)
                    {
                        readyToFire = false;
                        aimTimer = 0f;
                        Attack();
                    }
                }
                else
                {
                    readyToFire = false;
                    aimTimer = 0f;
                }
            }
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
        }

        public bool FindEnemy()
        {
            if (target != null && target.gameObject.activeInHierarchy)
                return true;

            target = LevelController.Instance.GetRandomEnemy();

            return target != null;
        }
    }
}
