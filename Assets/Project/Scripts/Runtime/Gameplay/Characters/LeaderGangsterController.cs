using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.WeaponStore;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    [System.Serializable]
    public class CreateBulletPoints
    {
        [SerializeField, FormerlySerializedAs("points")]
        private List<Center> _points;
        public List<Center> Points { get => _points; set => _points = value; }
    }

    public class LeaderGangsterController : GangsterControllerBase, ILeaderWeaponController
    {
        [SerializeField, FormerlySerializedAs("createBulletPointList")]
        private List<Center> _createBulletPointList;

        [SerializeField, FormerlySerializedAs("bulletPoints")]
        private List<CreateBulletPoints> _bulletPoints;

        [SerializeField, FormerlySerializedAs("isAttacking")]
        private bool _isAttacking = false;
        public bool IsAttacking => _isAttacking;

        private Weapon _weapon;
        private WeaponSettings _properties;
        private int _index;
        private bool _isShooting;
        private readonly WeaponFireCooldown _fireCooldown = new();
        private IWeaponAmmoService _ammoService;

        [Inject]
        public void ConstructLeader(IWeaponAmmoService ammoService)
        {
            _ammoService = ammoService;
            _ammoService.ReloadAdvanced += OnReloadAdvanced;
            _ammoService.ReloadCompleted += OnReloadCompleted;
            _ammoService.MagazineEmptied += OnMagazineEmptied;
        }

        private void OnDestroy()
        {
            if (_ammoService == null)
                return;

            _ammoService.ReloadAdvanced -= OnReloadAdvanced;
            _ammoService.ReloadCompleted -= OnReloadCompleted;
            _ammoService.MagazineEmptied -= OnMagazineEmptied;
        }

        public void InitWeapon(int weaponId)
        {
            var weaponStore = _assetProvider.WeaponStoreDatabase;
            var weaponShortInfo = _progressService.GetWeaponShortInfo(weaponId);
            _weapon = weaponStore.GetWeapon(weaponId);

            var upgrade = weaponShortInfo != null ? weaponStore.GetUpgrade(weaponId, weaponShortInfo.UpgradeId) : null;
            _properties = upgrade != null ? upgrade.Settings : _weapon?.Settings;
            _fireCooldown.SetFireRate(_weapon?.FireRate ?? 0f);

            Init();

            _createBulletPointList = _bulletPoints[weaponId].Points;
            _index = 0;
            _isShooting = false;

            _ammoService.SelectWeapon(weaponId);
        }

        public override void Update()
        {
            base.Update();

            if (_isAttacking)
                TryShoot();
            else if (_isShooting && _fireCooldown.IsReady(Time.time))
                FinishShooting();
        }

        public override void SpawnBullet()
        {
        }

        public override void Attack()
        {
            if (_ammoService.IsEmpty || _isAttacking)
                return;

            _isAttacking = true;
            TryShoot();
        }

        public void StopAttack()
        {
            _isAttacking = false;
        }

        public void CancelAttack()
        {
            _isAttacking = false;

            if (_isShooting || Animation.GetCurrentAnimation() == AnimationsName.Shoot)
                FinishShooting();
        }

        public Vector3 GetAimOrigin()
        {
            if (_createBulletPointList != null && _createBulletPointList.Count > 0 && _createBulletPointList[0] != null)
                return _createBulletPointList[0].GetPosition();
            if (CreateBulletPoint)
                return CreateBulletPoint.GetPosition();
            return transform.position;
        }

        private void OnReloadAdvanced()
        {
            if (Animation.GetCurrentAnimation() != AnimationsName.Reload)
                Animation.SetAnimation(AnimationsName.Reload);
        }

        private void OnReloadCompleted()
        {
            if (Animation.GetCurrentAnimation() != AnimationsName.Idle)
                Animation.SetAnimation(AnimationsName.Idle);
        }

        private void OnMagazineEmptied() => StopAttack();

        private void TryShoot()
        {
            if (_ammoService.IsEmpty || !_fireCooldown.TryConsume(Time.time, Time.deltaTime))
                return;

            if (!_isShooting)
                BeginShooting();

            Shoot();
        }

        private void BeginShooting()
        {
            _isShooting = true;
            _ammoService.StopReload();
            Animation.PlayFireLoop(_weapon.FireRate);
        }

        private void Shoot()
        {
            _characterWeapon.Fire(_weapon.BulletId, _properties.Damage, _createBulletPointList[_index].GetPosition(), default(Vector3), 0);
            _index = _index + 1 < _createBulletPointList.Count ? _index + 1 : 0;
            _ammoService.TryConsumeRound();
        }

        private void FinishShooting()
        {
            if (Animation.GetCurrentAnimation() == AnimationsName.Shoot)
                Animation.SetAnimation(AnimationsName.Idle);

            _isShooting = false;
            _index = 0;
            _ammoService.BeginReload();
        }
    }
}
