using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BomberGangsterController : GangsterControllerBase
    {
        [SerializeField, FormerlySerializedAs("target")]
        private Transform _target;

        [SerializeField, FormerlySerializedAs("center")]
        private Center _center;

        private Vector3 _lastTargetPosition;
        private bool _hasLastTargetPosition;
        private PersonageSettings _properties;
        private Personage _personage;

        public override void Awake()
        {
            _center = GetComponentInChildren<Center>();
            base.Awake();
        }

        public override void Init()
        {
            Animation.SetAnimation(AnimationsName.Idle);
            var _shortInfo = _progressService.GetPersonageShortInfo((int)PersonagesName.Bomber);
            var playerStore = _assetProvider.MainPlayerDatabase;
            _personage = playerStore.GetPersonage((int)PersonagesName.Bomber);
            var _upgrade = playerStore.GetUpgrade((int)PersonagesName.Bomber, _shortInfo.UpgradeId);
            
            if (_upgrade != null)
                _properties = _upgrade.Settings;
            else
                _properties = playerStore.GetPersonage((int)PersonagesName.Bomber).Settings;

            StartAttack();
        }

        public void StartAttack()
        {
            AttackTween = DOVirtual.DelayedCall(_properties.AttackSpeed, () =>
            {
                if (FindEnemy())
                    Attack();
            }, false).SetLoops(-1);
        }

        public override void Update()
        {
            base.Update();
            UpdateLastTargetPosition();
        }

        public override void Attack()
        {
            UpdateLastTargetPosition();
            Animation.SetAnimation(AnimationsName.Throwing);
        }

        public override void SpawnBullet()
        {
            var offset = Vector2.Distance(_center.GetPosition(), CreateBulletPoint.GetPosition());
            _characterWeapon.Fire(_personage.BulletId, _properties.Damage, CreateBulletPoint.GetPosition(), GetTargetPosition(), offset);
        }

        public bool FindEnemy()
        {
            if (_target != null && _target.gameObject.activeInHierarchy)
            {
                UpdateLastTargetPosition();
                return true;
            }

            _target = _levelRuntimeService?.GetRandomEnemy();
            UpdateLastTargetPosition();
            return _target != null;
        }

        private void UpdateLastTargetPosition()
        {
            if (_target == null)
                return;

            if (!_target.gameObject.activeInHierarchy)
            {
                _target = null;
                return;
            }

            _lastTargetPosition = _target.position;
            _hasLastTargetPosition = true;
        }

        private Vector3 GetTargetPosition()
        {
            UpdateLastTargetPosition();
            if (_hasLastTargetPosition)
                return _lastTargetPosition;
                
            return _target != null ? _target.position : CreateBulletPoint.GetPosition();
        }
    }
}
