using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class SceneTransitionService : ISceneTransitionService
    {
        // Upper bound of frames we let the freshly loaded scene "settle" (spawning,
        // pool warmup, Start callbacks) before fading out, so the heavy first frames
        // happen under a fully black screen instead of cutting through the fade.
        private const int MaxSettleFrames = 30;

        // A frame is considered "smooth" once it runs at least this fast (~30 FPS).
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
            await WaitForSceneToSettleAsync();
            await loadingScreen.HideAsync();
        }

        // The first frames of a freshly loaded scene are usually a hitch (instantiation,
        // pool warmup). Rendering is frozen during that hitch, so a fade started right
        // away is never actually drawn and the black screen appears to vanish abruptly.
        // Waiting here keeps the screen fully black until the frame rate recovers, then
        // the fade-out plays smoothly.
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
