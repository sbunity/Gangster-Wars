namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelCoinsService
    {
        int Earned { get; }

        void Earn(int amount);
    }
}
