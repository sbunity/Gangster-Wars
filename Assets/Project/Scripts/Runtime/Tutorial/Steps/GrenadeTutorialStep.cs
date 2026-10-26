using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class GrenadeTutorialStep : ITutorialStep
    {
        private static readonly TutorialTargetKey[] GrenadeSlots = Enum.GetValues(typeof(GrenadesName))
            .Cast<GrenadesName>()
            .Where(grenade => grenade != GrenadesName.None)
            .Select(TutorialTargetKey.GrenadeSlot)
            .ToArray();

        private readonly SignalBus _signalBus;
        private readonly IPlayerProgressService _progressService;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private readonly SignalSubscriptions _signals;
        private EnemyControllerBase _enemy;
        private bool _isThrown;

        public GrenadeTutorialStep(
            SignalBus signalBus,
            IPlayerProgressService progressService,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _progressService = progressService;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GrenadeThrownSignal>(() => _isThrown = true);
        }

        public TutorialStepId Id => TutorialStepId.Grenade;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            GrantGiftGrenades();
            _signals.Enable();
            _enemy = await TutorialEnemies.WaitForSpawnedEnemyAsync(_signalBus, _config.GrenadeDelayAfterSpawn, () => _isThrown, cancellationToken);
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (_isThrown)
                return true;

            var enemy = _enemy;
            if (!TutorialEnemies.IsTargetable(enemy) || !_targets.TryGetFirst(GrenadeSlots, out var grenadeSlot))
                return false;

            var destination = new WorldTutorialAnchor(enemy.Center.GetTransform(), Camera.main, _config.EnemyTargetWorldSize);
            var hint = new TutorialHint(grenadeSlot, _config.GrenadeCaption, TutorialGesture.HoldAndDrag, TutorialInputMode.PassThrough, true, destination);
            var deadline = Time.unscaledTime + _config.GrenadeHintMaxDuration;

            var timeScale = new TutorialTimeScale();
            try
            {
                timeScale.SlowDown(_config.GrenadeTimeScale);
                var until = UniTask.WaitUntil(() => _isThrown || !TutorialEnemies.IsTargetable(enemy) || Time.unscaledTime >= deadline, cancellationToken: cancellationToken);
                await _view.ShowUntilAsync(hint, until);
            }
            finally
            {
                timeScale.Restore();
            }

            return _isThrown || TutorialEnemies.IsTargetable(enemy);
        }

        public void Dispose()
            => _signals.Disable();

        private void GrantGiftGrenades()
        {
            if (_progressService.CurrentLevelId != _config.GrenadeLevelId)
                return;

            var grenadeId = (int)_config.GiftGrenade;
            var grenade = _progressService.GetGrenadeShortInfo(grenadeId);
            if (grenade == null)
                return;

            for (var missing = _config.GiftGrenadeCount - Mathf.Max(0, grenade.Count); missing > 0; missing--)
                _progressService.BuyGrenade(grenadeId, true);
        }
    }
}
