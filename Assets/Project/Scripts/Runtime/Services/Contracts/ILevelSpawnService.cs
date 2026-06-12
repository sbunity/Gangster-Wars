using UnityEngine;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;

namespace SBabchuk.Runtime.Services.Contracts
{
    /// <summary>
    /// Command side of the level runtime: spawns gameplay objects into the active level.
    /// </summary>
    public interface ILevelSpawnService
    {
        void SpawnBullet(int bulletId, int damage = 0, Vector3 position = default, Vector3 target = default, float offset = 0, string tag = "Bullet");
        bool SpawnGrenadeOnPlace(GrenadesName grenade, Vector3 position);
        void SpawnCollision(int collisionId, Vector3 position, Grenade properties = null);
        void SpawnBonus(Vector3 position);
    }
}
