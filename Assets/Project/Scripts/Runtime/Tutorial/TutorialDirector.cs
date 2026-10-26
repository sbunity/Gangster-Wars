using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Tutorial
{
    public sealed class TutorialDirector : IInitializable, IDisposable
    {
        private readonly IReadOnlyList<ITutorialStep> _steps;
        private readonly ITutorialProgressService _progress;
        private readonly SignalSubscriptions _signals;
        private readonly List<ITutorialStep> _ready = new();
        private readonly CancellationTokenSource _sceneLifetime = new();
        private readonly CancellationTokenSource _battleLifetime;
        private bool _isProcessing;
        private bool _isDisposed;

        public TutorialDirector(List<ITutorialStep> steps, ITutorialProgressService progress, SignalBus signalBus)
        {
            _steps = steps.OrderBy(step => step.Id).ToList();
            _progress = progress;
            _battleLifetime = CancellationTokenSource.CreateLinkedTokenSource(_sceneLifetime.Token);
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        public void Initialize()
        {
            _signals.Enable();

            foreach (var step in _steps)
            {
                if (!_progress.IsCompleted(step.Id))
                    WatchAsync(step).Forget();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _signals.Disable();
            _sceneLifetime.Cancel();
            _ready.Clear();

            foreach (var step in _steps)
                step.Dispose();

            _battleLifetime.Dispose();
            _sceneLifetime.Dispose();
        }

        private void OnGameFinished()
        {
            if (_battleLifetime.IsCancellationRequested)
                return;

            _battleLifetime.Cancel();
            _ready.RemoveAll(IsBattleStep);

            foreach (var step in _steps.Where(IsBattleStep))
                step.Dispose();
        }

        private async UniTaskVoid WatchAsync(ITutorialStep step)
        {
            var cancellationToken = TokenFor(step);

            try
            {
                await step.WaitForTriggerAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            Enqueue(step);
        }

        private void Enqueue(ITutorialStep step)
        {
            if (_isDisposed || TokenFor(step).IsCancellationRequested || _ready.Contains(step))
                return;

            _ready.Add(step);
            _ready.Sort((left, right) => left.Id.CompareTo(right.Id));

            if (!_isProcessing)
                ProcessAsync().Forget();
        }

        private async UniTaskVoid ProcessAsync()
        {
            _isProcessing = true;

            try
            {
                await UniTask.Yield(PlayerLoopTiming.Update, _sceneLifetime.Token);

                while (_ready.Count > 0)
                {
                    var step = _ready[0];
                    _ready.RemoveAt(0);
                    await RunAsync(step);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async UniTask RunAsync(ITutorialStep step)
        {
            var cancellationToken = TokenFor(step);
            if (cancellationToken.IsCancellationRequested)
                return;

            bool isCompleted;
            try
            {
                isCompleted = await step.RunAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (isCompleted)
                _progress.Complete(step.Id);
            else
                WatchAsync(step).Forget();
        }

        private CancellationToken TokenFor(ITutorialStep step)
            => _isDisposed
                ? new CancellationToken(true)
                : IsBattleStep(step) ? _battleLifetime.Token : _sceneLifetime.Token;

        private static bool IsBattleStep(ITutorialStep step)
            => step.Scope == TutorialStepScope.Battle;
    }
}
