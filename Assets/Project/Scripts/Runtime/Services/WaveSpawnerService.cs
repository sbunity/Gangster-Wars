using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class WaveSpawnerService : IWaveSpawnerService
    {
        private readonly IGameFactory _gameFactory;
        private readonly List<Tween> _tweens = new List<Tween>();
        private bool _isStopped;

        public WaveSpawnerService(IGameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public async UniTask SpawnAsync(Level level, IReadOnlyList<Transform> spawnPoints, IReadOnlyList<Transform> targetPoints)
        {
            _isStopped = false;
            for (var waveIndex = 0; waveIndex < level.Waves.Count; waveIndex++)
            {
                var wave = level.Waves[waveIndex];
                await UniTask.Delay(System.TimeSpan.FromSeconds(wave.StartDelay));

                if (_isStopped)
                    return;

                foreach (var enemyOfWave in wave.Enemies)
                {
                    for (var index = 0; index < enemyOfWave.CountEnemy; index++)
                    {
                        if (_isStopped)
                            return;

                        var path = Random.Range(0, spawnPoints.Count);
                        _gameFactory.CreateEnemy(enemyOfWave, spawnPoints[path], targetPoints[path]);
                        await UniTask.Delay(System.TimeSpan.FromSeconds(enemyOfWave.Interval));
                    }
                }

                await UniTask.Delay(System.TimeSpan.FromSeconds(wave.Delay));
            }
        }

        public void Stop()
        {
            _isStopped = true;
            foreach (var tween in _tweens)
                tween?.Kill();

            _tweens.Clear();
        }
    }
}