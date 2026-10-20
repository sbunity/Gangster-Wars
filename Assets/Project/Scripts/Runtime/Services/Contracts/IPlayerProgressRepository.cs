using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPlayerProgressRepository
    {
        bool IsLoaded { get; }
        PlayerProgress Progress { get; }

        UniTask LoadAsync(CancellationToken cancellationToken);
        void Save();
    }
}
