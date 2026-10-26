using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public abstract class TapToTransitionTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ITutorialProgressService _tutorialProgress;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;

        protected TapToTransitionTutorialStep(
            SignalBus signalBus,
            ITutorialProgressService tutorialProgress,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _tutorialProgress = tutorialProgress;
            _targets = targets;
            _view = view;
            Config = assetProvider.TutorialConfig;
        }

        public abstract TutorialStepId Id { get; }
        public TutorialStepScope Scope => TutorialStepScope.Scene;

        protected TutorialConfig Config { get; }
        protected abstract TutorialTargetKey Target { get; }
        protected abstract string Caption { get; }
        protected abstract Scene Destination { get; }

        private bool IsLeadingToGrenadeLevel
            => _tutorialProgress.IsTutorialLevelCompleted && !_tutorialProgress.IsLevelCompleted(Config.GrenadeLevelId);

        public UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => IsLeadingToGrenadeLevel ? UniTask.CompletedTask : UniTask.Never(cancellationToken);

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            var target = await _targets.WaitForAsync(Target, cancellationToken);
            var hint = new TutorialHint(target, Caption, TutorialGesture.Tap, TutorialInputMode.TargetOnly, true);
            await _view.ShowUntilAsync(hint, _signalBus.WaitAsync<SceneTransitionStartedSignal>(cancellationToken, signal => signal.Is(Destination)));
            return true;
        }

        public void Dispose()
        {
        }
    }
}
