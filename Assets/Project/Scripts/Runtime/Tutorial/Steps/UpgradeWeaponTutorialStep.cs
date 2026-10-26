using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class UpgradeWeaponTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly IPlayerProgressService _progressService;
        private readonly ITutorialProgressService _tutorialProgress;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;

        public UpgradeWeaponTutorialStep(
            SignalBus signalBus,
            IPlayerProgressService progressService,
            ITutorialProgressService tutorialProgress,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _progressService = progressService;
            _tutorialProgress = tutorialProgress;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.UpgradeWeapon;
        public TutorialStepScope Scope => TutorialStepScope.Scene;

        private int WeaponId => (int)_config.UpgradeWeapon;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            while (!IsUpgraded() && !CanAffordUpgrade())
                await _signalBus.WaitAsync<CoinsChangedSignal>(cancellationToken);
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (IsUpgraded())
                return true;

            if (!CanAffordUpgrade())
                return false;

            var button = await _targets.WaitForAsync(new TutorialTargetKey(TutorialTargetId.WeaponUpgradeButton, WeaponId), cancellationToken);
            var hint = new TutorialHint(button, _config.UpgradeCaption, TutorialGesture.Tap, TutorialInputMode.TargetOnly, true);
            await _view.ShowUntilAsync(hint, _signalBus.WaitAsync<ProgressUpgradedSignal>(cancellationToken, _ => IsUpgraded()));
            return true;
        }

        public void Dispose()
        {
        }

        private bool CanAffordUpgrade()
            => _tutorialProgress.IsTutorialLevelCompleted
               && _progressService.TryGetNextWeaponUpgradePrice(WeaponId, out var price)
               && _progressService.CanBuy(price);

        private bool IsUpgraded()
            => _progressService.GetWeaponShortInfo(WeaponId)?.UpgradeId >= 0;
    }
}
