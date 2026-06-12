using UnityEngine;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Gameplay.Collisions;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Gameplay.Projectiles;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IGameFactory
    {
        EnemyControllerBase CreateEnemy(EnemyOfWave enemyOfWave, Transform spawnPoint, Transform targetPoint);
        BaseBulletController CreateBullet(int bulletId, int damage, Vector3 position, Vector3 target, float offset, string tag);
        GrenadeController CreateGrenade(GrenadesName grenade, Vector3 position);
        CollisionController CreateCollision(int collisionId, Vector3 position, Grenade grenade = null);
        BonusController CreateBonus(int bonusId, Vector3 position);
    }
}
