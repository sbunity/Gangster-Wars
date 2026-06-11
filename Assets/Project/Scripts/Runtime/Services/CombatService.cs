using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class CombatService : ICombatService
    {
        private readonly IDamageService _damageService;
        private readonly IPlayerProgressService _progressService;

        public CombatService(IDamageService damageService, IPlayerProgressService progressService)
        {
            _damageService = damageService;
            _progressService = progressService;
        }

        public void ApplyBulletHit(EnemyControllerBase enemy, BaseBulletController bullet)
        {
            if (enemy == null || bullet == null)
                return;
                
            bullet.Pop();
            _damageService.DamageEnemy(enemy, bullet.Damage);
        }

        public void ApplyGrenadeDamage(Vector3 position, int damage, float radius)
        {
        }

        public void RewardEnemyDeath(Enemy enemy)
        {
            if (enemy != null)
                _progressService.AddCoins(enemy.Gold);
        }
    }
}