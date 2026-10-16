using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public class BigBossEnemyController : EnemyControllerBase
    {
        [SerializeField, FormerlySerializedAs("mixDuration"), Range(0, 1)]
        private float _mixDuration = 0.2f;

        [SerializeField, FormerlySerializedAs("bulletPoint")]
        private Center _bulletPoint;

        [SerializeField] private EnemyWeaponAim _weaponAim;
        [SerializeField, Min(0f)] private float _shotDistance = 20f;

        private Tween _correctionMixDuration;

        public override void Attack()
        {
            IsAttacked = true;
            if (_weaponAim != null)
                _weaponAim.AimAt(GetShotTarget());

            Animation.SetAnimation(AnimationsName.Attack);
            _correctionMixDuration = DOVirtual.DelayedCall(_mixDuration, () =>
            {
                _correctionMixDuration = DOVirtual.DelayedCall(0.1f, () =>
                {
                    GiveDamage();
                }).SetLoops(4);
            });
        }

        public override void Attacked()
        {
            WaitNextAttack(Properties.AttackSpeed);
        }

        public override void GiveDamage()
        {
            if (_bulletPoint == null)
                return;

            var bulletPosition = _bulletPoint.GetPosition();
            var direction = _bulletPoint.GetTransform().right;
            var offset = Vector2.Distance(Center.GetPosition(), bulletPosition);
            LevelSpawnService?.SpawnBullet(Properties.BulletId, Properties.Damage, bulletPosition, bulletPosition + direction * _shotDistance, offset);
            PlayShotSound();
        }

        public override void StopAllTweens()
        {
            base.StopAllTweens();
            _correctionMixDuration?.Kill();
            if (_weaponAim != null)
                _weaponAim.Release();
        }
    }
}
