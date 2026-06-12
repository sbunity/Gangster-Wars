using System.Collections.Generic;
using UnityEngine;
using SBabchuk.Runtime.Gameplay.Bonuses;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelEntityTracker
    {
        private readonly List<EnemyControllerBase> _enemies = new();
        private readonly List<BonusController> _bonuses = new();

        public int EnemyCount => _enemies.Count;
        public int BonusCount => _bonuses.Count;

        public void Clear()
        {
            _enemies.Clear();
            _bonuses.Clear();
        }

        public void AddEnemy(EnemyControllerBase enemy)
        {
            if (enemy != null && !_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void RemoveEnemy(int enemyId)
        {
            _enemies.Remove(_enemies.Find(enemy => enemy != null && enemy.Properties != null && enemy.Properties.Id == enemyId));
        }

        public void AddBonus(BonusController bonus)
        {
            if (bonus != null && !_bonuses.Contains(bonus))
                _bonuses.Add(bonus);
        }

        public void RemoveBonus(BonusController bonus)
        {
            _bonuses.Remove(bonus);
        }

        public Transform GetRandomEnemy()
        {
            RemoveMissingEnemies();
            if (_enemies.Count == 0)
                return null;

            var enemy = _enemies[Random.Range(0, _enemies.Count)];
            return enemy != null && enemy.Center != null ? enemy.Center.GetTransform() : null;
        }

        private void RemoveMissingEnemies()
        {
            _enemies.RemoveAll(enemy => enemy == null || enemy.Properties == null);
        }
    }
}
