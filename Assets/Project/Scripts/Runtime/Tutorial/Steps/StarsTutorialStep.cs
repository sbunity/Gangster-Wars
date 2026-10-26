using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class StarsTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;

        public StarsTutorialStep(SignalBus signalBus, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.Stars;
        public TutorialStepScope Scope => TutorialStepScope.Scene;

        public UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => _signalBus.WaitAsync<GameFinishedSignal>(cancellationToken, signal => signal.Panel == Panels.Win);

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            var stars = await _targets.WaitForAsync(TutorialTargetId.WinStars, cancellationToken);
            _view.Show(new TutorialHint(stars, _config.StarsCaption, TutorialGesture.None, TutorialInputMode.PassThrough, false));
            return true;
        }

        public void Dispose()
        {
        }
    }
}
