using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SBabchuk.Runtime.Databases.Levels;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelWaveScheduler
    {
        private readonly Action<EnemyOfWave> _spawnEnemy;
        private Tween _waveDelayTween;
        private Tween _nextWaveTween;
        private Tween _enemySpawnTween;
        private Tween _nextEnemyGroupTween;
        private readonly List<Tween> _activeTweens = new List<Tween>();
        private bool[] _startedWaves;
        private bool[] _completedWaves;
        private Level _level;
        private bool _isStopped;

        public LevelWaveScheduler(Action<EnemyOfWave> spawnEnemy)
        {
            _spawnEnemy = spawnEnemy;
        }

        public int CurrentWave { get; private set; }
        public bool IsWaveFull { get; private set; }
        public bool CanStartNextWave => !_isStopped
                                        && _level != null
                                        && CurrentWave < _level.Waves.Count
                                        && (_startedWaves == null || !_startedWaves[CurrentWave]);

        public void Initialize(Level level)
        {
            Stop();
            _level = level;
            CurrentWave = 0;
            IsWaveFull = false;
            _isStopped = false;
            _startedWaves = new bool[level?.Waves.Count ?? 0];
            _completedWaves = new bool[level?.Waves.Count ?? 0];
        }

        public void Wave()
        {
            if (_isStopped || _level == null)
                return;

            if (CanStartNextWave)
            {
                IsWaveFull = false;
                var waveIndex = CurrentWave;
                _waveDelayTween = Track(DOVirtual.DelayedCall(_level.Waves[waveIndex].StartDelay, () =>
                {
                    if (!_isStopped)
                        StartWave(waveIndex);
                }).SetUpdate(false));
            }
        }

        public bool StartNextWave()
        {
            if (!CanStartNextWave)
                return false;

            StartWave(CurrentWave);
            return true;
        }

        public void StartWave(int waveIndex)
        {
            if (_isStopped || _level == null || waveIndex < 0 || waveIndex >= _level.Waves.Count)
                return;

            if (_startedWaves != null && _startedWaves[waveIndex])
                return;

            if (_startedWaves != null)
                _startedWaves[waveIndex] = true;
            if (_completedWaves != null)
                _completedWaves[waveIndex] = false;

            IsWaveFull = false;
            var expectedNextWave = waveIndex + 1;
            if (CurrentWave < expectedNextWave)
                CurrentWave = expectedNextWave;

            _nextWaveTween = Track(DOVirtual.DelayedCall(_level.Waves[waveIndex].Delay, () =>
            {
                if (_isStopped)
                    return;

                if (CurrentWave == expectedNextWave)
                    Wave();
            }).SetUpdate(false));

            WaveHandler(waveIndex, 0);
        }

        public void WaveHandler(int waveIndex, int currentEnemyIndex)
        {
            if (_isStopped || _level == null || waveIndex < 0 || waveIndex >= _level.Waves.Count)
                return;

            var wave = _level.Waves[waveIndex];
            if (currentEnemyIndex < 0)
                return;

            if (currentEnemyIndex >= wave.Enemies.Count)
            {
                CompleteWaveSpawn(waveIndex);
                return;
            }

            IsWaveFull = false;
            var enemyOfWave = wave.Enemies[currentEnemyIndex];
            if (enemyOfWave.CountEnemy <= 0)
            {
                CompleteEnemyGroup(waveIndex, currentEnemyIndex);
                return;
            }

            _enemySpawnTween = Track(DOVirtual.DelayedCall(0, () =>
            {
                if (!_isStopped)
                    _spawnEnemy?.Invoke(enemyOfWave);
            }).SetLoops(enemyOfWave.CountEnemy).OnComplete(() => CompleteEnemyGroup(waveIndex, currentEnemyIndex)).SetUpdate(false));
        }

        private void CompleteEnemyGroup(int waveIndex, int currentEnemyIndex)
        {
            if (_isStopped || _level == null || waveIndex < 0 || waveIndex >= _level.Waves.Count)
                return;

            var wave = _level.Waves[waveIndex];
            var nextEnemy = currentEnemyIndex + 1;
            if (nextEnemy >= wave.Enemies.Count)
            {
                CompleteWaveSpawn(waveIndex);
                return;
            }

            _nextEnemyGroupTween = Track(DOVirtual.DelayedCall(wave.Enemies[nextEnemy].Interval, () =>
            {
                WaveHandler(waveIndex, nextEnemy);
            }).SetUpdate(false));
        }

        private void CompleteWaveSpawn(int waveIndex)
        {
            if (_completedWaves != null && waveIndex >= 0 && waveIndex < _completedWaves.Length)
                _completedWaves[waveIndex] = true;

            IsWaveFull = AreStartedWavesCompleted();
        }

        private bool AreStartedWavesCompleted()
        {
            if (_startedWaves == null || _completedWaves == null)
                return false;

            return !_startedWaves.Where((started, index) => started && !_completedWaves[index]).Any();
        }

        private Tween Track(Tween tween)
        {
            _activeTweens.Add(tween);
            tween.OnKill(() => _activeTweens.Remove(tween));
            return tween;
        }

        public void Stop()
        {
            _isStopped = true;
            for (var i = _activeTweens.Count - 1; i >= 0; i--)
            {
                _activeTweens[i]?.Kill();
            }

            _activeTweens.Clear();
            _waveDelayTween = null;
            _nextWaveTween = null;
            _enemySpawnTween = null;
            _nextEnemyGroupTween = null;
        }
    }
}
