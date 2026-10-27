using SBabchuk.Runtime.Databases.Enemies;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IEnemyDiscoveryService
    {
        bool HasSeen(int enemyId);
        bool TryDiscover(Enemy enemy);
    }
}
