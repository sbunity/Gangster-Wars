using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Projectiles;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ICombatService
    {
        void ApplyBulletHit(EnemyControllerBase enemy, BaseBulletController bullet);
        void RewardEnemyDeath(Enemy enemy);
    }
}
