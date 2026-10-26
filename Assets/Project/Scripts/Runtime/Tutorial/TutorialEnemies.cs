using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Tutorial
{
    public static class TutorialEnemies
    {
        public static bool IsTargetable(EnemyControllerBase enemy)
            => enemy != null && enemy.IsAlive && enemy.Center != null;

        public static async UniTask<EnemyControllerBase> WaitForSpawnedEnemyAsync(
            SignalBus signalBus,
            float secondsOnField,
            Func<bool> isAlreadyDone,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var enemy = (await signalBus.WaitAsync<EnemySpawnedSignal>(cancellationToken)).Enemy;
                var triggerTime = Time.time + secondsOnField;
                await UniTask.WaitUntil(() => isAlreadyDone() || !IsTargetable(enemy) || Time.time >= triggerTime, cancellationToken: cancellationToken);

                if (isAlreadyDone() || IsTargetable(enemy))
                    return enemy;
            }
        }
    }
}
