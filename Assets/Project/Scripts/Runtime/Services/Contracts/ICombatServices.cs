using UnityEngine;

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
        void ApplyGrenadeDamage(Vector3 position, int damage, float radius);
        void RewardEnemyDeath(Enemy enemy);
    }
}