using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class NextWaveTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ILevelWaveControlService _waveControl;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private readonly SignalSubscriptions _signals;
        private bool _isSkipped;

        public NextWaveTutorialStep(SignalBus signalBus, ILevelWaveControlService waveControl, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _waveControl = waveControl;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
            _signals = new SignalSubscriptions(signalBus)
                .Add<WaveCountdownSkippedSignal>(() => _isSkipped = true);
        }

        public TutorialStepId Id => TutorialStepId.NextWave;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            _signals.Enable();

            using var trigger = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await UniTask.WhenAny(
                UniTask.WaitUntil(() => _isSkipped, cancellationToken: trigger.Token),
                WaitForWaveBreakAsync(trigger.Token));
            trigger.Cancel();
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (_isSkipped)
                return true;

            if (!CanRun())
                return false;

            using var countdownHold = _waveControl.HoldCountdown();
            var button = await _targets.WaitForAsync(TutorialTargetId.NextWaveButton, cancellationToken);
            var hint = new TutorialHint(button, _config.NextWaveCaption, TutorialGesture.Tap, TutorialInputMode.TargetOnly, true);
            await _view.ShowUntilAsync(hint, _signalBus.WaitAsync<WaveCountdownSkippedSignal>(cancellationToken));
            return true;
        }

        public void Dispose()
            => _signals.Disable();

        private async UniTask WaitForWaveBreakAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await _signalBus.WaitAsync<WaveClearedSignal>(cancellationToken);
                if (CanRun())
                    return;
            }
        }

        private bool CanRun()
            => _waveControl.CanStartNextWave
               && _waveControl.SecondsUntilNextWave >= _config.MinSecondsUntilNextWave;
    }
}
