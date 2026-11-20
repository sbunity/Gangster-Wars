using System;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class IAPRewardService : IInitializable, IDisposable
    {
        private readonly IIAPService _iapService;
        private readonly IPlayerProgressService _progressService;
        private readonly IStoreItemPurchaseService _storeItemPurchaseService;
        private readonly INoAdsService _noAdsService;
        private readonly IAssetProvider _assetProvider;

        public IAPRewardService(
            IIAPService iapService,
            IPlayerProgressService progressService,
            IStoreItemPurchaseService storeItemPurchaseService,
            INoAdsService noAdsService,
            IAssetProvider assetProvider)
        {
            _iapService = iapService;
            _progressService = progressService;
            _storeItemPurchaseService = storeItemPurchaseService;
            _noAdsService = noAdsService;
            _assetProvider = assetProvider;
        }

        public void Initialize()
        {
            _iapService.PurchaseCompleted += OnPurchaseCompleted;
            _iapService.PurchaseRestored += OnPurchaseRestored;
        }

        public void Dispose()
        {
            _iapService.PurchaseCompleted -= OnPurchaseCompleted;
            _iapService.PurchaseRestored -= OnPurchaseRestored;
        }

        private void OnPurchaseCompleted(ProductKeyType product)
        {
            if (!TryGetReward(product, out var reward))
                return;

            if (reward.Coins > 0)
                _progressService.AddCoins(reward.Coins);

            GrantEntitlements(reward);
        }

        private void OnPurchaseRestored(ProductKeyType product)
        {
            if (TryGetReward(product, out var reward))
                GrantEntitlements(reward);
        }

        private void GrantEntitlements(IAPReward reward)
        {
            foreach (var item in reward.Items)
                _storeItemPurchaseService.Grant(item);

            if (reward.RemovesAds)
                _noAdsService.RemoveAds();
        }

        private bool TryGetReward(ProductKeyType product, out IAPReward reward)
        {
            if (_assetProvider.IAPRewardsConfig.TryGetReward(product, out reward))
                return true;

            Debug.LogWarning($"[IAP Rewards]: Product {product} has no reward in IAPRewardsConfig.");
            return false;
        }
    }
}
