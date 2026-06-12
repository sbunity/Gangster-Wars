namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IWaveSkipRewardService
    {
        int GrantReward(float secondsUntilNextWave);
    }
}
