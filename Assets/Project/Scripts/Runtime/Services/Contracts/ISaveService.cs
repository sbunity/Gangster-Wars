using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISaveService
    {
        UniTask LoadAsync();
        UniTask SaveAsync();
        UniTask SaveAsync(ScriptableObject objectToPersist);
    }
}
