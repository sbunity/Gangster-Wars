using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class EnemyDiscoveryService : IEnemyDiscoveryService
    {
        private readonly IPlayerProgressRepository _repository;
        private readonly SignalBus _signalBus;
        private readonly HashSet<int> _sessionDiscoveredEnemyIds = new();

        public EnemyDiscoveryService(IPlayerProgressRepository repository, SignalBus signalBus)
        {
            _repository = repository;
            _signalBus = signalBus;
        }

        public bool HasSeen(int enemyId)
            => _repository.Progress.HasSeenEnemy(enemyId) || _sessionDiscoveredEnemyIds.Contains(enemyId);

        public bool TryDiscover(Enemy enemy)
        {
            if (enemy == null || HasSeen(enemy.Id))
                return false;

            _repository.Progress.SeenEnemyIds.Add(enemy.Id);
            _sessionDiscoveredEnemyIds.Add(enemy.Id);

            _repository.Save();
            _signalBus.Fire(new NewEnemyDiscoveredSignal(enemy.Id, enemy.Name, enemy.Icon));
            return true;
        }
    }
}
