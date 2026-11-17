using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class DummyIAPWrapper : IAPWrapper
    {
        private const float PurchaseDelay = 1.0f;

        public DummyIAPWrapper(IIAPService iapService, IIAPWrapperCallbacks callbacks, ISystemMessageService systemMessage, IServiceLogger logger)
            : base(iapService, callbacks, systemMessage, logger)
        {
        }

        public override async UniTask InitAsync(IAPSettings settings)
        {
            await UniTask.Yield();

            _callbacks.OnModuleInitialized();
        }

        public override void BuyProduct(ProductKeyType productKeyType)
        {
            if (!_iapService.IsInitialized)
            {
                _systemMessage.ShowMessage(NetworkErrorMessage);
                return;
            }

            var item = _iapService.GetIAPItem(productKeyType);

            _systemMessage.ShowLoadingPanel();
            _systemMessage.ChangeLoadingMessage("Payment in progress..");

            DOVirtual.DelayedCall(PurchaseDelay, () =>
            {
                _logger.Log(string.Format("[IAPManager]: Purchasing - {0} is completed!", productKeyType));

                _callbacks.OnPurchaseCompleted(item);

                _systemMessage.ChangeLoadingMessage("Payment complete!");
                _systemMessage.HideLoadingPanel();
            }, true);
        }

        public override ProductData GetProductData(ProductKeyType productKeyType)
        {
            var iapItem = _iapService.GetIAPItem(productKeyType);
            if (iapItem != null)
                return new ProductData(iapItem.ProductType);

            return null;
        }

        public override bool IsSubscribed(ProductKeyType productKeyType)
        {
            return false;
        }

        public override bool IsPurchased(string id)
        {
            return false;
        }

        public override void RestorePurchases()
        {
        }
    }
}
