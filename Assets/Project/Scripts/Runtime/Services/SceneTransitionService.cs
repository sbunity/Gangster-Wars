using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class SceneTransitionService : ISceneTransitionService
    {
        private const int MaxSettleFrames = 30;

        private const float SmoothFrameSeconds = 1f / 30f;

        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreenFactory _loadingScreenFactory;

        private ILoadingScreen _loadingScreen;

        public SceneTransitionService(ISceneLoaderService sceneLoaderService, ILoadingScreenFactory loadingScreenFactory)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreenFactory = loadingScreenFactory;
        }

        public UniTask TransitionToAsync(SBabchuk.Runtime.Scene scene)
            => TransitionToAsync(scene.ToString());

        public async UniTask TransitionToAsync(string sceneName)
        {
            var loadingScreen = GetOrCreateLoadingScreen();

            await loadingScreen.ShowAsync();
            await _sceneLoaderService.LoadAsync(sceneName);
            Time.timeScale = 1f;
            await WaitForSceneToSettleAsync();
            await loadingScreen.HideAsync();
        }

        private static async UniTask WaitForSceneToSettleAsync()
        {
            for (var frame = 0; frame < MaxSettleFrames; frame++)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);

                if (Time.unscaledDeltaTime <= SmoothFrameSeconds)
                    break;
            }
        }

        private ILoadingScreen GetOrCreateLoadingScreen()
            => _loadingScreen ??= _loadingScreenFactory.Create();
    }
}
