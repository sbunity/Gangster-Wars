using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace SBabchuk.Runtime.Architecture
{
    public static class TweenAsyncExtensions
    {
        public static UniTask WaitForCompletionAsync(this Tween tween, CancellationToken cancellationToken)
            => IsPlaying(tween)
                ? UniTask.WaitWhile(() => IsPlaying(tween), cancellationToken: cancellationToken)
                : UniTask.CompletedTask;

        private static bool IsPlaying(Tween tween) => tween != null && tween.IsActive();
    }
}
