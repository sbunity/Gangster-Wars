using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class PoolService : IPoolService
    {
        private readonly PoolManager _poolManager;
        private readonly IPoolAssetResolver _poolAssetResolver;

        public PoolService(PoolManager poolManager, IPoolAssetResolver poolAssetResolver)
        {
            _poolManager = poolManager;
            _poolAssetResolver = poolAssetResolver;
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
            => _poolAssetResolver.GetPoolName(pool, id);
    }
}
