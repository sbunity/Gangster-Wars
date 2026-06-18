using SBabchuk.Runtime.UI.WeaponStore.Info;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IStoreItemStatsService
    {
        StoreItemStatsSnapshot GetStats(StoreInfoItemType itemType, int itemId);
    }
}
