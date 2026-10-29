using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ITutorialView
    {
        void Show(TutorialHint hint);
        void Hide();
        UniTask WaitForTapAsync(CancellationToken cancellationToken);
    }
}
