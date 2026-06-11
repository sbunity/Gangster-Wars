using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class InputService : IInputService
    {
        public bool IsPointerPressed => Input.GetMouseButton(0);
        public Vector2 PointerPosition => Input.mousePosition;

        public Vector2 WorldPointerPosition
        {
            get
            {
                var camera = Camera.main;
                if (camera == null)
                    return Vector2.zero;

                return camera.ScreenToWorldPoint(PointerPosition);
            }
        }
    }
}
