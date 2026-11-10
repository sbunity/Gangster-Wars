using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISceneIntro
    {
        UniTask WaitForCompletionAsync(CancellationToken cancellationToken);
    }
}
