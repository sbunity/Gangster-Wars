using UnityEngine;

namespace SBabchuk.Runtime.UI.Popups
{
    public static class UIPopupExtensions
    {
        public static void ShowPopup(this GameObject popup)
        {
            if (popup.TryGetComponent(out UIPopupAnimation animation))
                animation.Show();
            else
                popup.SetActive(true);
        }

        public static void HidePopup(this GameObject popup)
        {
            if (popup.TryGetComponent(out UIPopupAnimation animation))
                animation.Hide();
            else
                popup.SetActive(false);
        }
    }
}
