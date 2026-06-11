using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelService
    {
        Level CurrentLevel { get; }

        int CurrentWave { get; }

        bool IsFinished { get; }

        void Initialize(Level level);
        void Finish(Panels panel);
    }

    public interface ILevelFlowService
    {
        event Action<Panels> Finished;
        Panels LastPanel { get; }

        bool IsFinished { get; }

        void Start(Level level);
        void Finish(Panels panel);
    }

    public interface IWaveSpawnerService
    {
        UniTask SpawnAsync(Level level, IReadOnlyList<Transform> spawnPoints, IReadOnlyList<Transform> targetPoints);
        void Stop();
    }

    public interface ILevelRuntimeService
    {
        Transform GetRandomEnemy();
        void SpawnBullet(int bulletId, int damage = 0, Vector3 position = default, Vector3 target = default, float offset = 0, string tag = "Bullet");
        void SpawnGrenadeOnPlace(GrenadesName grenade, Vector3 position);
        void SpawnCollision(int collisionId, Vector3 position, Grenade properties = null);
        void SpawnBonus(Vector3 position);
    }

    public interface IAimService
    {
        Vector2 CurrentAimPosition { get; }
    }
}