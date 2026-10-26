using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class SwitchWeaponTutorialStep : ITutorialStep
    {
        private static readonly WeaponsName[] Weapons = Enum.GetValues(typeof(WeaponsName))
            .Cast<WeaponsName>()
            .Where(weapon => weapon != WeaponsName.None)
            .ToArray();

        private readonly SignalBus _signalBus;
        private readonly ILeaderWeaponSelectionService _weaponSelection;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;

        public SwitchWeaponTutorialStep(
            SignalBus signalBus,
            ILeaderWeaponSelectionService weaponSelection,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _weaponSelection = weaponSelection;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.SwitchWeapon;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        private IEnumerable<TutorialTargetKey> OtherWeaponSlots
            => Weapons
                .Where(weapon => weapon != _weaponSelection.CurrentWeapon)
                .Select(TutorialTargetKey.WeaponSlot);

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            await _signalBus.WaitAsync<EnemySpawnedSignal>(cancellationToken);
            await UniTask.WaitUntil(() => _targets.TryGetFirst(OtherWeaponSlots, out _), cancellationToken: cancellationToken);
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (!_targets.TryGetFirst(OtherWeaponSlots, out var slot))
                return false;

            var weapon = _weaponSelection.CurrentWeapon;
            var hint = new TutorialHint(slot, _config.SwitchWeaponCaption, TutorialGesture.Tap, TutorialInputMode.PassThrough, false);
            var deadline = Time.time + _config.SwitchWeaponHintDuration;
            await _view.ShowUntilAsync(hint, UniTask.WaitUntil(() => _weaponSelection.CurrentWeapon != weapon || Time.time >= deadline, cancellationToken: cancellationToken));
            return true;
        }

        public void Dispose()
        {
        }
    }
}
