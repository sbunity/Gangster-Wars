using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyReward : MonoBehaviour
    {
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        public void Grant(Enemy enemy)
        {
            if (enemy == null)
                return;
                
            _progressService?.AddCoins(enemy.Gold);
        }
    }
}