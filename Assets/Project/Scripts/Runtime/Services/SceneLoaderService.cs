using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine.SceneManagement;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class SceneLoaderService : ISceneLoaderService
    {
        private readonly ZenjectSceneLoader _sceneLoader;

        public SceneLoaderService(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public UniTask LoadAsync(SBabchuk.Runtime.Scene scene, LoadSceneMode mode = LoadSceneMode.Single) 
            => LoadAsync(scene.ToString(), mode);

        public async UniTask LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var operation = _sceneLoader != null ? _sceneLoader.LoadSceneAsync(sceneName, mode) : SceneManager.LoadSceneAsync(sceneName, mode);
            await operation.ToUniTask();
        }
    }
}