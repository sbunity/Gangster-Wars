using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ITutorialAnchor
    {
        bool IsValid { get; }

        Rect ScreenRect { get; }
    }
}
