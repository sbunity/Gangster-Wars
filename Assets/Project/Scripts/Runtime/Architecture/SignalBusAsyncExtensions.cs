using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace SBabchuk.Runtime.Architecture
{
    public static class SignalBusAsyncExtensions
    {
        public static async UniTask<TSignal> WaitAsync<TSignal>(this SignalBus signalBus, CancellationToken cancellationToken, Func<TSignal, bool> predicate = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var completion = new UniTaskCompletionSource<TSignal>();
            Action<TSignal> handler = signal =>
            {
                if (predicate == null || predicate(signal))
                    completion.TrySetResult(signal);
            };

            signalBus.Subscribe(handler);
            try
            {
                using (cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken)))
                    return await completion.Task;
            }
            finally
            {
                signalBus.Unsubscribe(handler);
            }
        }
    }
}
