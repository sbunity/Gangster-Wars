using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    public sealed class UIIntroView : MonoBehaviour, IUIIntroView
    {
        [SerializeField] private UIIntroStyle _style;
        [SerializeField, Min(0f)] private float _startDelay = 0.1f;
        [SerializeField, Min(0f)] private float _stagger = 0.05f;
        [SerializeField] private UIIntroAnimation[] _animations = Array.Empty<UIIntroAnimation>();

        private Sequence _sequence;

        public void Hide()
        {
            foreach (var animation in _animations)
                animation.Hide(_style);
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            _sequence?.Kill();

            var sequence = _sequence = CreateSequence();
            var completion = new UniTaskCompletionSource();
            sequence.OnKill(() => completion.TrySetResult());

            using (cancellationToken.Register(() => sequence.Kill()))
                await completion.Task;

            cancellationToken.ThrowIfCancellationRequested();
        }

        private Sequence CreateSequence()
        {
            var sequence = DOTween.Sequence().SetUpdate(true);

            for (var i = 0; i < _animations.Length; i++)
            {
                var animation = _animations[i];
                sequence.Insert(_startDelay + i * _stagger + animation.ExtraDelay, animation.Play(_style));
            }

            return sequence;
        }

        private void OnDestroy() => _sequence?.Kill();

        [ContextMenu("Collect Child Animations")]
        private void CollectChildAnimations()
            => _animations = GetComponentsInChildren<UIIntroAnimation>(true);
    }
}
