using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPlayerProgressNormalizer
    {
        bool Normalize(PlayerProgress progress);
    }
}
