using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Purchasing;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IIAPShopService
    {
        bool IsReady { get; }
        bool IsRestoreSupported { get; }
        string GetPrice(ProductKeyType product);
        bool CanPurchase(ProductKeyType product);
        UniTask<bool> PurchaseAsync(ProductKeyType product, CancellationToken cancellationToken);
        void RestorePurchases();
    }
}
