using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class WaveSkipRewardService : IWaveSkipRewardService
    {
        private readonly ILevelCoinsService _levelCoins;

        public WaveSkipRewardService(ILevelCoinsService levelCoins)
        {
            _levelCoins = levelCoins;
        }

        public int GrantReward(float secondsUntilNextWave)
        {
            var reward = Mathf.Max(0, Mathf.CeilToInt(secondsUntilNextWave));
            _levelCoins.Earn(reward);

            return reward;
        }
    }
}
