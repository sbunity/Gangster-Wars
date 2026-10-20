using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Bootstrap
{
    public interface IBootstrapStep
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken);
    }
}
