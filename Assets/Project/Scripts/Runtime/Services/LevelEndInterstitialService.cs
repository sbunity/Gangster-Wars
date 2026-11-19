using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class LevelEndInterstitialService : ILevelEndInterstitialService, IDisposable
    {
        private readonly IAdsService _adsService;
        private readonly AdPlacementConfig _config;
        private readonly bool _isFirstLevel;
        private readonly CancellationTokenSource _lifetime = new();

        private State _state;
        private Action _pendingAction;

        public LevelEndInterstitialService(IAdsService adsService, IAssetProvider assetProvider, IPlayerProgressService progressService, ITutorialProgressService tutorialProgress)
        {
            _adsService = adsService;
            _config = assetProvider.AdPlacementConfig;
            _isFirstLevel = progressService.CurrentLevelId == tutorialProgress.TutorialLevelId;
        }

        private enum State
        {
            Idle,
            Scheduled,
            Showing,
            Finished
        }

        public void ScheduleAfter(UniTask presentation)
        {
            if (_state != State.Idle || _isFirstLevel)
                return;

            _state = State.Scheduled;
            CountdownAsync(presentation, _lifetime.Token).Forget();
        }

        public void RunAfterAd(Action action)
        {
            switch (_state)
            {
                case State.Scheduled:
                    _pendingAction = action;
                    Show();
                    break;
                case State.Showing:
                    _pendingAction ??= action;
                    break;
                default:
                    action();
                    break;
            }
        }

        public void Dispose()
        {
            _state = State.Finished;
            _pendingAction = null;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private async UniTaskVoid CountdownAsync(UniTask presentation, CancellationToken cancellationToken)
        {
            await presentation;
            await UniTask.Delay(TimeSpan.FromSeconds(_config.LevelEndInterstitialDelay), ignoreTimeScale: true, cancellationToken: cancellationToken);

            if (_state == State.Scheduled)
                Show();
        }

        private void Show()
        {
            _state = State.Showing;
            _adsService.ShowInterstitial(OnAdFinished);
        }

        private void OnAdFinished(bool isShown)
        {
            if (_state != State.Showing)
                return;

            _state = State.Finished;

            var action = _pendingAction;
            _pendingAction = null;
            action?.Invoke();
        }
    }
}
