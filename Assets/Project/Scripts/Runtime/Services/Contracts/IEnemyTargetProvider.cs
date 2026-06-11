using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    /// <summary>
    /// Query side of the level runtime: lets combatants ask the level for a target.
    /// </summary>
    public interface IEnemyTargetProvider
    {
        Transform GetRandomEnemy();
    }
}
