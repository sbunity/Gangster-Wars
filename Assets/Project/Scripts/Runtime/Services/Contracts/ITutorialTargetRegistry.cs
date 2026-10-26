using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ITutorialTargetRegistry
    {
        void Register(TutorialTargetKey key, ITutorialAnchor target);
        void Unregister(TutorialTargetKey key, ITutorialAnchor target);
        bool TryGet(TutorialTargetKey key, out ITutorialAnchor target);
        UniTask<ITutorialAnchor> WaitForAsync(TutorialTargetKey key, CancellationToken cancellationToken);
    }
}
