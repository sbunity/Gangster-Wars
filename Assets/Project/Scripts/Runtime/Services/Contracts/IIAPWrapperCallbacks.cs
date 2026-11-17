using SBabchuk.Runtime.Purchasing;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IIAPWrapperCallbacks
    {
        void OnModuleInitialized();
        void OnPurchaseCompleted(IAPItem item);
        void OnPurchaseRestored(IAPItem item);
        void OnPurchaseFailed(ProductKeyType productKey, PurchaseFailureReason failureReason);
    }
}
