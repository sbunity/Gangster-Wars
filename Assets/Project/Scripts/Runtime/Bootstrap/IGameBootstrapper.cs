using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Bootstrap
{
    public interface IGameBootstrapper
    {
        UniTask RunAsync(CancellationToken cancellationToken);
    }
}
