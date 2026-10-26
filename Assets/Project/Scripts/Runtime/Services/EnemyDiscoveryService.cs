using System;
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
        private readonly List<Enemy> _heldEnemies = new();
        private int _holds;

        public EnemyDiscoveryService(IPlayerProgressRepository repository, SignalBus signalBus)
        {
            _repository = repository;
            _signalBus = signalBus;
        }

        public bool HasHeldDiscoveries => _heldEnemies.Count > 0;

        public bool HasSeen(int enemyId)
            => _repository.Progress.HasSeenEnemy(enemyId) || _sessionDiscoveredEnemyIds.Contains(enemyId);

        public bool TryDiscover(Enemy enemy)
        {
            if (enemy == null || HasSeen(enemy.Id))
                return false;

            _sessionDiscoveredEnemyIds.Add(enemy.Id);

            if (_holds > 0)
                _heldEnemies.Add(enemy);
            else
                Publish(enemy);

            return true;
        }

        public IDisposable HoldNotifications()
        {
            _holds++;
            return new DisposableAction(ReleaseHold);
        }

        public void DiscardHeldDiscoveries()
        {
            foreach (var enemy in _heldEnemies)
                _sessionDiscoveredEnemyIds.Remove(enemy.Id);

            _heldEnemies.Clear();
        }

        private void ReleaseHold()
        {
            if (_holds == 0)
                return;

            _holds--;
            if (_holds > 0)
                return;

            var enemies = _heldEnemies.ToArray();
            _heldEnemies.Clear();
            foreach (var enemy in enemies)
                Publish(enemy);
        }

        private void Publish(Enemy enemy)
        {
            _repository.Progress.SeenEnemyIds.Add(enemy.Id);
            _repository.Save();
            _signalBus.Fire(new NewEnemyDiscoveredSignal(enemy.Id, enemy.Name, enemy.Icon));
        }
    }
}
