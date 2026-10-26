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

        public NextWaveTutorialStep(SignalBus signalBus, ILevelWaveControlService waveControl, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _waveControl = waveControl;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.NextWave;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await _signalBus.WaitAsync<WaveClearedSignal>(cancellationToken);
                if (CanRun())
                    return;
            }
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (!CanRun())
                return false;

            using var countdownHold = _waveControl.HoldCountdown();
            var button = await _targets.WaitForAsync(TutorialTargetId.NextWaveButton, cancellationToken);
            var hint = new TutorialHint(button, _config.NextWaveCaption, TutorialGesture.Tap, TutorialInputMode.TargetOnly, true);
            await _view.ShowUntilAsync(hint, _signalBus.WaitAsync<WaveCountdownSkippedSignal>(cancellationToken));
            return true;
        }

        public void Dispose()
        {
        }

        private bool CanRun()
            => _waveControl.CanStartNextWave
               && _waveControl.SecondsUntilNextWave >= _config.MinSecondsUntilNextWave;
    }
}
