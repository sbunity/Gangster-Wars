using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPlayerProgressFactory
    {
        PlayerProgress CreateDefault();
    }
}
