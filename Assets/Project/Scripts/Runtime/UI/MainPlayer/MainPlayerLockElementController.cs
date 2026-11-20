using SBabchuk.Runtime.Services.Models;
using SBabchuk.Runtime.UI.WeaponStore;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public class MainPlayerLockElementController : StoreItemLockElementControllerBase
    {
        protected override StoreItemType ItemType => StoreItemType.Personage;
    }
}
