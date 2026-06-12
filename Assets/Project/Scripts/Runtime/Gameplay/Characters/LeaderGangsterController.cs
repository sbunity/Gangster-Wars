using System.Collections.Generic;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.PlayerPrefs;
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

        private WeaponShortInfo _weaponShortInfo;
        private Weapon _weapon;
        private WeaponSettings _properties;
        private int _countPatrons;
        private Tween _reloadTween;
        private WeaponsName _weaponsName;
        private int _index;
        private readonly Dictionary<int, int> _inGunCount = new();
        private readonly LeaderShotGate _shotGate = new();
        private SignalBus _signalBus;

        [Inject]
        public void ConstructLeader(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnDestroy()
        {
            _reloadTween?.Kill();
        }

        public void InitWeapon(int weaponId)
        {
            if (_weaponShortInfo != null)
                _inGunCount[(int)_weaponsName] = _countPatrons;

            _reloadTween?.Kill();

            _weaponsName = (WeaponsName)weaponId;
            _weaponShortInfo = _progressService.GetWeaponShortInfo(weaponId);

            var weaponStore = _assetProvider.WeaponStoreDatabase;
            _weapon = weaponStore.GetWeapon(weaponId);

            var upgrade = weaponStore.GetUpgrade(weaponId, _weaponShortInfo.UpgradeId);
            _properties = upgrade != null ? upgrade.Settings : _weapon.Settings;

            Init();

            if (_inGunCount.TryGetValue(weaponId, out var savedAmmo))
            {
                _countPatrons = savedAmmo;
            }
            else
            {
                _countPatrons = _weaponShortInfo.AmmoCount;

                if (_countPatrons >= _weapon.Magazine)
                    _countPatrons = _weapon.Magazine;
            }

            _signalBus.Fire(new LeaderMagazineInitializedSignal(_weapon.Magazine));
            _signalBus.Fire(new LeaderPatronsChangedSignal(_countPatrons));
            _createBulletPointList = _bulletPoints[weaponId].Points;
            _index = 0;

            if (_countPatrons < _weapon.Magazine)
                Reload();
        }

        public override void SpawnBullet()
        {
            if (_countPatrons <= 0 || !_shotGate.TryConsumeShot(out var shouldFinishAfterShot))
                return;

            _characterWeapon.Fire(_weapon.BulletId, _properties.Damage, _createBulletPointList[_index].GetPosition(), default(Vector3), 0);
            _index = _index + 1 < _createBulletPointList.Count ? _index + 1 : 0;

            if (_weaponsName != WeaponsName.Weapon_1)
                _progressService.SetWeaponAmmo(_weaponsName, -1);

            UpdatePatrons(-1);

            if (shouldFinishAfterShot)
                FinishShooting();
        }

        public override void Attack()
        {
            if (_countPatrons <= 0 || _isAttacking)
                return;

            _shotGate.Press();
            _isAttacking = true;
            _reloadTween?.Kill();

            if (Animation.GetCurrentAnimation() != AnimationsName.Shoot)
                Animation.SetAnimation(AnimationsName.Shoot);
        }

        public void Reload()
        {
            _reloadTween?.Kill();
            _reloadTween = DOVirtual.DelayedCall(1f, () =>
            {
                var weaponStore = _assetProvider.WeaponStoreDatabase;
                _reloadTween = DOVirtual.DelayedCall(weaponStore.GetWeapon(_weaponShortInfo.Id).SpeedReload, () =>
                {
                    if (_countPatrons < _weapon.Magazine)
                    {
                        if (Animation.GetCurrentAnimation() != AnimationsName.Reload)
                            Animation.SetAnimation(AnimationsName.Reload);
                        
                        if (_countPatrons < _weaponShortInfo.AmmoCount)
                        {
                            UpdatePatrons(1);
                        }
                        else
                        {
                            if (Animation.GetCurrentAnimation() != AnimationsName.Idle)
                                Animation.SetAnimation(AnimationsName.Idle);

                            _reloadTween?.Kill();
                        }
                    }
                    else
                    {
                        if (Animation.GetCurrentAnimation() != AnimationsName.Idle)
                            Animation.SetAnimation(AnimationsName.Idle);

                        _reloadTween?.Kill();
                    }
                }).SetLoops(-1);
            });
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
            Reload();
        }

        public Vector3 GetAimOrigin()
        {
            if (_createBulletPointList != null && _createBulletPointList.Count > 0 && _createBulletPointList[0] != null)
                return _createBulletPointList[0].GetPosition();
            if (CreateBulletPoint)
                return CreateBulletPoint.GetPosition();
            return transform.position;
        }

        private void UpdatePatrons(int value)
        {
            _countPatrons += value;
            _signalBus.Fire(new LeaderPatronsChangedSignal(_countPatrons));
            if (_countPatrons == 0)
                StopAttack();
        }

        private void FinishShooting()
        {
            if (Animation.GetCurrentAnimation() == AnimationsName.Shoot)
                Animation.SetAnimation(AnimationsName.Idle);

            StopShootingFinished();
        }
    }
}
