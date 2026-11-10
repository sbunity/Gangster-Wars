using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.UI.Intro
{
    public sealed class NoSceneIntro : ISceneIntro
    {
        public UniTask WaitForCompletionAsync(CancellationToken cancellationToken) => UniTask.CompletedTask;
    }
}
