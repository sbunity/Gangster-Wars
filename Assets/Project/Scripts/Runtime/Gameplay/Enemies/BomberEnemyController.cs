using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public class BomberEnemyController : EnemyControllerBase
    {
        [SerializeField, FormerlySerializedAs("mixDuration"), Range(0, 1)]
        private float _mixDuration = 0.2f;

        [SerializeField, FormerlySerializedAs("bulletPoints")]
        private List<Center> _bulletPoints;

        private int _index = 0;
        private Tween _correctionMixDuration;

        public override void Attacked()
        {
            _index = 0;
            WaitNextAttack(Properties.AttackSpeed);
        }

        public override void GiveDamage()
        {
            if (_bulletPoints == null || _bulletPoints.Count == 0)
                return;
            
            _correctionMixDuration = DOVirtual.DelayedCall(_mixDuration, () =>
            {
                var bulletPoint = _bulletPoints[_index % _bulletPoints.Count];
                var bulletPosition = bulletPoint.GetPosition();
                var offset = Vector2.Distance(Center.GetPosition(), bulletPosition);
                
                LevelSpawnService?.SpawnBullet(Properties.BulletId, Properties.Damage, bulletPosition, Target.position, offset);
                _index++;
            });
        }

        public override void StopAllTweens()
        {
            base.StopAllTweens();
            _correctionMixDuration?.Kill();
        }
    }
}
