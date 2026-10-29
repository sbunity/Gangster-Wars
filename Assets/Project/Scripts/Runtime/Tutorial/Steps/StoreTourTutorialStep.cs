using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class StoreTourTutorialStep : ITutorialStep
    {
        private readonly ITutorialProgressService _tutorialProgress;
        private readonly ITutorialTargetRegistry _targets;
        private readonly ITutorialView _view;
        private readonly IStoreTabs _storeTabs;
        private readonly TutorialConfig _config;

        public StoreTourTutorialStep(
            ITutorialProgressService tutorialProgress,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IStoreTabs storeTabs,
            IAssetProvider assetProvider)
        {
            _tutorialProgress = tutorialProgress;
            _targets = targets;
            _view = view;
            _storeTabs = storeTabs;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.StoreTour;
        public TutorialStepScope Scope => TutorialStepScope.Scene;

        public UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => _tutorialProgress.IsLevelCompleted(_config.StoreTourLevelId) ? UniTask.CompletedTask : UniTask.Never(cancellationToken);

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            var initialTab = _storeTabs.Current;
            try
            {
                foreach (var page in _config.StoreTourPages)
                    await ShowPageAsync(page, cancellationToken);
            }
            finally
            {
                _view.Hide();
            }

            _storeTabs.Select(initialTab);
            return true;
        }

        public void Dispose()
        {
        }

        private async UniTask ShowPageAsync(StoreTourPage page, CancellationToken cancellationToken)
        {
            if (page.Tab != StoreTab.None)
                _storeTabs.Select(page.Tab);

            var target = await _targets.WaitForAsync(page.Target, cancellationToken);
            _targets.TryGet(TutorialTargetKey.StoreTabButton(page.Tab), out var tabButton);
            _view.Show(new TutorialHint(target, page.Caption, TutorialGesture.None, TutorialInputMode.TapToContinue, true, captionAnchor: tabButton));
            await UniTask.Delay(TimeSpan.FromSeconds(_config.StoreTourMinPageDuration), true, cancellationToken: cancellationToken);
            await _view.WaitForTapAsync(cancellationToken);
        }
    }
}
