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

            if (PriceBuy)
                PriceBuy.text = weaponInfo.Price.ToString();

            if (BttnBuy)
                BttnBuy.interactable = _progressService.CanBuy(weaponInfo.Price);
        }

        public override void Buy()
        {
            _progressService.BuyWeapon(Id);
        }
    }
}
