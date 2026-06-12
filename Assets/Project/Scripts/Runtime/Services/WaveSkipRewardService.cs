using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class WaveSkipRewardService : IWaveSkipRewardService
    {
        private readonly IPlayerProgressService _progressService;

        public WaveSkipRewardService(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        public int GrantReward(float secondsUntilNextWave)
        {
            var reward = Mathf.Max(0, Mathf.CeilToInt(secondsUntilNextWave));
            if (reward > 0)
                _progressService.AddCoins(reward);

            return reward;
        }
    }
}
