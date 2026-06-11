using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class PoolService : IPoolService
    {
        private readonly PoolManager _poolManager;

        public PoolService(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public Pool GetPool(NamesPool pool, int id)
        {
            if (_poolManager == null || _poolManager.pools == null)
                return null;

            var poolName = GetPoolName(pool, id);
            for (var index = 0; index < _poolManager.pools.Count; index++)
            {
                var targetPool = _poolManager.pools[index];
                if (targetPool != null && targetPool.name == poolName)
                    return targetPool;
            }

            Debug.LogError("Not found pool with name: '" + poolName + "'");
            return null;
        }

        public GameObject Get(NamesPool pool, int id, bool activate = false)
        {
            var targetPool = GetPool(pool, id);
            if (targetPool == null)
                return null;

            return targetPool.GetPooledObject(activate);
        }

        public T Get<T>(NamesPool pool, int id, bool activate = false) where T : Component
        {
            var instance = Get(pool, id, activate);
            return instance != null ? instance.GetComponent<T>() : null;
        }

        public string GetPoolName(NamesPool pool, int id)
        {
            switch (pool)
            {
                case NamesPool.Enemies:
                    return "Enemy_" + (id + 1);
                case NamesPool.Grenades:
                    return "Grenade_" + (id + 1);
                case NamesPool.Collisions:
                    return "Collision_" + (id + 1);
                case NamesPool.Bonuses:
                    return "Bonus_" + (id + 1);
                case NamesPool.Bullets:
                    return "Bullet_" + (id + 1);
                default:
                    return "Enemy_" + (id + 1);
            }
        }
    }
}
