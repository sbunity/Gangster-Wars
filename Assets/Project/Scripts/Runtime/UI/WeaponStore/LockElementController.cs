using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LockElementController : StoreItemLockElementControllerBase
    {
        protected override StoreItemType ItemType => StoreItemType.Weapon;
    }
}
