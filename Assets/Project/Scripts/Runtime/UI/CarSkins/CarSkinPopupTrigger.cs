using UnityEngine;

namespace SBabchuk.Runtime.UI.CarSkins
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CarSkinPopupTrigger : MonoBehaviour
    {
        [SerializeField] private CarSkinPopupView _popup;

        private void OnEnable()
            => EasyTouch.On_TouchUp += OnTouchUp;

        private void OnDisable()
            => EasyTouch.On_TouchUp -= OnTouchUp;

        private void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
                _popup.Open();
        }
    }
}
