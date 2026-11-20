using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services
{
    public sealed class StoreItemPurchaseService : IStoreItemPurchaseService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressService _progressService;
        private readonly ICarSkinService _carSkinService;
        private readonly IIAPShopService _iapShopService;

        public StoreItemPurchaseService(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            ICarSkinService carSkinService,
            IIAPShopService iapShopService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _carSkinService = carSkinService;
            _iapShopService = iapShopService;
        }

        public StoreItemPrice GetPrice(StoreItem item)
        {
            if (TryGetProduct(item, out var product))
                return StoreItemPrice.ForMoney(_iapShopService.GetPrice(product), _iapShopService.CanPurchase(product));

            var coins = GetCoinPrice(item);
            return coins.HasValue
                ? StoreItemPrice.ForCoins(coins.Value, CanBuyForCoins(item, coins.Value))
                : StoreItemPrice.Unavailable;
        }

        public UniTask<bool> BuyAsync(StoreItem item, CancellationToken cancellationToken)
        {
            if (TryGetProduct(item, out var product))
                return _iapShopService.PurchaseAsync(product, cancellationToken);

            BuyForCoins(item);
            return UniTask.FromResult(IsOwned(item));
        }

        private void BuyForCoins(StoreItem item)
        {
            switch (item.Type)
            {
                case StoreItemType.Weapon:
                    _progressService.BuyWeapon(item.Id);
                    break;
                case StoreItemType.Personage:
                    _progressService.BuyPersonage(item.Id);
                    break;
                case StoreItemType.Defence:
                    _progressService.BuyDefence(item.Id);
                    break;
                case StoreItemType.CarSkin:
                    _carSkinService.TryBuy(item.Id);
                    break;
            }
        }

        public void Grant(StoreItem item)
        {
            switch (item.Type)
            {
                case StoreItemType.Weapon:
                    _progressService.UnlockWeapon(item.Id);
                    break;
                case StoreItemType.Personage:
                    _progressService.UnlockPersonage(item.Id);
                    break;
                case StoreItemType.Defence:
                    _progressService.UnlockDefence(item.Id);
                    break;
                case StoreItemType.CarSkin:
                    _carSkinService.Unlock(item.Id);
                    break;
            }
        }

        private bool TryGetProduct(StoreItem item, out ProductKeyType product)
            => _assetProvider.IAPRewardsConfig.TryGetProduct(item, out product);

        private int? GetCoinPrice(StoreItem item)
            => item.Type switch
            {
                StoreItemType.Weapon => _assetProvider.WeaponStoreDatabase.GetWeapon(item.Id)?.Price,
                StoreItemType.Personage => _assetProvider.MainPlayerDatabase.GetPersonage(item.Id)?.Price,
                StoreItemType.Defence => _assetProvider.DefenseStoreDatabase.GetDefense(item.Id)?.Price,
                StoreItemType.CarSkin => _assetProvider.CarSkinDatabase.GetSkin(item.Id)?.Price,
                _ => null
            };

        private bool CanBuyForCoins(StoreItem item, int price)
            => item.Type == StoreItemType.CarSkin
                ? _carSkinService.CanBuy(item.Id)
                : _progressService.CanBuy(price);

        private bool IsOwned(StoreItem item)
            => item.Type switch
            {
                StoreItemType.Weapon => _progressService.GetWeaponShortInfo(item.Id)?.IsBuy == mySwitch.On,
                StoreItemType.Personage => _progressService.GetPersonageShortInfo(item.Id)?.IsBuy == mySwitch.On,
                StoreItemType.Defence => _progressService.GetDefenceShortInfo(item.Id)?.IsBuy == mySwitch.On,
                StoreItemType.CarSkin => _carSkinService.IsOwned(item.Id),
                _ => false
            };
    }
}
