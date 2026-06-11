using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class CharacterWeapon : MonoBehaviour
    {
        private IGameFactory _gameFactory;
        private ILevelRuntimeService _levelRuntimeService;
        
        [Inject]
        public void Construct(IGameFactory gameFactory, ILevelRuntimeService levelRuntimeService)
        {
            _gameFactory = gameFactory;
            _levelRuntimeService = levelRuntimeService;
        }

        public void Fire(int bulletId, int damage, Vector3 position, Vector3 target, float offset)
        {
            if (_gameFactory != null)
                _gameFactory.CreateBullet(bulletId, damage, position, target, offset, "BulletHero");
            else if (_levelRuntimeService != null)
                _levelRuntimeService.SpawnBullet(bulletId, damage, position, target, offset, "BulletHero");
        }
    }
}