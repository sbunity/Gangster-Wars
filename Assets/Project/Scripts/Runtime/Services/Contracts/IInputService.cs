using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IInputService
    {
        bool IsPointerPressed { get; }
        Vector2 PointerPosition { get; }
        Vector2 WorldPointerPosition { get; }
    }
}
