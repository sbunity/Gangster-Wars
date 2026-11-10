using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IUIIntroView
    {
        void Hide();
        UniTask PlayAsync(CancellationToken cancellationToken);
    }
}
