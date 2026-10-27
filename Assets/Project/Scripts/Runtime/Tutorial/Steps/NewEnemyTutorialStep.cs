using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class NewEnemyTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ILevelWaveControlService _waveControl;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private readonly SignalSubscriptions _signals;
        private bool _isSelected;

        public NewEnemyTutorialStep(
            SignalBus signalBus,
            ILevelWaveControlService waveControl,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _waveControl = waveControl;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
            _signals = new SignalSubscriptions(signalBus)
                .Add<NewEnemyNotificationSelectedSignal>(() => _isSelected = true);
        }

        public TutorialStepId Id => TutorialStepId.NewEnemy;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            _signals.Enable();

            using var trigger = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await UniTask.WhenAny(
                UniTask.WaitUntil(() => _isSelected, cancellationToken: trigger.Token),
                WaitForWaveBreakAsync(trigger.Token));
            trigger.Cancel();
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (_isSelected)
                return true;

            if (!TryGetHintTarget(out var icon))
                return false;

            using var countdownHold = _waveControl.HoldCountdown();
            try
            {
                _view.Show(new TutorialHint(icon, _config.NewEnemyCaption, TutorialGesture.Tap, TutorialInputMode.TargetOnly, true));

                await _signalBus.WaitAsync<NewEnemyNotificationSelectedSignal>(cancellationToken);
                _view.Hide();
                await _signalBus.WaitAsync<EnemyInfoClosedSignal>(cancellationToken);
                return true;
            }
            finally
            {
                _view.Hide();
            }
        }

        public void Dispose()
            => _signals.Disable();

        private async UniTask WaitForWaveBreakAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await _signalBus.WaitAsync<WaveClearedSignal>(cancellationToken);
                if (TryGetHintTarget(out _))
                    return;
            }
        }

        private bool TryGetHintTarget(out ITutorialAnchor icon)
        {
            icon = null;
            return _waveControl.CanStartNextWave
                   && _waveControl.SecondsUntilNextWave >= _config.MinSecondsUntilNextWave
                   && _targets.TryGet(TutorialTargetId.NewEnemyIcon, out icon);
        }
    }
}
