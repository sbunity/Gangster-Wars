using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if MODULE_IAP && UNITY_IAP_NEW
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class UnityIAP5Wrapper : IAPWrapper
    {
        private readonly IUserService _userService;
        private readonly StoreType _currentStore = GetCurrentStore();

#if MODULE_IAP && UNITY_IAP_NEW
        private StoreController _storeController;

        private CrossPlatformValidator _validator;

        private EntitlementsService _entitlementsService;

        private FraudDetectionData _fraudDetectionData;
#endif

        public UnityIAP5Wrapper(IIAPService iapService, IIAPWrapperCallbacks callbacks, ISystemMessageService systemMessage, IServiceLogger logger, IUserService userService)
            : base(iapService, callbacks, systemMessage, logger)
        {
            _userService = userService;
        }

        public override async UniTask InitAsync(IAPSettings settings)
        {
#if MODULE_IAP && UNITY_IAP_NEW
            try
            {
                _storeController = UnityIAPServices.StoreController();

                _storeController.OnPurchasePending += OnPurchasePending;
                _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                _storeController.OnPurchaseFailed += OnPurchaseFailed;

                _storeController.OnStoreDisconnected += OnStoreDisconnected;

                _logger.Log("[IAP Manager]: Connecting to store.");

                await _storeController.Connect();

                _fraudDetectionData = new FraudDetectionData(_userService.GetIdSha256Hex(), _userService.GetIdAsGuidFromSha256());
                _entitlementsService = new EntitlementsService(_storeController, _logger);

#if UNITY_ANDROID
                ConfigureGoogleFraudDetection(_storeController.GooglePlayStoreExtendedService);
#elif UNITY_IOS
                ConfigureAppleFraudDetection(_storeController.AppleStoreExtendedService);
#endif

                ConfigureValidator();

                _storeController.OnProductsFetchFailed += OnProductsFetchedFailed;
                _storeController.OnProductsFetched += OnProductsFetched;
                _storeController.OnPurchasesFetched += OnPurchasesFetched;
                _storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
                _storeController.OnPurchaseDeferred += OnPurchaseDeferred;

                var initialProductsToFetch = new List<ProductDefinition>();

                var items = settings.StoreItems;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.ID))
                        initialProductsToFetch.Add(new ProductDefinition(item.ID, (UnityEngine.Purchasing.ProductType)item.ProductType));
                    else
                        Debug.LogWarning($"[IAP Manager]: Product {item.ProductType} does not have configured IDs.");
                }

                _storeController.FetchProducts(initialProductsToFetch);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[IAP Manager]: Initialization failed with exception: {exception}");
            }
#else
            _logger.Log("[IAP Manager]: Define MODULE_IAP is disabled!");
            await UniTask.CompletedTask;
#endif
        }

#if MODULE_IAP && UNITY_IAP_NEW
        private void OnPurchaseDeferred(DeferredOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;

                if (product is null)
                {
                    _logger.Log("[IAPManager]: Could not find product in order.");
                    continue;
                }

                _logger.Log($"OnPurchaseDeferred - Product: {product?.definition.id}");
            }
        }
#endif

        private void ConfigureValidator()
        {
            if (_currentStore != StoreType.GooglePlay) return;

#if MODULE_IAP && UNITY_IAP_NEW
#if !UNITY_EDITOR
            var googlePlayTangleType = Type.GetType("UnityEngine.Purchasing.Security.GooglePlayTangle");
            if (googlePlayTangleType != null)
            {
                var dataMethod = googlePlayTangleType.GetMethod("Data", BindingFlags.Static | BindingFlags.Public);
                if (dataMethod != null)
                {
                    var googleData = (byte[])dataMethod.Invoke(null, null);
                    _validator = new CrossPlatformValidator(googleData, Application.identifier);
                }
            }
#endif
#endif
        }

