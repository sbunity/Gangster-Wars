using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Bootstrap
{
    public sealed class InitSceneEntryPoint : IInitializable, IDisposable
    {
        private readonly IGameBootstrapper _bootstrapper;
        private readonly ISceneTransitionService _sceneTransitionService;
        private readonly IFirstSceneRouter _firstSceneRouter;
        private readonly BootstrapSettings _settings;
        private readonly CancellationTokenSource _cancellation = new();

        public InitSceneEntryPoint(IGameBootstrapper bootstrapper, ISceneTransitionService sceneTransitionService, IFirstSceneRouter firstSceneRouter, BootstrapSettings settings)
        {
            _bootstrapper = bootstrapper;
            _sceneTransitionService = sceneTransitionService;
            _firstSceneRouter = firstSceneRouter;
            _settings = settings;
        }

        public void Initialize()
            => RunAsync(_cancellation.Token).Forget();

        public void Dispose()
        {
            _cancellation.Cancel();
            _cancellation.Dispose();
        }

        private async UniTaskVoid RunAsync(CancellationToken cancellationToken)
        {
            var minDuration = UniTask.Delay(_settings.MinDuration, ignoreTimeScale: true, cancellationToken: cancellationToken);
            var bootstrap = _bootstrapper.RunAsync(cancellationToken).Preserve();

            await minDuration;

            var finishedInTime = bootstrap.Status.IsCompleted();
            await bootstrap;

            if (!finishedInTime)
                await UniTask.Delay(_settings.OvertimeTail, ignoreTimeScale: true, cancellationToken: cancellationToken);

            await _sceneTransitionService.TransitionToAsync(_firstSceneRouter.PrepareFirstScene());
        }
    }
}
