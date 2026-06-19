using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class EnemyInfoService : IEnemyInfoService
    {
        private readonly IAssetProvider _assetProvider;

        public EnemyInfoService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public EnemyInfoSnapshot GetInfo(int enemyId)
        {
            var database = _assetProvider.EnemyDatabase;
            var enemy = database.GetEnemy(enemyId);
            if (enemy == null)
                return EmptySnapshot();

            var stats = new List<EnemyInfoStatSnapshot>
            {
                new("Health", enemy.Health, GetMax(database, item => item.Health)),
                new("Damage", enemy.Damage, GetMax(database, item => item.Damage)),
                new("Move Speed", enemy.SpeedMove, GetMax(database, item => item.SpeedMove)),
                new("Attack Rate", ToAttackRate(enemy.AttackSpeed), GetMax(database, item => ToAttackRate(item.AttackSpeed))),
                new("Attack Range", enemy.AttackRadius, GetMax(database, item => item.AttackRadius)),
                new("Reward", enemy.Gold, GetMax(database, item => item.Gold))
            };

            return new EnemyInfoSnapshot(
                enemy.DisplayName,
                GetKindLabel(enemy.Kind),
                enemy.Description,
                enemy.Icon,
                stats);
        }

        private EnemyInfoSnapshot EmptySnapshot()
            => new("Unknown", string.Empty, string.Empty, null, new List<EnemyInfoStatSnapshot>());

        private static string GetKindLabel(EnemyKind kind)
            => kind == EnemyKind.Boss ? "Boss" : "Soldier";

        private static float GetMax(EnemyDatabase database, System.Func<Enemy, float> selector)
        {
            var max = 1f;
            foreach (var enemy in database.Enemies)
            {
                if (enemy != null)
                    max = Mathf.Max(max, selector(enemy));
            }

            return max;
        }

        private static float ToAttackRate(float attackDelay)
            => attackDelay <= 0.001f ? 0f : 1f / attackDelay;
    }
}
