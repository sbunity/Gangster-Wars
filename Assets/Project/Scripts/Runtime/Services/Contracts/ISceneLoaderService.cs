using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISceneLoaderService
    {
        UniTask LoadAsync(SBabchuk.Runtime.Scene scene, LoadSceneMode mode = LoadSceneMode.Single);
        UniTask LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
    }
}