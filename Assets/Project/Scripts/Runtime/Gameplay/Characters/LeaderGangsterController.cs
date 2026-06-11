using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{ 
    [System.Serializable]
    public class CreateBulletPoints
    {
        [Header("�����")]
        public List<Center> points;
    }

    public class LeaderGangsterController : GangsterControllerBase, ILeaderWeaponController
    {
        private WeaponShortInfo weaponShortInfo;

        private Weapon weapon;

        private WeaponSettings properties;

        private int countPatrons;

        private Tween twnReload;

        private Tween twnReloadPause;

        WeaponsName weaponsName;

        [Header("����� �� ���� �������� ����")]
        [HideInInspector]
        public List<Center> createBulletPointList;

        public List<CreateBulletPoints> bulletPoints;

        private int index;

        private readonly Dictionary<int, int> inGunCount = new Dictionary<int, int>();
        private SignalBus _signalBus;

        public bool isAttacking = false;
        public bool IsAttacking => isAttacking;

        [Inject]
        private void ConstructLeader(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnDestroy()
        {
            twnReload?.Kill();
        }

        /// <summary>
        /// ������������� ������������
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            //Time.timeScale = 0.075f;
        }

        /// <summary>
        /// �����
        /// </summary>
        public override void Start()
        {
            //InitWeapon(2);
        }

        public void InitWeapon(int _weapoID)
        {
            // �������� ������� ������� �������� � ����� ��� ���������� ����
            if (weaponShortInfo != null)
                inGunCount[(int)weaponsName] = countPatrons;

            twnReload?.Kill();

            weaponsName = (WeaponsName)_weapoID;

            weaponShortInfo = ProgressService.GetWeaponShortInfo(_weapoID);

            var weaponStore = AssetProvider.WeaponStoreDatabase;

            weapon = weaponStore.GetWeapon(_weapoID);

            WUpgrade upgrade = weaponStore.GetUpgrade(_weapoID, weaponShortInfo.upgradeID);

            properties = (upgrade != null)? upgrade.settings : weapon.settings;

            Init(); //������������ �����

            // ³��������� ��������� ������� ��� ����������� ������
            if (inGunCount.TryGetValue(_weapoID, out int saved))
                countPatrons = saved;
            else
            {
                countPatrons = weaponShortInfo.countPatrons;
                if (countPatrons >= weapon.magazine)
                    countPatrons = weapon.magazine;
            }

            Debug.Log("countPatrons: "+ countPatrons);

            _signalBus.Fire(new LeaderMagazineInitializedSignal(weapon.magazine));
            _signalBus.Fire(new LeaderPatronsChangedSignal(countPatrons));

            createBulletPointList = bulletPoints[_weapoID].points;

            index = 0;

            if (countPatrons < weapon.magazine)
                Reload();
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        public override void SpawnBullet()
        {
            CharacterWeapon.Fire(weapon.bulletID, properties.damage, createBulletPointList[index].GetPosition(), default(Vector3), 0);

            index = (index + 1 < createBulletPointList.Count) ? index + 1 : 0;

            if (weaponsName != WeaponsName.Weapon_1)
            {
                ProgressService.SetWeaponAmmo(weaponsName, -1);
            }

            //return;

            UpdatePatrons(-1);
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="_value"></param>
        private void UpdatePatrons(int _value)
        {
            countPatrons += _value;

            _signalBus.Fire(new LeaderPatronsChangedSignal(countPatrons));

            if (countPatrons == 0)
            {
                StopAttack();
            }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        public override void Attack()
        {
            if (countPatrons > 0)
            {
                isAttacking = true;

                twnReload?.Kill();
                
                e_animation.SetAnimation(AnimationsName.Shoot); //������������� � �������� �����
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void Reload()
        {
            twnReload?.Kill();

            twnReload = DOVirtual.DelayedCall(1f, () =>
            {
                 var weaponStore = AssetProvider.WeaponStoreDatabase;
                 twnReload = DOVirtual.DelayedCall(weaponStore.GetWeapon(weaponShortInfo.id).speedReload, () => 
                 {
                     if (countPatrons < weapon.magazine)
                     {
                         if (e_animation.GetCurrentAnimation() != AnimationsName.Reload)
                         {
                             e_animation.SetAnimation(AnimationsName.Reload); //������������� � �������� �����������
                         }

                         if (countPatrons < weaponShortInfo.countPatrons) //������� �� ������ ������ �� �� ������ ����� � �������
                         {
                             UpdatePatrons(1);
                         }
                         else
                         {
                             if (e_animation.GetCurrentAnimation() != AnimationsName.Idle)
                                 e_animation.SetAnimation(AnimationsName.Idle); //������������� � �������� ������

                             twnReload?.Kill();
                         }

                     }
                     else
                     {
                         if (e_animation.GetCurrentAnimation() != AnimationsName.Idle)
                             e_animation.SetAnimation(AnimationsName.Idle); //������������� � �������� ������

                         twnReload?.Kill();
                     }
                 }).SetLoops(-1);
             });
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        public void StopAttack()
        {
            isAttacking = false;
            
            //e_animation.SetAnimation(AnimationsName.Idle);

            //index = 0;

            //Reload();
        }
        
        public void StopShootingFinished()
        {
            index = 0;
            
            Reload();
        } 

        public Vector3 GetAimOrigin()
        {
            if (createBulletPointList != null && createBulletPointList.Count > 0 && createBulletPointList[0] != null)
                return createBulletPointList[0].GetPosition();

            if (createBulletPoint)
                return createBulletPoint.GetPosition();

            return transform.position;
        }
    }

}
