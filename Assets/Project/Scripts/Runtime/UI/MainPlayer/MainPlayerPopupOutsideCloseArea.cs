using SBabchuk.Runtime.UI.WeaponStore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public sealed class MainPlayerPopupOutsideCloseArea : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private BuyPlayerController _owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_owner)
                _owner.Hide();
        }
    }
}
