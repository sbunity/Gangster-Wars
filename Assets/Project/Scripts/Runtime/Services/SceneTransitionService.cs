using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class SceneTransitionService : ISceneTransitionService
    {
        private const int MaxSettleFrames = 30;

        private const float SmoothFrameSeconds = 1f / 30f;

        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreenFactory _loadingScreenFactory;
        private readonly SignalBus _signalBus;

        private ILoadingScreen _loadingScreen;

        public SceneTransitionService(ISceneLoaderService sceneLoaderService, ILoadingScreenFactory loadingScreenFactory, SignalBus signalBus)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreenFactory = loadingScreenFactory;
            _signalBus = signalBus;
        }

        public UniTask TransitionToAsync(SBabchuk.Runtime.Scene scene)
            => TransitionToAsync(scene.ToString());

        public async UniTask TransitionToAsync(string sceneName)
        {
            _signalBus.Fire(new SceneTransitionStartedSignal(sceneName));
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
