using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Tutorial
{
    public static class TutorialViewExtensions
    {
        public static async UniTask ShowUntilAsync(this ITutorialView view, TutorialHint hint, UniTask until)
        {
            view.Show(hint);
            try
            {
                await until;
            }
            finally
            {
                view.Hide();
            }
        }

        public static UniTask ShowForAsync(this ITutorialView view, TutorialHint hint, float seconds, CancellationToken cancellationToken, bool ignoreTimeScale = false)
            => view.ShowUntilAsync(hint, UniTask.Delay(TimeSpan.FromSeconds(seconds), ignoreTimeScale, cancellationToken: cancellationToken));
    }
}
