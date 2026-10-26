using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class BonusTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private BonusController _bonus;

        public BonusTutorialStep(SignalBus signalBus, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.Bonus;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => _bonus = (await _signalBus.WaitAsync<BonusSpawnedSignal>(cancellationToken)).Bonus;

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            var bonus = _bonus;
            if (!IsWaitingForCollection(bonus))
                return false;

            var anchor = new WorldTutorialAnchor(bonus.transform, Camera.main, _config.BonusTargetWorldSize);
            var hint = new TutorialHint(anchor, _config.BonusCaption, TutorialGesture.Tap, TutorialInputMode.PassThrough, false);
            await _view.ShowUntilAsync(hint, UniTask.WaitUntil(() => !IsWaitingForCollection(bonus), cancellationToken: cancellationToken));

            var slotKey = SlotOf(bonus.Reward);
            await UniTask.WaitUntil(() => !bonus.gameObject.activeInHierarchy, cancellationToken: cancellationToken);

            if (slotKey.HasValue && _targets.TryGet(slotKey.Value, out var slot))
            {
                var slotHint = new TutorialHint(slot, null, TutorialGesture.Point, TutorialInputMode.PassThrough, false);
                await _view.ShowForAsync(slotHint, _config.BonusSlotHintDuration, cancellationToken);
            }

            return true;
        }

        public void Dispose()
        {
        }

        private static bool IsWaitingForCollection(BonusController bonus)
            => bonus != null && bonus.gameObject.activeInHierarchy && !bonus.IsCollecting;

        private static TutorialTargetKey? SlotOf(BonusReward reward)
        {
            if (reward.Weapon != WeaponsName.None)
                return TutorialTargetKey.WeaponSlot(reward.Weapon);

            if (reward.Grenade != GrenadesName.None)
                return TutorialTargetKey.GrenadeSlot(reward.Grenade);

            return null;
        }
    }
}
