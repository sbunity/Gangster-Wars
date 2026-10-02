using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.BombStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class UnlockGElementController : LockElementControllerBase
    {
        private Grenade _grenadeInfo;

        public override void Initialisation(int id)
        {
            Id = id;
            var bombStore = _assetProvider.BombStoreDatabase;
            _grenadeInfo = bombStore.GetGrenade(Id);

            StoreElementView.ApplyPrice(PriceBuy, BttnBuy, _grenadeInfo?.Price, _progressService);
        }

        public override void Buy()
        {
            _progressService.BuyGrenade(Id);
        }
    }
}
