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
        private readonly ILevelCoinsService _levelCoins;

        public CombatService(IDamageService damageService, ILevelCoinsService levelCoins)
        {
            _damageService = damageService;
            _levelCoins = levelCoins;
        }

        public void ApplyBulletHit(EnemyControllerBase enemy, BaseBulletController bullet)
        {
            if (enemy == null || bullet == null)
                return;

            var impact = bullet.Pop();
            impact?.SortInFrontOf(enemy.Sorting);
            _damageService.DamageEnemy(enemy, bullet.Damage);
        }

        public void RewardEnemyDeath(Enemy enemy)
        {
            if (enemy != null)
                _levelCoins.Earn(enemy.Gold);
        }
    }
}
