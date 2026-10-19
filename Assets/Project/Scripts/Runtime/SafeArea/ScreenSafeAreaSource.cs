using UnityEngine;

namespace SBabchuk.Runtime.SafeArea
{
    public sealed class ScreenSafeAreaSource : IScreenSafeAreaSource
    {
        public Rect SafeArea => Screen.safeArea;
        public Vector2Int Resolution => new(Screen.width, Screen.height);
    }
}
