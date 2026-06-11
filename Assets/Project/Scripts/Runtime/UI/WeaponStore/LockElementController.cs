using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class LockElementController : LockElementControllerBase
    {
        private Weapon weaponInfo;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public override void Initialisation(int _id)
        {
            Id = _id;
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
