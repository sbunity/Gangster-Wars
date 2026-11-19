using System.Threading;
using Cysharp.Threading.Tasks;
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

        public static UniTask ShowPopupAsync(this GameObject popup, CancellationToken cancellationToken)
        {
            if (popup.TryGetComponent(out UIPopupAnimation animation))
                return animation.ShowAsync(cancellationToken);

            popup.SetActive(true);
            return UniTask.CompletedTask;
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
