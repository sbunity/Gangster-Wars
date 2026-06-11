using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class CharacterWeapon : MonoBehaviour
    {
        private IGameFactory _gameFactory;

        [Inject]
        public void Construct(IGameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public void Fire(int bulletId, int damage, Vector3 position, Vector3 target, float offset)
        {
            _gameFactory.CreateBullet(bulletId, damage, position, target, offset, "BulletHero");
        }
    }
}