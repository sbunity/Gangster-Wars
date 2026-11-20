using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if MODULE_IAP && !UNITY_IAP_NEW
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
#endif

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class UnityIAPWrapper : IAPWrapper
#if MODULE_IAP && !UNITY_IAP_NEW
        , IDetailedStoreListener
#endif
    {
        private const string EnvironmentName = "production";

        public UnityIAPWrapper(IIAPService iapService, IIAPWrapperCallbacks callbacks, ISystemMessageService systemMessage, IServiceLogger logger)
            : base(iapService, callbacks, systemMessage, logger)
        {
        }

#if MODULE_IAP && !UNITY_IAP_NEW
        private IStoreController Controller { get; set; }
        private IExtensionProvider Extensions { get; set; }
#endif

        public override async UniTask InitAsync(IAPSettings settings)
        {
#if MODULE_IAP && !UNITY_IAP_NEW
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(EnvironmentName);

                await UnityServices.InitializeAsync(options);

                var purchasingModule = StandardPurchasingModule.Instance();

                if (settings.UseFakeStore)
                {
                    purchasingModule.useFakeStoreAlways = true;
                    purchasingModule.useFakeStoreUIMode = (FakeStoreUIMode)settings.FakeStoreMode;
                }

                var builder = ConfigurationBuilder.Instance(purchasingModule);

                var items = settings.StoreItems;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.ID))
                        builder.AddProduct(item.ID, (UnityEngine.Purchasing.ProductType)item.ProductType);
                    else
                        Debug.LogWarning($"[IAP Manager]: Product {item.ProductType} does not have configured IDs.");
                }

                UnityPurchasing.Initialize(this, builder);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"[IAP Manager]: Initialization failed with exception: {exception}");
            }
#else
            _logger.Log("[IAP Manager]: Define MODULE_IAP is disabled!");
            await UniTask.CompletedTask;
#endif
        }

#if MODULE_IAP && !UNITY_IAP_NEW
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Controller = controller;
            Extensions = extensions;

            _callbacks.OnModuleInitialized();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _logger.Log($"[IAPManager]: Module initialization failed with reason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            _logger.Log($"[IAPManager]: Module initialization failed with reason: {error}, message: {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var product = e.purchasedProduct;

            _logger.Log($"[IAPManager]: Purchasing - {product.definition.id} is completed!");

            var item = _iapService.GetIAPItem(product.definition.id);
            if (item != null)
                _callbacks.OnPurchaseCompleted(item);
            else
                _logger.Log($"[IAPManager]: Product - {product.definition.id} can't be found!");

            _systemMessage.ChangeLoadingMessage("Payment complete!");
            _systemMessage.HideLoadingPanel();

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason failureReason)
        {
            _logger.Log($"[IAPManager]: Purchasing - {product.definition.id} failed with reason: {failureReason}");

            var item = _iapService.GetIAPItem(product.definition.id);
            if (item != null)
                _callbacks.OnPurchaseFailed(item.ProductKeyType, (SBabchuk.Runtime.Purchasing.PurchaseFailureReason)failureReason);
            else
                _logger.Log($"[IAPManager]: Product - {product.definition.id} can't be found!");

            _systemMessage.ChangeLoadingMessage("Payment failed!");
            _systemMessage.HideLoadingPanel();
        }

        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureDescription failureDescription)
        {
            _logger.Log($"[IAPManager]: Purchasing - {product.definition.id} failed with reason: {failureDescription.message}");

            var item = _iapService.GetIAPItem(product.definition.id);
            if (item != null)
                _callbacks.OnPurchaseFailed(item.ProductKeyType, (SBabchuk.Runtime.Purchasing.PurchaseFailureReason)failureDescription.reason);
            else
                _logger.Log($"[IAPManager]: Product - {product.definition.id} can't be found!");

            _systemMessage.ChangeLoadingMessage("Payment failed!");
            _systemMessage.HideLoadingPanel();
        }
#endif

        public override void RestorePurchases()
        {
#if MODULE_IAP && !UNITY_IAP_NEW
            if (!_iapService.IsInitialized)
            {
                _systemMessage.ShowMessage(NetworkErrorMessage);
                return;
            }

            _systemMessage.ShowLoadingPanel();
            _systemMessage.ChangeLoadingMessage("Restoring purchased products..");

#if UNITY_ANDROID
            Extensions.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(OnRestored);
#elif UNITY_IOS
            Extensions.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestored);
#endif
#endif

#if UNITY_EDITOR
            OnRestored(true, "");
#endif
        }

        private void OnRestored(bool result, string error)
        {
#if MODULE_IAP && !UNITY_IAP_NEW
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
#if MODULE_IAP && !UNITY_IAP_NEW
            if (!_iapService.IsInitialized)
            {
                _systemMessage.ShowMessage(NetworkErrorMessage);
                return;
            }

            _systemMessage.ShowLoadingPanel();
            _systemMessage.ChangeLoadingMessage("Payment in progress..");

            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
                Controller.InitiatePurchase(item.ID);
#else
            _systemMessage.ShowMessage("Network error.");
#endif
        }

        public override ProductData GetProductData(ProductKeyType productKeyType)
        {
            if (!_iapService.IsInitialized)
                return null;

#if MODULE_IAP && !UNITY_IAP_NEW
            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
            {
                var product = Controller.products.WithID(item.ID);
                if (product != null)
                    return new ProductData(product);
            }
#endif

            return null;
        }

        public override bool IsSubscribed(ProductKeyType productKeyType)
        {
#if MODULE_IAP && !UNITY_IAP_NEW
            var item = _iapService.GetIAPItem(productKeyType);
            if (item != null)
            {
                var product = Controller.products.WithID(item.ID);
                if (product != null)
                {
                    if (product.receipt == null)
                        return false;

                    var subscriptionManager = new SubscriptionManager(product, null);
                    var info = subscriptionManager.getSubscriptionInfo();

                    return info.isSubscribed() == Result.True;
                }
            }
#endif

            return false;
        }

        public override bool IsPurchased(string id)
        {
#if MODULE_IAP && !UNITY_IAP_NEW
            var item = _iapService.GetIAPItem(id);
            if (item != null)
            {
                var product = Controller.products.WithID(item.ID);
                if (product != null)
                    return product.hasReceipt;
            }
#endif

            return false;
        }
    }
}
