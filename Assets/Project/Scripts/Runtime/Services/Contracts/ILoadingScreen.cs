using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILoadingScreen
    {
        UniTask ShowAsync();
        UniTask HideAsync();
    }
}
