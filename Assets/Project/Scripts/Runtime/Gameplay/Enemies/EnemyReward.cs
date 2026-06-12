using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Databases.Enemies;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyReward : MonoBehaviour
    {
        private ICombatService _combatService;

        [Inject]
        public void Construct(ICombatService combatService)
        {
            _combatService = combatService;
        }

        public void Grant(Enemy enemy)
        {
            if (enemy == null)
                return;
                
            _combatService?.RewardEnemyDeath(enemy);
        }
    }
}
