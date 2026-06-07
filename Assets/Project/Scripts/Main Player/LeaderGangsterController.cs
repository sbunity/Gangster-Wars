using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
{ 
    [System.Serializable]
    public class CreateBulletPoints
    {
        [Header("Точки")]
        public List<Center> points;
    }

    public class LeaderGangsterController : GangsterControllerBase
    {
        public delegate void Event(int _value);
        public static event Event OnUpdateCountPatrons;
        public static event Event OnInitMagazine;
        public static event Event OnInitPatrons;

        public static LeaderGangsterController Instance;

        private WeaponShortInfo weaponShortInfo;

        private Weapon weapon;

        private WeaponSettings properties;

        private int countPatrons;

        private Tween twnReload;

        private Tween twnReloadPause;

        WeaponsName weaponsName;

        [Header("Точка де буде створена пуля")]
        [HideInInspector]
        public List<Center> createBulletPointList;

        public List<CreateBulletPoints> bulletPoints;

        private int index;

        private void OnDestroy()
        {
            Utils.StopTween(twnReload);
        }

        /// <summary>
        /// Передстартова ініціалізація
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
                Instance = this;

            base.Awake();

            //Time.timeScale = 0.075f;
        }

        /// <summary>
        /// Старт
        /// </summary>
        public override void Start()
        {
            //InitWeapon(2);
        }

        public void InitWeapon(int _weapoID)
        {
            weaponsName = (WeaponsName)_weapoID;

            weaponShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo(_weapoID);

            weapon = PersistableSO.Instance.WeaponStore.GetWeapon(_weapoID);

            WUpgrade upgrade = PersistableSO.Instance.WeaponStore.GetUpgrade(_weapoID, weaponShortInfo.upgradeID);

            properties = (upgrade != null)? upgrade.settings : weapon.settings;

            Init(); //Ініціалізація героя

            countPatrons = weaponShortInfo.countPatrons; //отримуєм кількість патрон

            Debug.Log("countPatrons: "+ countPatrons);

            if (countPatrons >= weapon.magazine) //передаєм не більше чим є в магазині патрон
                countPatrons = weapon.magazine;

            OnInitMagazine(weapon.magazine);

            OnInitPatrons(countPatrons);

            createBulletPointList = bulletPoints[_weapoID].points;

            index = 0;
        }

        /// <summary>
        /// Нанесення шкоди
        /// </summary>
        public override void SpawnBullet()
        {
            //float offset = Vector2.Distance(center.GetPosition(), bulletPoints[index].GetPosition());
            LevelController.Instance.SpawnBullet(weapon.bulletID, properties.damage,createBulletPointList[index].GetPosition(),  default(Vector3), 0, "BulletHero");

            index = (index + 1 < createBulletPointList.Count) ? index + 1 : 0;

            if (weaponsName != WeaponsName.Weapon_1)
                PersistableSO.Instance.PlayerPrefs.SetPatrons(weaponsName, -1);

            //return;

            UpdatePatrons(-1);
        }

        /// <summary>
        /// Оновлення патрон
        /// </summary>
        /// <param name="_value"></param>
        private void UpdatePatrons(int _value)
        {
            countPatrons += _value;

            OnUpdateCountPatrons?.Invoke(countPatrons);

            if (countPatrons == 0)
            {
                StopAttack();
            }
        }

        /// <summary>
        /// Метод атаки
        /// </summary>
        public override void Attack()
        {
            if (countPatrons > 0)
            {
                Utils.StopTween(twnReload);
                e_animation.SetAnimation(AnimationsName.Shoot); //Переключаємось в анімацію атаки
            }
        }

        /// <summary>
        /// Перезарядка
        /// </summary>
        public void Reload()
        {
            Utils.StopTween(twnReload);

            twnReload = DOVirtual.DelayedCall(1f, () =>
            {
                 twnReload = DOVirtual.DelayedCall(PersistableSO.Instance.WeaponStore.GetWeapon(weaponShortInfo.id).speedReload, () => 
                 {
                     if (countPatrons < weapon.magazine)
                     {
                         if (e_animation.GetCurrentAnimation() != AnimationsName.Reload)
                         {
                             e_animation.SetAnimation(AnimationsName.Reload); //Переключаємось в анімацію перезарядки
                         }

                         if (countPatrons < weaponShortInfo.countPatrons) //збільшуєм на стільки скільки ми ще можемо взяти з резерва
                         {
                             UpdatePatrons(1);
                         }
                         else
                         {
                             if (e_animation.GetCurrentAnimation() != AnimationsName.Idle)
                                 e_animation.SetAnimation(AnimationsName.Idle); //Переключаємось в анімацію спокою

                             Utils.StopTween(twnReload);
                         }

                     }
                     else
                     {
                         if (e_animation.GetCurrentAnimation() != AnimationsName.Idle)
                             e_animation.SetAnimation(AnimationsName.Idle); //Переключаємось в анімацію спокою

                         Utils.StopTween(twnReload);
                     }
                 }).SetLoops(-1);
             });
        }

        /// <summary>
        /// Зупиняєм атаку
        /// </summary>
        public void StopAttack()
        {
            e_animation.SetAnimation(AnimationsName.Idle);

            index = 0;

            Reload();
        }
    }

}
