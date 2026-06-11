using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class PoolAssetResolver : IPoolAssetResolver
    {
        public string GetPoolName(NamesPool pool, int id)
        {
            var prefix = GetPrefabPrefix(pool);
            return string.IsNullOrEmpty(prefix) ? string.Empty : prefix + "_" + (id + 1);
        }

        public string GetPrefabResourcesPath(NamesPool pool, int id)
        {
            var folder = GetPrefabFolder(pool);
            var name = GetPoolName(pool, id);
            return string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name)
                ? string.Empty
                : "Prefabs/" + folder + "/" + name;
        }

        private string GetPrefabFolder(NamesPool pool)
            => pool switch
            {
                NamesPool.Enemies => "Enemies",
                NamesPool.Bullets => "Bullets",
                NamesPool.Grenades => "Grenades",
                NamesPool.Collisions => "Collisions",
                NamesPool.Bonuses => "Bonuses",
                _ => string.Empty,
            };

        private string GetPrefabPrefix(NamesPool pool)
            => pool switch
            {
                NamesPool.Enemies => "Enemy",
                NamesPool.Bullets => "Bullet",
                NamesPool.Grenades => "Grenade",
                NamesPool.Collisions => "Collision",
                NamesPool.Bonuses => "Bonus",
                _ => "Enemy",
            };
    }
}
