using System;
using System.Linq;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Tutorial
{
    public sealed class TutorialWaveBreakGuard : IInitializable, ITickable, IDisposable
    {
        private readonly ILevelWaveControlService _waveControl;
        private readonly IEnemyTargetProvider _enemies;
        private readonly ITutorialProgressService _tutorialProgress;
        private readonly TutorialConfig _config;
        private readonly SignalSubscriptions _signals;
        private IDisposable _countdownHold;
        private bool _isActive;

        public TutorialWaveBreakGuard(
            ILevelWaveControlService waveControl,
            IEnemyTargetProvider enemies,
            ITutorialProgressService tutorialProgress,
            IAssetProvider assetProvider,
            SignalBus signalBus)
        {
            _waveControl = waveControl;
            _enemies = enemies;
            _tutorialProgress = tutorialProgress;
            _config = assetProvider.TutorialConfig;
            _signals = new SignalSubscriptions(signalBus)
                .Add<WaveClearedSignal>(ReleaseCountdown)
                .Add<GameFinishedSignal>(Stop);
        }

        private bool AreWaveBreakStepsCompleted
            => _tutorialProgress.IsCompleted(TutorialStepId.NewEnemy) && _tutorialProgress.IsCompleted(TutorialStepId.NextWave);

        public void Initialize()
        {
            _isActive = _tutorialProgress.IsTutorialLevelActive && !AreWaveBreakStepsCompleted;
            if (_isActive)
                _signals.Enable();
        }

        public void Tick()
        {
            if (!_isActive || _countdownHold != null)
                return;

            if (AreWaveBreakStepsCompleted)
            {
                Stop();
                return;
            }

            if (_waveControl.CanStartNextWave
                && _waveControl.SecondsUntilNextWave <= _config.WaveBreakSeconds
                && _enemies.Enemies.Any(TutorialEnemies.IsTargetable))
                _countdownHold = _waveControl.HoldCountdown();
        }

        public void Dispose()
            => Stop();

        private void ReleaseCountdown()
        {
            var hold = _countdownHold;
            _countdownHold = null;
            hold?.Dispose();
        }

        private void Stop()
        {
            _isActive = false;
            _signals.Disable();
            ReleaseCountdown();
        }
    }
}
