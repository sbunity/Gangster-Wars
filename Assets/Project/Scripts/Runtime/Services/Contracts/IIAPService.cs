using System;
using SBabchuk.Runtime.Purchasing;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IIAPService
    {
        bool IsInitialized { get; }

        event Action Initialized;
        event Action<ProductKeyType> PurchaseCompleted;
        event Action<ProductKeyType> PurchaseRestored;
        event Action<ProductKeyType, PurchaseFailureReason> PurchaseFailed;

        void Init(IAPSettings iapSettings);
        IAPItem GetIAPItem(string productID);
        IAPItem GetIAPItem(ProductKeyType productKeyType);
        void RestorePurchases();
        void SubscribeOnPurchaseModuleInitted(Action callback);
        void BuyProduct(ProductKeyType productKeyType);
        ProductData GetProductData(ProductKeyType productKeyType);
        bool IsSubscribed(ProductKeyType productKeyType);
        bool IsPurchased(ProductKeyType productKeyType);
        string GetProductLocalPriceString(ProductKeyType productKeyType);
        bool IsPayableUser();
    }
}
