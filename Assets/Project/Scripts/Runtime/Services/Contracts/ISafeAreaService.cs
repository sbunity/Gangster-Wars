using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISafeAreaService
    {
        Rect NormalizedArea { get; }
    }
}
