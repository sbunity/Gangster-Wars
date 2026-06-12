using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.DefenseStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LockDElementController : LockElementControllerBase
    {
        private Defense defenceInfo;

        public override void Initialisation(int id)
        {
            Id = id;
            var defenceStore = _assetProvider.DefenseStoreDatabase;
            defenceInfo = defenceStore.GetDefense(Id);

            if (PriceBuy)
                PriceBuy.text = defenceInfo.Price.ToString();

            if (BttnBuy)
                BttnBuy.interactable = _progressService.CanBuy(defenceInfo.Price);
        }

        public override void Buy()
        {
            _progressService.BuyDefence(Id);
        }
    }
}
