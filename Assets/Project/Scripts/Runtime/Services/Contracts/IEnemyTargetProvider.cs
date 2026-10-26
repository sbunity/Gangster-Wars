using System.Collections.Generic;
using SBabchuk.Runtime.Gameplay.Enemies;
using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IEnemyTargetProvider
    {
        IReadOnlyList<EnemyControllerBase> Enemies { get; }

        Transform GetRandomEnemy();
    }
}
