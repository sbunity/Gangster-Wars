using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class MainPlayerLockElementController : LockElementControllerBase
    {
        private Personage _personage;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public override void Initialisation(int personageId)
        {
            Id = personageId;
            var playerStore = _assetProvider.MainPlayerDatabase;
            _personage = playerStore.GetPersonage(Id);
            if (PriceBuy)
            {
                PriceBuy.text = _personage.Price.ToString();
            }

            if (BttnBuy)
            {
                BttnBuy.interactable = _progressService.CanBuy(_personage.Price);
            }
        }

        public override void Buy()
        {
            _progressService.BuyPersonage(Id);
        }
    }
}
