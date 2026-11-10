using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.UI.Intro
{
    public sealed class SceneIntro : ISceneIntro, IInitializable, IDisposable
    {
        private readonly IUIIntroView _view;
        private readonly ISceneTransitionService _sceneTransition;
        private readonly UniTaskCompletionSource _completion = new();
        private readonly CancellationTokenSource _lifetime = new();

        public SceneIntro(IUIIntroView view, ISceneTransitionService sceneTransition)
        {
            _view = view;
            _sceneTransition = sceneTransition;
        }

        public void Initialize()
        {
            _view.Hide();
            PlayAsync(_lifetime.Token).Forget();
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        public UniTask WaitForCompletionAsync(CancellationToken cancellationToken)
            => _completion.Task.AttachExternalCancellation(cancellationToken);

        private async UniTaskVoid PlayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _sceneTransition.WaitForSceneRevealAsync(cancellationToken);
                await _view.PlayAsync(cancellationToken);
                _completion.TrySetResult();
            }
            catch (OperationCanceledException)
            {
                _completion.TrySetCanceled();
            }
        }
    }
}
