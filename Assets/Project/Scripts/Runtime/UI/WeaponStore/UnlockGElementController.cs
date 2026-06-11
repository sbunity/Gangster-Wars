using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class UnlockGElementController : LockElementControllerBase
    {
        private Grenade _grenadeInfo;

        public override void Initialisation(int _id)
        {
            Id = _id;
            var bombStore = _assetProvider.BombStoreDatabase;
            _grenadeInfo = bombStore.GetGrenade(Id);
            
            if (PriceBuy)
                PriceBuy.text = _grenadeInfo.Price.ToString();

            if (BttnBuy)
                BttnBuy.interactable = _progressService.CanBuy(_grenadeInfo.Price);
        }

        public override void Buy()
        {
            _progressService.BuyGrenade(Id);
        }
    }
}
