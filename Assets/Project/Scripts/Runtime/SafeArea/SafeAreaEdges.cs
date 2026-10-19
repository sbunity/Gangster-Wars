using System;

namespace SBabchuk.Runtime.SafeArea
{
    [Flags]
    public enum SafeAreaEdges
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        Horizontal = Left | Right,
        Vertical = Top | Bottom,
        All = Horizontal | Vertical
    }
}
