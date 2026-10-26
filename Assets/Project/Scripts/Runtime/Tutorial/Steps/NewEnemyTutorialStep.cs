using System;
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
        private readonly IEnemyDiscoveryService _discoveryService;
        private readonly ILevelWaveControlService _waveControl;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private IDisposable _notificationsHold;

        public NewEnemyTutorialStep(
            SignalBus signalBus,
            IEnemyDiscoveryService discoveryService,
            ILevelWaveControlService waveControl,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _discoveryService = discoveryService;
            _waveControl = waveControl;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.NewEnemy;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            _notificationsHold ??= _discoveryService.HoldNotifications();

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
            try
            {
                ReleaseNotifications();

                var icon = await _targets.WaitForAsync(TutorialTargetId.NewEnemyIcon, cancellationToken);
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
        {
            if (_notificationsHold == null)
                return;

            _discoveryService.DiscardHeldDiscoveries();
            ReleaseNotifications();
        }

        private bool CanRun()
            => _discoveryService.HasHeldDiscoveries
               && _waveControl.CanStartNextWave
               && _waveControl.SecondsUntilNextWave >= _config.MinSecondsUntilNextWave;

        private void ReleaseNotifications()
        {
            var hold = _notificationsHold;
            _notificationsHold = null;
            hold?.Dispose();
        }
    }
}
