using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SBabchuk.Runtime.Architecture
{
    public sealed class CoalescingTaskRunner
    {
        private readonly Func<UniTask> _task;

        private bool _isRunning;
        private bool _hasPendingRun;

        public CoalescingTaskRunner(Func<UniTask> task)
        {
            _task = task;
        }

        public void Run()
        {
            if (_isRunning)
            {
                _hasPendingRun = true;
                return;
            }

            RunLoopAsync().Forget();
        }

        private async UniTaskVoid RunLoopAsync()
        {
            _isRunning = true;

            do
            {
                _hasPendingRun = false;

                try
                {
                    await _task();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            while (_hasPendingRun);

            _isRunning = false;
        }
    }
}
