using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPoolService
    {
        Pool GetPool(NamesPool pool, int id);
        GameObject Get(NamesPool pool, int id, bool activate = false);
        T Get<T>(NamesPool pool, int id, bool activate = false) where T : Component;
        string GetPoolName(NamesPool pool, int id);
    }
}
