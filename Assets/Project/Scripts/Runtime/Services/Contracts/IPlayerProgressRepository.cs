using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPlayerProgressRepository
    {
        PlayerProgress Progress { get; }

        void Save();
    }
}
