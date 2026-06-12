using UnityEngine;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Projectiles;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IDamageService
    {
        void DamageEnemy(EnemyControllerBase enemy, int damage);
        void DamageBarricade(BarricadeController barricade, int damage);
    }

    public interface ICombatService
    {
        void ApplyBulletHit(EnemyControllerBase enemy, BaseBulletController bullet);
        void RewardEnemyDeath(Enemy enemy);
    }
}
