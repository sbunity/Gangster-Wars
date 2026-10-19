using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IProgressStorage
    {
        bool Exists { get; }

        bool TryLoad(out PlayerProgress progress);
        void Save(PlayerProgress progress);
        void Delete();
    }
}
