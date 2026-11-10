using System.Threading;
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
        private UniTaskCompletionSource _sceneReveal;

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
            var sceneReveal = _sceneReveal = new UniTaskCompletionSource();

            try
            {
                await loadingScreen.ShowAsync();
                await _sceneLoaderService.LoadAsync(sceneName);
                Time.timeScale = 1f;
                await WaitForSceneToSettleAsync();
            }
            finally
            {
                if (_sceneReveal == sceneReveal)
                    _sceneReveal = null;

                sceneReveal.TrySetResult();
            }

            await loadingScreen.HideAsync();
        }

        public UniTask WaitForSceneRevealAsync(CancellationToken cancellationToken)
            => _sceneReveal == null
                ? UniTask.CompletedTask
                : _sceneReveal.Task.AttachExternalCancellation(cancellationToken);

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
