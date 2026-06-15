using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISceneTransitionService
    {
        UniTask TransitionToAsync(SBabchuk.Runtime.Scene scene);
        UniTask TransitionToAsync(string sceneName);
    }
}
