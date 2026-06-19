using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class EnemyDiscoveryService : IEnemyDiscoveryService
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ISaveService _saveService;
        private readonly SignalBus _signalBus;
        private readonly HashSet<int> _sessionDiscoveredEnemyIds = new HashSet<int>();

        public EnemyDiscoveryService(IPlayerProgressService progressService, ISaveService saveService, SignalBus signalBus)
        {
            _progressService = progressService;
            _saveService = saveService;
            _signalBus = signalBus;
        }

        public bool HasSeen(int enemyId)
            => _progressService.PlayerPrefs.HasSeenEnemy(enemyId) || _sessionDiscoveredEnemyIds.Contains(enemyId);

        public bool TryDiscover(Enemy enemy)
        {
            if (enemy == null || HasSeen(enemy.Id))
                return false;

            EnsureSeenEnemyIds();
            _progressService.PlayerPrefs.SeenEnemyIds.Add(enemy.Id);
            _sessionDiscoveredEnemyIds.Add(enemy.Id);

            _saveService.SaveAsync(_progressService.Preferences).Forget();
            _signalBus.Fire(new NewEnemyDiscoveredSignal(enemy.Id, enemy.Name, enemy.Icon));
            return true;
        }

        private void EnsureSeenEnemyIds()
        {
            if (_progressService.PlayerPrefs.SeenEnemyIds == null)
                _progressService.PlayerPrefs.SeenEnemyIds = new List<int>();
        }
    }
}
