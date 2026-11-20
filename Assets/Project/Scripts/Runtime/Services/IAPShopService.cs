using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class IAPShopService : IIAPShopService, IInitializable, IDisposable
    {
        private const decimal StorePlaceholderPrice = 0.01m;
        private const string DefaultCurrencySymbol = "$";
        private const string DefaultPriceFormat = "0.00";

        private readonly IIAPService _iapService;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<ProductKeyType, UniTaskCompletionSource<bool>> _pendingPurchases = new();

        public IAPShopService(IIAPService iapService, SignalBus signalBus)
        {
            _iapService = iapService;
            _signalBus = signalBus;
        }

        public bool IsReady => _iapService.IsInitialized;

#if UNITY_IOS
        public bool IsRestoreSupported => true;
#else
        public bool IsRestoreSupported => false;
#endif

        public void Initialize()
        {
            _iapService.SubscribeOnPurchaseModuleInitted(OnInitialized);
            _iapService.PurchaseCompleted += OnPurchaseCompleted;
            _iapService.PurchaseFailed += OnPurchaseFailed;
        }

        public void Dispose()
        {
            _iapService.Initialized -= OnInitialized;
            _iapService.PurchaseCompleted -= OnPurchaseCompleted;
            _iapService.PurchaseFailed -= OnPurchaseFailed;
        }

        public string GetPrice(ProductKeyType product)
        {
            var storeProduct = IsReady ? _iapService.GetProductData(product) : null;
            if (storeProduct != null && storeProduct.Price > StorePlaceholderPrice && !string.IsNullOrEmpty(storeProduct.LocalizedPriceString))
                return storeProduct.LocalizedPriceString;

            var item = _iapService.GetIAPItem(product);
            return item != null ? FormatDefaultPrice(item.DefaultUSDPrice) : string.Empty;
        }

        public bool CanPurchase(ProductKeyType product)
            => IsReady && _iapService.GetIAPItem(product) != null;

        public UniTask<bool> PurchaseAsync(ProductKeyType product, CancellationToken cancellationToken)
        {
            if (!CanPurchase(product))
                return UniTask.FromResult(false);

            if (!_pendingPurchases.TryGetValue(product, out var completion))
            {
                completion = new UniTaskCompletionSource<bool>();
                _pendingPurchases[product] = completion;
            }

            _iapService.BuyProduct(product);
            return completion.Task.AttachExternalCancellation(cancellationToken);
        }

        public void RestorePurchases()
        {
            if (IsRestoreSupported)
                _iapService.RestorePurchases();
        }

        private void OnInitialized()
            => _signalBus.Fire<IAPInitializedSignal>();

        private void OnPurchaseCompleted(ProductKeyType product)
            => CompletePurchase(product, true);

        private void OnPurchaseFailed(ProductKeyType product, PurchaseFailureReason reason)
            => CompletePurchase(product, false);

        private void CompletePurchase(ProductKeyType product, bool isPurchased)
        {
            if (_pendingPurchases.Remove(product, out var completion))
                completion.TrySetResult(isPurchased);
        }

        private static string FormatDefaultPrice(float price)
            => DefaultCurrencySymbol + ((decimal)price).ToString(DefaultPriceFormat, CultureInfo.InvariantCulture);
    }
}
