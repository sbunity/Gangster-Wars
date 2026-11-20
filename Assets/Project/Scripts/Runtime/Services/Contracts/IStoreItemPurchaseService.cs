using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IStoreItemPurchaseService
    {
        StoreItemPrice GetPrice(StoreItem item);
        UniTask<bool> BuyAsync(StoreItem item, CancellationToken cancellationToken);
        void Grant(StoreItem item);
    }
}
