using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class LockDElementController : LockElementControllerBase
    {
        private Defense defenceInfo;
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
