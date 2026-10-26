using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Tutorial
{
    public interface ITutorialStep : IDisposable
    {
        TutorialStepId Id { get; }

        TutorialStepScope Scope { get; }

        UniTask WaitForTriggerAsync(CancellationToken cancellationToken);

        UniTask<bool> RunAsync(CancellationToken cancellationToken);
    }
}
