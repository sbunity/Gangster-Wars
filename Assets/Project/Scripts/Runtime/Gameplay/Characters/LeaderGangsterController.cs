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
        private readonly LeaderShotGate _shotGate = new();
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

            Init();

            _createBulletPointList = _bulletPoints[weaponId].Points;
            _index = 0;

            _ammoService.SelectWeapon(weaponId);
        }

        public override void SpawnBullet()
        {
            if (_ammoService.IsEmpty || !_shotGate.TryConsumeShot(out var shouldFinishAfterShot))
                return;

            var firedWeapon = _ammoService.ActiveWeapon;
            _characterWeapon.Fire(_weapon.BulletId, _properties.Damage, _createBulletPointList[_index].GetPosition(), default(Vector3), 0);
            _index = _index + 1 < _createBulletPointList.Count ? _index + 1 : 0;

            _ammoService.TryConsumeRound();

            if (shouldFinishAfterShot && _ammoService.ActiveWeapon == firedWeapon)
                FinishShooting();
        }

        public override void Attack()
        {
            if (_ammoService.IsEmpty || _isAttacking)
                return;

            _shotGate.Press();
            _isAttacking = true;
            _ammoService.StopReload();

            if (Animation.GetCurrentAnimation() != AnimationsName.Shoot)
                Animation.SetAnimation(AnimationsName.Shoot);
        }

        public void StopAttack()
        {
            _isAttacking = false;

            if (_shotGate.Release())
                FinishShooting();
        }

        public void CancelAttack()
        {
            _isAttacking = false;

            if (_shotGate.Cancel() || Animation.GetCurrentAnimation() == AnimationsName.Shoot)
                FinishShooting();
        }

        public void StopShootingFinished()
        {
            _shotGate.Cancel();
            _index = 0;
            _ammoService.BeginReload();
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

        private void FinishShooting()
        {
            if (Animation.GetCurrentAnimation() == AnimationsName.Shoot)
                Animation.SetAnimation(AnimationsName.Idle);

            StopShootingFinished();
        }
    }
}
