using SBabchuk.Runtime.UI.WeaponStore.Info;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IMainPlayerStatsService
    {
        StoreItemStatsSnapshot GetStats(int personageId, bool includeUpgradeDelta);
    }
}
