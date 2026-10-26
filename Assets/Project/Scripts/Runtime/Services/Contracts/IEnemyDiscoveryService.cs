using System;
using SBabchuk.Runtime.Databases.Enemies;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IEnemyDiscoveryService
    {
        bool HasHeldDiscoveries { get; }

        bool HasSeen(int enemyId);
        bool TryDiscover(Enemy enemy);
        IDisposable HoldNotifications();
        void DiscardHeldDiscoveries();
    }
}
