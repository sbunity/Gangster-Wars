namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IRewardedCoinsService
    {
        int Amount { get; }
        bool CanWatch { get; }
        void Watch();
    }
}
