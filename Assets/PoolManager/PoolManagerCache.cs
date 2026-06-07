using UnityEngine;
using System;
using System.Collections.Generic;

// Pool module v 1.2.0

[Serializable]
public class PoolManagerCache
{
    public Dictionary<string, List<PoolCache>> poolsCache = new Dictionary<string, List<PoolCache>>();

    public List<PoolCache> GetPoolCache(string levelId)
    {
        if (poolsCache.ContainsKey(levelId))
        {
            return poolsCache[levelId];
        }
        else
        {
            return null;
        }
    }

    public bool IsEmpty()
    {
        return poolsCache.Count == 0 ? true : false;
    }

    public bool ContainsLevel(string levelId)
    {
        return poolsCache.ContainsKey(levelId) ? true : false;
    }

    public void UpdateCache(string levelId, List<PoolCache> poolCache)
    {
        poolsCache[levelId] = poolCache;
    }
}
