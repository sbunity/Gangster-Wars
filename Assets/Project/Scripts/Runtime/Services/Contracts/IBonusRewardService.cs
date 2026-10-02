using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBonusRewardService
    {
        void Grant(BonusReward reward);
    }
}