#if MODULE_IAP && UNITY_IAP_NEW
        private void OnPurchaseFailed(FailedOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;

                if (product is null)
                {
                    _logger.Log("[IAPManager]: Could not find product in order.");
                    continue;
                }

                _logger.Log($"Confirmation failed - Product: '{product?.definition.id}'," +
                          $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                          + $"Confirmation Failure Details: {order.Details}");

                var item = _iapService.GetIAPItem(product.definition.id);
                if (item != null)
                    _callbacks.OnPurchaseFailed(item.ProductKeyType, (SBabchuk.Runtime.Purchasing.PurchaseFailureReason)order.FailureReason);
                else
                    _logger.Log($"[IAPManager]: Product - {product.definition.id} can't be found!");

                _logger.LogWarning($"Purchase failed - Product: '{product?.definition.id}'," +
                          $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                          + $"Purchase Failure Details: {order.Details}");
            }

            _systemMessage.ChangeLoadingMessage("Payment failed!");
            _systemMessage.HideLoadingPanel();
        }

        private bool IsPurchaseValid(Order order)
        {
            if (_validator != null)
            {
                try
                {
                    var result = _validator.Validate(order.Info.Receipt);

                    _logger.Log("Receipt is valid. Contents:");
                    foreach (var receipt in result)
                    {
                        _logger.Log($"Product ID: {receipt.productID}\n" +
                            $"Purchase Date: {receipt.purchaseDate}\n" +
                            $"Transaction ID: {receipt.transactionID}");

                        if (receipt is GooglePlayReceipt googleReceipt)
                        {
                            _logger.Log($"Purchase State: {googleReceipt.purchaseState}\n" +
                                $"Purchase Token: {googleReceipt.purchaseToken}");
                        }
                    }
                }
                catch (IAPSecurityException reason)
                {
                    _logger.Log($"Invalid receipt: {reason}");

                    return false;
                }
            }

            return true;
        }

        private void OnPurchasePending(PendingOrder order)
        {
            var validPurchase = true;

            if (_validator != null)
                validPurchase = IsPurchaseValid(order);

            if (validPurchase)
                _storeController.ConfirmPurchase(order);
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription description)
        {
        }

        private void OnPurchasesFetched(Orders orders)
        {
            var ownedProductIds = new HashSet<string>();

            foreach (var confirmedOrder in orders.ConfirmedOrders)
            {
                foreach (var item in confirmedOrder.CartOrdered.Items())
                {
                    var productId = item.Product.definition.id;

                    ownedProductIds.Add(productId);
                }
            }

            foreach (var productId in ownedProductIds)
            {
                var iapItem = _iapService.GetIAPItem(productId);
                if (iapItem == null)
                    continue;

                if (iapItem.ProductType == ProductType.Consumable)
                    continue;

                _callbacks.OnPurchaseRestored(iapItem);
            }
        }

        private void OnPurchaseConfirmed(Order order)
        {
            switch (order)
            {
                case ConfirmedOrder confirmedOrder:
                    OnPurchaseConfirmed(confirmedOrder);
                    break;
                case FailedOrder failedOrder:
                    OnPurchaseConfirmationFailed(failedOrder);
                    break;
                default:
                    _logger.Log("Unknown OnPurchaseConfirmed result.");
                    break;
            }
        }

        private void OnPurchaseConfirmed(ConfirmedOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;

                if (product is null)
                {
                    _logger.Log("[IAPManager]: Could not find product in order.");
                    continue;
                }

                _logger.Log($"[IAPManager]: Purchasing - {product.definition.id} is completed!");

                var item = _iapService.GetIAPItem(product.definition.id);
                if (item != null)
                    _callbacks.OnPurchaseCompleted(item);
                else
                    _logger.LogWarning($"[IAPManager]: Product - {product.definition.id} can't be found!");
            }

            _systemMessage.ChangeLoadingMessage("Payment complete!");
            _systemMessage.HideLoadingPanel();
        }

        private void OnPurchaseConfirmationFailed(FailedOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;

                if (product is null)
                {
                    _logger.Log("[IAPManager]: Could not find product in order.");
                    continue;
                }

                _logger.Log($"Confirmation failed - Product: '{product?.definition.id}'," +
                          $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                          + $"Confirmation Failure Details: {order.Details}");
            }

            _systemMessage.ChangeLoadingMessage("Purchase failed!");
            _systemMessage.HideLoadingPanel();
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            _logger.Log($"Store disconnected details: {description.message}");
        }

        private void OnProductsFetched(List<Product> products)
        {
            _logger.Log($"Products fetched successfully for {products.Count} products.");

            _storeController.FetchPurchases();

            _callbacks.OnModuleInitialized();
        }

        private void OnProductsFetchedFailed(ProductFetchFailed failure)
        {
            _logger.Log($"Products fetch failed for {failure.FailedFetchProducts.Count} products: {failure.FailureReason}");
        }
