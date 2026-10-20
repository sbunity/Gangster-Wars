using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Progress;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IProgressStorage
    {
        UniTask<PlayerProgress> LoadAsync(CancellationToken cancellationToken);
        UniTask SaveAsync(PlayerProgress progress, CancellationToken cancellationToken);
    }
}
