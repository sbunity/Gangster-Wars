using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class IAPService : IIAPService, IIAPWrapperCallbacks
    {
        private readonly IPlayerProgressRepository _repository;
        private readonly IInstantiator _instantiator;
        private readonly IServiceLogger _logger;

        private Dictionary<ProductKeyType, IAPItem> _productsTypeToProductLink;
        private Dictionary<string, IAPItem> _productsIDToProductLink;

        private IAPWrapper _wrapper;

        private IAPSettings _settings;
        private IAPSave _save;

        public IAPService(IPlayerProgressRepository repository, IInstantiator instantiator, IServiceLogger logger)
        {
            _repository = repository;
            _instantiator = instantiator;
            _logger = logger;
        }

        public bool IsInitialized { get; private set; }

        public event Action Initialized;
        public event Action<ProductKeyType> PurchaseCompleted;
        public event Action<ProductKeyType> PurchaseRestored;
        public event Action<ProductKeyType, PurchaseFailureReason> PurchaseFailed;

        public IAPItem GetIAPItem(string productID)
        {
            if (string.IsNullOrEmpty(productID)) return null;

            _productsIDToProductLink.TryGetValue(productID, out var item);
            return item;
        }

        public IAPItem GetIAPItem(ProductKeyType productKeyType)
        {
            _productsTypeToProductLink.TryGetValue(productKeyType, out var item);
            return item;
        }

        public void RestorePurchases()
        {
            if (!IsInitialized) return;

            _wrapper.RestorePurchases();
        }

        public void SubscribeOnPurchaseModuleInitted(Action callback)
        {
            if (IsInitialized)
                callback?.Invoke();
            else
                Initialized += callback;
        }

        public void BuyProduct(ProductKeyType productKeyType)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAP Manager]: The module is not initialized!");
                return;
            }

            _wrapper.BuyProduct(productKeyType);
        }

        public ProductData GetProductData(ProductKeyType productKeyType)
        {
            if (!IsInitialized) return new ProductData();

            var product = _wrapper.GetProductData(productKeyType);

            if (product == null)
                Debug.LogWarning($"[IAP Manager]: Product of type '{productKeyType}' was not found in IAP Settings. Please ensure it is added to the products list.");

            return product;
        }

        public bool IsSubscribed(ProductKeyType productKeyType)
        {
            if (!IsInitialized) return false;

            return _wrapper.IsSubscribed(productKeyType);
        }

        public bool IsPurchased(ProductKeyType productKeyType)
        {
#if MODULE_IAP
            var iapItem = GetIAPItem(productKeyType);
            if (iapItem != null)
                return _wrapper.IsPurchased(iapItem.ID);
#endif
            return false;
        }

        public string GetProductLocalPriceString(ProductKeyType productKeyType)
        {
            var product = GetProductData(productKeyType);

            if (product == null)
            {
                Debug.LogWarning($"[IAP Manager]: Product of type '{productKeyType}' was not found in IAP Settings. Please ensure it is added to the products list.");
                return string.Empty;
            }

            return $"{product.ISOCurrencyCode} {product.Price}";
        }

        public bool IsPayableUser() => _save != null && _save.FirstPurchase;

        public void Init(IAPSettings iapSettings)
        {
            if (IsInitialized)
            {
                Debug.LogError("[IAP Manager]: Module is already initialized!");
                return;
            }

            if (iapSettings == null)
            {
                Debug.LogError("[IAP Manager]: IAPSettings is null!");
                return;
            }

            _settings = iapSettings;

            _save = _repository.Progress.IAP;

            _productsTypeToProductLink = new Dictionary<ProductKeyType, IAPItem>();
            _productsIDToProductLink = new Dictionary<string, IAPItem>();

            var items = _settings.StoreItems;
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.Init(GetProductSave(item.ProductKeyType));

                    if (!_productsTypeToProductLink.ContainsKey(item.ProductKeyType))
                    {
                        _productsTypeToProductLink.Add(item.ProductKeyType, item);
                        _productsIDToProductLink[item.ID] = item;
                    }
                    else
                    {
                        Debug.LogError($"[IAP Manager]: Product with the type {item.ProductKeyType} has duplicates in the list!");
                    }
                }
            }

            _wrapper = GetPlatformWrapper();
            _wrapper.InitAsync(_settings).Forget();
        }

        public void OnModuleInitialized()
        {
            IsInitialized = true;

            if (Initialized != null)
            {
                foreach (var listener in Initialized.GetInvocationList())
                {
                    if (listener.Target is UnityEngine.Object unityObject && unityObject == null)
                        continue;

                    try
                    {
                        ((Action)listener).Invoke();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }

            _logger.Log("[IAPManager]: Module is initialized!");
        }

        public void OnPurchaseCompleted(IAPItem item)
        {
            if (!_save.FirstPurchase)
                _save.FirstPurchase = true;

            item.OnProductPurchased();

            _repository.Save();

            PurchaseCompleted?.Invoke(item.ProductKeyType);
        }

        public void OnPurchaseRestored(IAPItem item)
        {
            PurchaseRestored?.Invoke(item.ProductKeyType);
        }

        public void OnPurchaseFailed(ProductKeyType productKey, PurchaseFailureReason failureReason)
        {
            PurchaseFailed?.Invoke(productKey, failureReason);
        }

        private IAPProductSave GetProductSave(ProductKeyType productKeyType)
        {
            var productSave = _save.GetProduct(productKeyType);
            if (productSave == null)
            {
                productSave = new IAPProductSave(productKeyType);
                _save.Products.Add(productSave);
            }

            return productSave;
        }

        private IAPWrapper GetPlatformWrapper()
        {
#if MODULE_IAP
#if UNITY_IAP_NEW
            return _instantiator.Instantiate<UnityIAP5Wrapper>();
#else
            return _instantiator.Instantiate<UnityIAPWrapper>();
#endif
#else
            return _instantiator.Instantiate<DummyIAPWrapper>();
#endif
        }
    }
}
