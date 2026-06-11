using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
{
    public class BomberGangsterController : GangsterControllerBase
    {
        [HideInInspector]
        public Transform target;

        private Vector3 lastTargetPosition;

        private bool hasLastTargetPosition;

        [Header("Властивості")]
        private PersonageSettings properties;

        private Personage personage;

        [Header("Центр персонажа")]
        [HideInInspector]
        public Center center;

        /// Передстартова ініціалізація
        /// </summary>
        public override void Awake()
        {
            center = GetComponentInChildren<Center>();

            base.Awake();
        }

        /// <summary>
        /// Ініціалізація героя
        /// </summary>
        /// <param name="_id">Id героя</param>
        public override void Init()
        {
            e_animation.SetAnimation(AnimationsName.Idle);

            PersonageShortInfo _shortInfo = ProgressService.GetPersonageShortInfo((int)PersonagesName.Bomber);

            var playerStore = AssetProvider.MainPlayerDatabase;

            personage = playerStore.GetPersonage((int)PersonagesName.Bomber);

            PUpgrade _upgrade = playerStore.GetUpgrade((int)PersonagesName.Bomber, _shortInfo.upgradeID);

            if (_upgrade != null)
            {
                properties = _upgrade.settings;
            }
            else
            {
                properties = playerStore.GetPersonage((int)PersonagesName.Bomber).settings;
            }

            StartAttack();
        }

        public void StartAttack()
        {
            twnAtack = DOVirtual.DelayedCall(properties.speedAtack, () => { //Запускаєм зациклення атаки
                if(FindEnemy())
                    Attack(); //Атакуєм
            }, false).SetLoops(-1);
        }

        public override void Update()
        {
            base.Update();

            UpdateLastTargetPosition();
        }

        /// <summary>
        /// Метод атаки
        /// </summary>
        public override void Attack()
        {
            UpdateLastTargetPosition();

            e_animation.SetAnimation(AnimationsName.Throwing); //Переключаємось в анімацію атаки
        }

        public override void SpawnBullet()
        {
            float offset = Vector2.Distance(center.GetPosition(), createBulletPoint.GetPosition());
            CharacterWeapon.Fire(personage.bulletID, properties.damage, createBulletPoint.GetPosition(), GetTargetPosition(), offset);
        }

        public bool FindEnemy()
        {
            if (target != null && target.gameObject.activeInHierarchy)
            {
                UpdateLastTargetPosition();
                return true;
            }

            target = LevelRuntimeService != null ? LevelRuntimeService.GetRandomEnemy() : null;

            UpdateLastTargetPosition();

            return target != null;
        }

        private void UpdateLastTargetPosition()
        {
            if (target == null)
                return;

            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return;
            }

            lastTargetPosition = target.position;
            hasLastTargetPosition = true;
        }

        private Vector3 GetTargetPosition()
        {
            UpdateLastTargetPosition();

            if (hasLastTargetPosition)
                return lastTargetPosition;

            return target != null ? target.position : createBulletPoint.GetPosition();
        }
    }
}