#endif

        public override void RestorePurchases()
        {
#if MODULE_IAP && UNITY_IAP_NEW
            if (!_iapService.IsInitialized)
            {
                _systemMessage.ShowMessage(NetworkErrorMessage);
                return;
            }

            _systemMessage.ShowLoadingPanel();
            _systemMessage.ChangeLoadingMessage("Restoring purchased products..");

            _storeController.RestoreTransactions(OnRestored);
#endif

#if UNITY_EDITOR
            OnRestored(true, "");
#endif
        }

        private void OnRestored(bool result, string error)
        {
#if MODULE_IAP && UNITY_IAP_NEW
            DOVirtual.DelayedCall(RestoreMessageDelay, () =>
            {
                if (result)
                    _systemMessage.ChangeLoadingMessage("Restoration completed!");
                else
                    _systemMessage.ChangeLoadingMessage($"Restoration failed with error: {error}!");

                DOVirtual.DelayedCall(RestoreMessageDelay, () => _systemMessage.HideLoadingPanel(), true);
            }, true);
#endif
        }

        public override void BuyProduct(ProductKeyType productKeyType)
        {
#if MODULE_IAP && UNITY_IAP_NEW
            if (!_iapService.IsInitialized)
            {
                _systemMessage.ShowMessage(NetworkErrorMessage);
                return;
            }

            _systemMessage.ShowLoadingPanel();
            _systemMessage.ChangeLoadingMessage("Payment in progress..");

            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
                _storeController.PurchaseProduct(item.ID);
#else
            _systemMessage.ShowMessage("Network error.");
#endif
        }

        public override ProductData GetProductData(ProductKeyType productKeyType)
        {
            if (!_iapService.IsInitialized)
                return null;

#if MODULE_IAP && UNITY_IAP_NEW
            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
                return new ProductData(_storeController.GetProductById(item.ID));
#endif

            return null;
        }

        public override bool IsSubscribed(ProductKeyType productKeyType)
        {
#if MODULE_IAP && UNITY_IAP_NEW
            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
                return _entitlementsService.IsSubscriptionActive(item.ID);
#endif

            return false;
        }

        public override bool IsPurchased(string id)
        {
#if MODULE_IAP && UNITY_IAP_NEW
            return _entitlementsService.IsOwned(id);
#else
            return false;
#endif
        }

#if MODULE_IAP && UNITY_IAP_NEW
        private void ConfigureGoogleFraudDetection(IGooglePlayStoreExtendedService googlePlayStoreExtendedService)
        {
            if (googlePlayStoreExtendedService == null)
            {
                _logger.Log("Google Play Store Extended Service is not available. Please make sure the project is being built for Android and the Google Play Store.");
                return;
            }

            googlePlayStoreExtendedService.SetObfuscatedAccountId(_fraudDetectionData.AccountId);
        }

        private void ConfigureAppleFraudDetection(IAppleStoreExtendedService appleStoreExtendedService)
        {
            if (appleStoreExtendedService == null)
            {
                _logger.Log("App Store Extended Service is not available. Please make sure the project is being built for Android and the Google Play Store.");
                return;
            }

            appleStoreExtendedService.SetAppAccountToken(_fraudDetectionData.AccountToken);
        }
