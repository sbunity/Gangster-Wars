using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class BarricadeTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;

        public BarricadeTutorialStep(SignalBus signalBus, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.Barricade;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => _signalBus.WaitAsync<BarricadeDamagedSignal>(cancellationToken);

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            var healthBar = await _targets.WaitForAsync(TutorialTargetId.BarricadeHealth, cancellationToken);
            var hint = new TutorialHint(healthBar, _config.BarricadeCaption, TutorialGesture.Point, TutorialInputMode.PassThrough, false);
            await _view.ShowForAsync(hint, _config.BarricadeHintDuration, cancellationToken);
            return true;
        }

        public void Dispose()
        {
        }
    }
}
