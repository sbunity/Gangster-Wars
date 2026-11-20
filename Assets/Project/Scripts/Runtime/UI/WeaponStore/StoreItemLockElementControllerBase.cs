using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public abstract class StoreItemLockElementControllerBase : LockElementControllerBase
    {
        [SerializeField] private StorePriceView _price;

        private IStoreItemPurchaseService _purchaseService;
        private UIClickSound _buySound;

        [Inject]
        public void ConstructPurchase(IStoreItemPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        protected abstract StoreItemType ItemType { get; }

        private StoreItem Item => new(ItemType, Id);

        private UIClickSound BuySound
        {
            get
            {
                if (_buySound == null && BttnBuy)
                    BttnBuy.TryGetComponent(out _buySound);

                return _buySound;
            }
        }

        public override void Initialisation(int id)
        {
            Id = id;

            var price = _purchaseService.GetPrice(Item);
            _price.Show(price);

            if (BttnBuy)
                BttnBuy.interactable = price.CanBuy;

            if (BuySound)
                BuySound.DeferSound = price.IsRealMoney;
        }

        public override void Buy()
        {
            BuyAsync().Forget();
        }

        private async UniTaskVoid BuyAsync()
        {
            var isRealMoney = _purchaseService.GetPrice(Item).IsRealMoney;
            var isBought = await _purchaseService.BuyAsync(Item, destroyCancellationToken);

            if (isBought && isRealMoney && BuySound)
                BuySound.PlayDeferredSound();
        }
    }
}
