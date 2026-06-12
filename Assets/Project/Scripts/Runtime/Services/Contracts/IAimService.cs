using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAimService
    {
        Vector2 CurrentAimPosition { get; }
    }
}
