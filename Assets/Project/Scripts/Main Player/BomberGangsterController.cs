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
        public static BomberGangsterController Instance;

        [HideInInspector]
        public Transform target;

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
            if (Instance == null)
                Instance = this;

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

            PersonageShortInfo _shortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetPersonageShortInfo((int)PersonagesName.Bomber);

            personage = PersistableSO.Instance.PlayerStore.GetPersonage((int)PersonagesName.Bomber);

            PUpgrade _upgrade = PersistableSO.Instance.PlayerStore.GetUpgrade((int)PersonagesName.Bomber, _shortInfo.upgradeID);

            if (_upgrade != null)
            {
                properties = _upgrade.settings;
            }
            else
            {
                properties = PersistableSO.Instance.PlayerStore.GetPersonage((int)PersonagesName.Bomber).settings;
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

        /// <summary>
        /// Метод атаки
        /// </summary>
        public override void Attack()
        {
            e_animation.SetAnimation(AnimationsName.Throwing); //Переключаємось в анімацію атаки
        }

        public override void SpawnBullet()
        {
            float offset = Vector2.Distance(center.GetPosition(), createBulletPoint.GetPosition());
            LevelController.Instance.SpawnBullet(personage.bulletID, properties.damage, createBulletPoint.GetPosition(), target.position, offset, "BulletHero");
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
