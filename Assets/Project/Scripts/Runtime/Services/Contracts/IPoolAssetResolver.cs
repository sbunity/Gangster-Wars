namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPoolAssetResolver
    {
        string GetPoolName(NamesPool pool, int id);
        string GetPrefabResourcesPath(NamesPool pool, int id);
    }
}
