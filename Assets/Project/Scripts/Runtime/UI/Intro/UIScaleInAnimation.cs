using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    public sealed class UIScaleInAnimation : UIIntroAnimation
    {
        [SerializeField, Range(0f, 1f)] private float _fromScale;

        private Vector3 _shownScale;
        private bool _isScaled;

        protected override void ApplyHidden(UIIntroStyle style)
        {
            if (!_isScaled)
                _shownScale = transform.localScale;

            _isScaled = true;
            transform.localScale = new Vector3(_shownScale.x * _fromScale, _shownScale.y * _fromScale, _shownScale.z);
        }

        protected override Tween CreateShowTween(UIIntroStyle style)
        {
            ApplyHidden(style);
            return transform.DOScale(_shownScale, style.Duration)
                .SetEase(style.Ease, style.Overshoot)
                .OnComplete(() => _isScaled = false);
        }
    }
}
