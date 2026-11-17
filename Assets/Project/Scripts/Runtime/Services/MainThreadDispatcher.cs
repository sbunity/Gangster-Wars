using System;
using System.Collections.Concurrent;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class MainThreadDispatcher : IMainThreadDispatcher, ITickable
    {
        private readonly ConcurrentQueue<Action> _queue = new();

        public void Dispatch(Action callback)
        {
            _queue.Enqueue(callback);
        }

        public void Tick()
        {
            while (_queue.TryDequeue(out var callback))
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}
