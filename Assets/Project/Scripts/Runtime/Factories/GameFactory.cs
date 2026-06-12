using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Gameplay.Collisions;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Gameplay.Projectiles;

namespace SBabchuk.Runtime.Factories
{
    public sealed class GameFactory : IGameFactory
    {
        private readonly IPoolService _poolService;
        private readonly DiContainer _container;
        public GameFactory(IPoolService poolService, DiContainer container)
        {
            _poolService = poolService;
            _container = container;
        }

        public EnemyControllerBase CreateEnemy(EnemyOfWave enemyOfWave, Transform spawnPoint, Transform targetPoint)
        {
            var enemy = _poolService.Get<EnemyControllerBase>(NamesPool.Enemies, enemyOfWave.EnemyId);
            if (enemy == null)
                return null;
            Inject(enemy.gameObject);
            enemy.Init(enemyOfWave, spawnPoint, targetPoint, enemyOfWave.DropChance);
            return enemy;
        }

        public BaseBulletController CreateBullet(int bulletId, int damage, Vector3 position, Vector3 target, float offset, string tag)
        {
            var bullet = _poolService.Get<BaseBulletController>(NamesPool.Bullets, bulletId);
            if (bullet == null)
                return null;
            Inject(bullet.gameObject);
            bullet.transform.tag = tag;
            bullet.Init(bulletId, damage, position, target, offset);
            return bullet;
        }

        public GrenadeController CreateGrenade(GrenadesName grenade, Vector3 position)
        {
            var grenadeController = _poolService.Get<GrenadeController>(NamesPool.Grenades, (int)grenade);
            if (grenadeController == null)
                return null;
            Inject(grenadeController.gameObject);
            if (!grenadeController.Init((int)grenade, position))
                return null;

            return grenadeController;
        }

        public CollisionController CreateCollision(int collisionId, Vector3 position, Grenade grenade = null)
        {
            var collision = _poolService.Get<CollisionController>(NamesPool.Collisions, collisionId);
            if (collision == null)
                return null;
            Inject(collision.gameObject);
            if (grenade != null)
                collision.Init(position, grenade.Damage, grenade.Radius, grenade.Time);
            else
                collision.Init(position);
            return collision;
        }

        public BonusController CreateBonus(int bonusId, Vector3 position)
        {
            var bonus = _poolService.Get<BonusController>(NamesPool.Bonuses, bonusId);
            if (bonus == null)
                return null;
            Inject(bonus.gameObject);
            bonus.Init(position);
            return bonus;
        }

        private void Inject(GameObject target)
        {
            _container?.InjectGameObject(target);
        }
    }
}
