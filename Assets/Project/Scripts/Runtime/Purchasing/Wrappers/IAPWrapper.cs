using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Purchasing
{
    public abstract class IAPWrapper
    {
        protected const string NetworkErrorMessage = "Network error. Please try again later";
        protected const float RestoreMessageDelay = 0.5f;

        protected readonly IIAPService _iapService;
        protected readonly IIAPWrapperCallbacks _callbacks;
        protected readonly ISystemMessageService _systemMessage;
        protected readonly IServiceLogger _logger;

        protected IAPWrapper(IIAPService iapService, IIAPWrapperCallbacks callbacks, ISystemMessageService systemMessage, IServiceLogger logger)
        {
            _iapService = iapService;
            _callbacks = callbacks;
            _systemMessage = systemMessage;
            _logger = logger;
        }

        public abstract UniTask InitAsync(IAPSettings settings);
        public abstract void RestorePurchases();
        public abstract void BuyProduct(ProductKeyType productKeyType);
        public abstract ProductData GetProductData(ProductKeyType productKeyType);
        public abstract bool IsSubscribed(ProductKeyType productKeyType);
        public abstract bool IsPurchased(string id);
    }
}
