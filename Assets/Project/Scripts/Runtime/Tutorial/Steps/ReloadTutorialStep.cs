using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class ReloadTutorialStep : ITutorialStep
    {
        private readonly IWeaponAmmoService _ammoService;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;

        public ReloadTutorialStep(IWeaponAmmoService ammoService, ITutorialTargetRegistry targets, ITutorialView view, IAssetProvider assetProvider)
        {
            _ammoService = ammoService;
            _targets = targets;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.Reload;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var completion = new UniTaskCompletionSource();
            Action handler = () => completion.TrySetResult();
            _ammoService.MagazineEmptied += handler;
            try
            {
                using (cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken)))
                    await completion.Task;
            }
            finally
            {
                _ammoService.MagazineEmptied -= handler;
            }
        }

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (IsReloaded())
                return true;

            var magazine = await _targets.WaitForAsync(TutorialTargetId.Magazine, cancellationToken);
            var hint = new TutorialHint(magazine, _config.ReloadCaption, TutorialGesture.Point, TutorialInputMode.PassThrough, false);
            var deadline = Time.time + _config.ReloadMaxDuration;
            await _view.ShowUntilAsync(hint, UniTask.WaitUntil(() => IsReloaded() || Time.time >= deadline, cancellationToken: cancellationToken));
            return true;
        }

        public void Dispose()
        {
        }

        private bool IsReloaded()
            => _ammoService.Current >= Mathf.Min(_config.ReloadCompleteRounds, _ammoService.Capacity);
    }
}
