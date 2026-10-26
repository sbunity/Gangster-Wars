using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelSpawnService : ILevelSpawnService
    {
        private readonly IGameFactory _gameFactory;
        private readonly IBonusDropService _bonusDropService;
        private readonly LevelEntityTracker _entityTracker;
        private readonly SignalBus _signalBus;

        public LevelSpawnService(IGameFactory gameFactory, IBonusDropService bonusDropService, LevelEntityTracker entityTracker, SignalBus signalBus)
        {
            _gameFactory = gameFactory;
            _bonusDropService = bonusDropService;
            _entityTracker = entityTracker;
            _signalBus = signalBus;
        }

        public void SpawnBullet(int bulletId, int damage = 0, Vector3 position = default, Vector3 target = default, float offset = 0, string tag = "Bullet")
        {
            var bullet = _gameFactory.CreateBullet(bulletId, damage, position, target, offset, tag);
            if (bullet == null)
                Debug.LogWarning("BaseBulletController is missing from the spawned bullet.");
        }

        public bool SpawnGrenadeOnPlace(GrenadesName grenade, Vector3 position)
        {
            var grenadeController = _gameFactory.CreateGrenade(grenade, position);
            if (grenadeController != null)
                return true;

            Debug.LogWarning("GrenadeController is missing from the spawned grenade.");
            return false;
        }

        public void SpawnCollision(int collisionId, Vector3 position, Grenade properties = null)
        {
            var collision = _gameFactory.CreateCollision(collisionId, position, properties);
            if (collision == null)
                Debug.LogWarning("CollisionController is missing from the spawned collision.");
        }

        public void SpawnBonus(Vector3 position)
        {
            var bonusId = _bonusDropService.GetAvailableBonusId();
            if (bonusId < 0)
                return;

            var bonus = _gameFactory.CreateBonus(bonusId, position);
            if (bonus == null)
            {
                Debug.LogWarning("BonusController is missing from the spawned bonus.");
                return;
            }

            _entityTracker.AddBonus(bonus);
            _signalBus.Fire(new BonusSpawnedSignal(bonus));
        }
    }
}