#endif

        private static StoreType GetCurrentStore()
        {
#if UNITY_EDITOR
            return StoreType.Fake;
#elif UNITY_ANDROID
            return StoreType.GooglePlay;
#elif UNITY_IOS
            return StoreType.AppleAppStore;
#elif UNITY_STANDALONE_OSX
            return StoreType.MacAppStore;
#else
            return StoreType.NotSpecified;
#endif
        }

        public enum StoreType
        {
            NotSpecified = 0,
            Fake = 1,
            GooglePlay = 2,
            MacAppStore = 3,
            AppleAppStore = 4
        }

        [Serializable]
        public sealed class FraudDetectionData
        {
            public string AccountId;
            public Guid AccountToken;

            public FraudDetectionData(string accountId, Guid accountToken)
            {
                AccountId = accountId;
                AccountToken = accountToken;
            }
        }

#if MODULE_IAP && UNITY_IAP_NEW
        public sealed class EntitlementsService
        {
            private readonly StoreController _store;
            private readonly IServiceLogger _logger;

            private readonly HashSet<string> _owned = new(StringComparer.Ordinal);

            private readonly Dictionary<string, SubscriptionStatusSnapshot> _subs = new(StringComparer.Ordinal);

            private readonly Dictionary<string, EntitlementStatus> _entitlements = new(StringComparer.Ordinal);

            public event Action EntitlementsChanged;

            public EntitlementsService(StoreController store, IServiceLogger logger)
            {
                _store = store ?? throw new ArgumentNullException(nameof(store));
                _logger = logger;

                _store.OnPurchasesFetched += OnPurchasesFetched;
                _store.OnPurchaseConfirmed += OnPurchaseConfirmed;
                _store.OnCheckEntitlement += OnCheckEntitlement;

                _store.ProcessPendingOrdersOnPurchasesFetched(true);
            }

            public void RefreshAll() => _store?.FetchPurchases();
            public bool IsOwned(string unityProductId) => _owned.Contains(unityProductId);
            public bool IsSubscriptionActive(string unityProductId) => _subs.TryGetValue(unityProductId, out var s) && s.IsActive;
            public SubscriptionStatusSnapshot GetSubscription(string unityProductId) => _subs.TryGetValue(unityProductId, out var s) ? s : default;
            public EntitlementStatus GetEntitlementStatus(string unityProductId) => _entitlements.TryGetValue(unityProductId, out var st) ? st : EntitlementStatus.Unknown;

            public IReadOnlyCollection<string> OwnedNonConsumables => new ReadOnlyCollection<string>(_owned.ToList());
            public IReadOnlyDictionary<string, SubscriptionStatusSnapshot> Subscriptions => new ReadOnlyDictionary<string, SubscriptionStatusSnapshot>(_subs);

            private void OnPurchasesFetched(Orders orders)
            {
                _owned.Clear();
                _subs.Clear();

                if (orders == null)
                {
                    RaiseChanged();
                    return;
                }

                foreach (var confirmed in orders.ConfirmedOrders)
                    AddOrder(confirmed);

                RaiseChanged();
            }

            private void OnPurchaseConfirmed(Order order)
            {
                if (order is ConfirmedOrder confirmed)
                {
                    AddOrder(confirmed);
                    RaiseChanged();
                }
            }

            private void OnCheckEntitlement(Entitlement entitlement)
            {
                if (entitlement?.Product == null) return;

                var unityId = entitlement.Product.definition?.id;
                if (string.IsNullOrEmpty(unityId)) return;

                _entitlements[unityId] = entitlement.Status;

                if (entitlement.Order is ConfirmedOrder co)
                {
                    AddOrder(co);
                    RaiseChanged();
                }
            }

            public bool RequestEntitlementCheck(string unityProductId)
            {
                var p = _store?.GetProductById(unityProductId);
                if (p == null) return false;

                _store.CheckEntitlement(p);
                return true;
            }

            private void AddOrder(ConfirmedOrder confirmed)
            {
                if (confirmed?.Info?.PurchasedProductInfo == null)
                    return;

                foreach (var ppi in confirmed.Info.PurchasedProductInfo)
                {
                    var unityId = ResolveUnityProductId(ppi);
                    if (unityId == null)
                        continue;

                    var product = _store.GetProductById(unityId);
                    var pType = product?.definition?.type ?? UnityEngine.Purchasing.ProductType.Unknown;

                    switch (pType)
                    {
                        case UnityEngine.Purchasing.ProductType.NonConsumable:
                            _owned.Add(unityId);
                            break;

                        case UnityEngine.Purchasing.ProductType.Subscription:
                            var snap = BuildSubscriptionSnapshot(unityId, ppi);
                            _subs[unityId] = snap;
                            break;
                    }
                }
            }

            private string ResolveUnityProductId(IPurchasedProductInfo ppi)
            {
                if (ppi == null) return null;

                if (!string.IsNullOrEmpty(ppi.productId) && _store.GetProductById(ppi.productId) != null)
                    return ppi.productId;

                var all = _store.GetProducts();
                var fromStoreSpecific = all.FirstOrDefault(p => string.Equals(p.definition?.storeSpecificId, ppi.productId, StringComparison.OrdinalIgnoreCase));

                if (fromStoreSpecific != null)
                    return fromStoreSpecific.definition.id;

                var storePid = ppi.subscriptionInfo?.GetProductId();
                if (!string.IsNullOrEmpty(storePid))
                {
                    var bySubStoreId = all.FirstOrDefault(p => string.Equals(p.definition?.storeSpecificId, storePid, StringComparison.OrdinalIgnoreCase));

                    if (bySubStoreId != null)
                        return bySubStoreId.definition.id;
                }

                _logger.LogWarning($"[IapEntitlements] Unknown purchased product id: '{ppi.productId}'. Ensure your catalog IDs match store product IDs.");

                return null;
            }

            private static SubscriptionStatusSnapshot BuildSubscriptionSnapshot(string unityProductId, IPurchasedProductInfo ppi)
            {
                var si = ppi.subscriptionInfo;
                if (si == null)
                    return new SubscriptionStatusSnapshot(unityProductId, false, null, null, false, false, null, null, null);

                var expired = si.IsExpired();
                var isActive = expired == Result.False;

                DateTime? expireAt = null;
                try { expireAt = si.GetExpireDate(); }
                catch { }

                TimeSpan? remaining = null;
                try { remaining = si.GetRemainingTime(); }
                catch { }

                var autoRenews = false;
                try { autoRenews = si.IsAutoRenewing() == Result.True; }
                catch { }

                var cancelled = false;
                try { cancelled = si.IsCancelled() == Result.True; }
                catch { }

                DateTime? purchaseDate = null;
                try { purchaseDate = si.GetPurchaseDate(); }
                catch { }

                string storeProductId = null;
                try { storeProductId = si.GetProductId(); }
                catch { }

                string payload = null;
                try { payload = si.GetSubscriptionInfoJsonString(); }
                catch { }

                return new SubscriptionStatusSnapshot(
                    unityProductId,
                    isActive,
                    expireAt,
                    remaining,
                    autoRenews,
                    cancelled,
                    storeProductId,
                    purchaseDate,
                    payload
                );
            }

            private void RaiseChanged() => EntitlementsChanged?.Invoke();
        }

        public readonly struct SubscriptionStatusSnapshot
        {
            public readonly string UnityProductId;
            public readonly bool IsActive;
            public readonly DateTime? ExpireAtUtc;
            public readonly TimeSpan? Remaining;
            public readonly bool AutoRenews;
            public readonly bool IsCancelled;
            public readonly string StoreProductId;
            public readonly DateTime? PurchaseDateUtc;
            public readonly string RawJson;

            public SubscriptionStatusSnapshot(string unityProductId, bool isActive, DateTime? expireAtUtc, TimeSpan? remaining, bool autoRenews, bool isCancelled, string storeProductId, DateTime? purchaseDateUtc, string rawJson)
            {
                UnityProductId = unityProductId;
                IsActive = isActive;
                ExpireAtUtc = expireAtUtc;
                Remaining = remaining;
                AutoRenews = autoRenews;
                IsCancelled = isCancelled;
                StoreProductId = storeProductId;
                PurchaseDateUtc = purchaseDateUtc;
                RawJson = rawJson;
            }

            public override string ToString() => $"[{UnityProductId}] Active={IsActive}, Expire={ExpireAtUtc:O}, AutoRenews={AutoRenews}, Cancelled={IsCancelled}";
        }
#endif
    }
}
