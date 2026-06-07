using UnityEngine;

// Pool module v 1.2.0

// not shure we should use STRING for pool comparsion and identifying

[System.Serializable]
public class PoolCache
{
    public string poolName;

    public int poolSize;

    public int updatesCount;

    public PoolCache(string poolName, int poolSize)
    {
        this.poolName = poolName;
        this.poolSize = poolSize;
        updatesCount = 1;
    }

    public void UpdateSize(int newSizeValue)
    {
        Debug.Log("## updating cache for " + poolName + " old value: " + poolSize + " new val: " + newSizeValue + " updates count: " + updatesCount);
        poolSize = (poolSize * updatesCount + newSizeValue) / (updatesCount + 1);
        updatesCount++;
        Debug.Log("## new size: " + poolSize + " updates count:" + updatesCount);
    }
}
