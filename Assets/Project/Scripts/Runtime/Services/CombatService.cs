using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Projectiles;

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

        public void RewardEnemyDeath(Enemy enemy)
        {
            if (enemy != null)
                _progressService.AddCoins(enemy.Gold);
        }
    }
}
