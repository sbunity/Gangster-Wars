using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Progress;

namespace SBabchuk.EditorTools
{
    public interface IProgressEditSource
    {
        string Title { get; }
        bool CanDelete { get; }

        UniTask<PlayerProgress> LoadAsync(CancellationToken cancellationToken);
        UniTask SaveAsync(PlayerProgress progress, CancellationToken cancellationToken);
        void Delete();
    }
}
