using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LockElementController : LockElementControllerBase
    {
        private Weapon weaponInfo;

        public override void Initialisation(int id)
        {
            Id = id;
            var weaponStore = _assetProvider.WeaponStoreDatabase;
            weaponInfo = weaponStore.GetWeapon(Id);

            StoreElementView.ApplyPrice(PriceBuy, BttnBuy, weaponInfo?.Price, _progressService);
        }

        public override void Buy()
        {
            _progressService.BuyWeapon(Id);
        }
    }
}
