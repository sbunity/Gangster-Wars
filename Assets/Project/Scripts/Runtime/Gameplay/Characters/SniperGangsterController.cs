using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public class SniperGangsterController : GangsterControllerBase
    {
        [SerializeField, FormerlySerializedAs("aim")]
        private Transform _aim;

        [SerializeField, FormerlySerializedAs("speedRotate"), Range(0, 2)]
        private float _speedRotate;

        [SerializeField, FormerlySerializedAs("target")]
        private Transform _target;

        [SerializeField, FormerlySerializedAs("aimFireThreshold"), Range(0.01f, 1f)]
        
        private float _aimFireThreshold = 0.15f;
        private PersonageSettings _properties;
        private Personage _personage;
        private Vector3 _aimDefaultPosition;
        private bool _readyToFire;
        private float _aimTimer;

        public override void Init()
        {
            Animation.SetAnimation(AnimationsName.Idle);
            PersonageShortInfo _shortInfo = _progressService.GetPersonageShortInfo((int)PersonagesName.Sniper);
            
            var playerStore = _assetProvider.MainPlayerDatabase;
            _personage = playerStore.GetPersonage((int)PersonagesName.Sniper);
            PUpgrade _upgrade = playerStore.GetUpgrade((int)PersonagesName.Sniper, _shortInfo.UpgradeId);
            
            if (_upgrade != null)
            {
                _properties = _upgrade.Settings;
            }
            else
            {
                _properties = playerStore.GetPersonage((int)PersonagesName.Sniper).Settings;
            }

            _aimDefaultPosition = _aim.position;
            StartAttack();
        }

        public void StartAttack()
        {
            AttackTween = DOVirtual.DelayedCall(_properties.AttackSpeed, () =>
            {
                if (FindEnemy())
                    _readyToFire = true;
            }, false).SetLoops(-1);
        }

        public override void Update()
        {
            base.Update();
            Vector3 dest = (_target != null && _target.gameObject.activeInHierarchy) ? _target.position : _aimDefaultPosition;
            _aim.position = Vector3.Lerp(_aim.position, dest, Time.deltaTime / Mathf.Max(_speedRotate, 0.01f));
            if (_readyToFire)
            {
                if (_target != null && _target.gameObject.activeInHierarchy)
                {
                    _aimTimer += Time.deltaTime;
                    if (_aimTimer >= _speedRotate)
                    {
                        _readyToFire = false;
                        _aimTimer = 0f;
                        Attack();
                    }
                }
                else
                {
                    _readyToFire = false;
                    _aimTimer = 0f;
                }
            }
        }

        public override void Attack()
        {
            Animation.SetAnimation(AnimationsName.Shoot_prev);
        }

        public override void SpawnBullet()
        {
            _characterWeapon.Fire(_personage.BulletId, _properties.Damage, CreateBulletPoint.GetPosition(), _target.position, 0);
        }

        public override void AttackEnded()
        {
        }

        public bool FindEnemy()
        {
            if (_target != null && _target.gameObject.activeInHierarchy)
                return true;
            _target = _enemyTargetProvider?.GetRandomEnemy();
            return _target != null;
        }
    }
}
