using UnityEngine;

namespace SBabchuk.Runtime.SafeArea
{
    public interface IScreenSafeAreaSource
    {
        Rect SafeArea { get; }
        Vector2Int Resolution { get; }
    }
}
