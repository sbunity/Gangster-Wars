using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.FlyingCoins
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public sealed class CoinFlightView : MonoBehaviour
    {
        private RectTransform _rect;
        private Image _image;
        private Tween _tween;

        public RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;
        public Image Image => _image != null ? _image : _image = GetComponent<Image>();

        public void SetTween(Tween tween)
        {
            KillTween();
            _tween = tween;
        }

        public void KillTween()
        {
            _tween?.Kill();
            _tween = null;
        }

        private void OnDisable()
        {
            KillTween();
        }
    }
}
