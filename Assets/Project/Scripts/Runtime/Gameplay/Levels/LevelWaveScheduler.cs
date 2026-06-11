using System;
using DG.Tweening;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelWaveScheduler
    {
        private readonly Action<EnemyOfWave> _spawnEnemy;
        private Tween _waveDelayTween;
        private Tween _nextWaveTween;
        private Tween _enemySpawnTween;
        private Tween _nextEnemyGroupTween;
        private Level _level;
        private bool _isStopped;

        public LevelWaveScheduler(Action<EnemyOfWave> spawnEnemy)
        {
            _spawnEnemy = spawnEnemy;
        }

        public int CurrentWave { get; private set; }
        public bool IsWaveFull { get; private set; }

        public void Initialize(Level level)
        {
            Stop();
            _level = level;
            CurrentWave = 0;
            IsWaveFull = false;
            _isStopped = false;
        }

        public void Wave()
        {
            if (_isStopped || _level == null)
                return;

            if (CurrentWave < _level.Waves.Count)
            {
                IsWaveFull = false;
                _waveDelayTween = DOVirtual.DelayedCall(_level.Waves[CurrentWave].StartDelay, () =>
                {
                    if (!_isStopped)
                        StartWave(CurrentWave);
                }).SetUpdate(false);
            }
        }

        public void StartWave(int waveIndex)
        {
            if (_isStopped || _level == null || waveIndex < 0 || waveIndex >= _level.Waves.Count)
                return;

            _nextWaveTween = DOVirtual.DelayedCall(_level.Waves[waveIndex].Delay, () =>
            {
                if (_isStopped)
                    return;

                CurrentWave++;
                Wave();
            }).SetUpdate(false);

            WaveHandler(waveIndex, 0);
        }

        public void WaveHandler(int waveIndex, int currentEnemyIndex)
        {
            if (_isStopped || _level == null || waveIndex < 0 || waveIndex >= _level.Waves.Count)
                return;

            var wave = _level.Waves[waveIndex];
            if (currentEnemyIndex < 0 || currentEnemyIndex >= wave.Enemies.Count)
                return;

            IsWaveFull = false;
            var enemyOfWave = wave.Enemies[currentEnemyIndex];
            _enemySpawnTween = DOVirtual.DelayedCall(0, () =>
            {
                if (!_isStopped)
                    _spawnEnemy?.Invoke(enemyOfWave);
            }).SetLoops(enemyOfWave.CountEnemy).OnComplete(() =>
            {
                if (_isStopped)
                    return;

                var nextEnemy = currentEnemyIndex + 1;
                IsWaveFull = nextEnemy == wave.Enemies.Count;
                if (!IsWaveFull)
                {
                    _nextEnemyGroupTween = DOVirtual.DelayedCall(wave.Enemies[nextEnemy].Interval, () =>
                    {
                        WaveHandler(waveIndex, nextEnemy);
                    }).SetUpdate(false);
                }
            }).SetUpdate(false);
        }

        public void Stop()
        {
            _isStopped = true;
            _waveDelayTween?.Kill();
            _nextWaveTween?.Kill();
            _enemySpawnTween?.Kill();
            _nextEnemyGroupTween?.Kill();
        }
    }
}
