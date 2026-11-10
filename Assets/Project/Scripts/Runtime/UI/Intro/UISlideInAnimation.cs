using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class UISlideInAnimation : UIIntroAnimation
    {
        [SerializeField] private UISlideSide _side = UISlideSide.Auto;
        [SerializeField] private bool _includeChildren = true;

        private readonly Vector3[] _corners = new Vector3[4];
        private RectTransform _rect;
        private RectTransform _canvasRect;
        private Vector2 _shownPosition;
        private bool _isDisplaced;

        private RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;

        private RectTransform CanvasRect => _canvasRect != null
            ? _canvasRect
            : _canvasRect = (RectTransform)GetComponentInParent<Canvas>(true).rootCanvas.transform;

        protected override void ApplyHidden(UIIntroStyle style)
        {
            if (_isDisplaced)
                Rect.anchoredPosition = _shownPosition;
            else
                _shownPosition = Rect.anchoredPosition;

            _isDisplaced = true;
            Rect.anchoredPosition = _shownPosition + ToParentSpace(HiddenOffsetOnCanvas(style.OffscreenMargin));
        }

        protected override Tween CreateShowTween(UIIntroStyle style)
        {
            ApplyHidden(style);
            return Rect.DOAnchorPos(_shownPosition, style.Duration)
                .SetEase(style.Ease, style.Overshoot)
                .OnComplete(() => _isDisplaced = false);
        }

        private Vector2 HiddenOffsetOnCanvas(float margin)
        {
            var screen = CanvasRect.rect;
            var bounds = BoundsOnCanvas();
            var left = new Vector2(screen.xMin - bounds.max.x - margin, 0f);
            var right = new Vector2(screen.xMax - bounds.min.x + margin, 0f);
            var bottom = new Vector2(0f, screen.yMin - bounds.max.y - margin);
            var top = new Vector2(0f, screen.yMax - bounds.min.y + margin);

            return _side switch
            {
                UISlideSide.Left => left,
                UISlideSide.Right => right,
                UISlideSide.Bottom => bottom,
                UISlideSide.Top => top,
                _ => Shorter(Shorter(left, right), Shorter(bottom, top)),
            };
        }

        private Bounds BoundsOnCanvas()
        {
            var bounds = new Bounds(CanvasRect.InverseTransformPoint(Rect.position), Vector3.zero);
            Encapsulate(ref bounds, Rect);

            if (_includeChildren)
            {
                foreach (var child in GetComponentsInChildren<RectTransform>())
                    Encapsulate(ref bounds, child);
            }

            return bounds;
        }

        private void Encapsulate(ref Bounds bounds, RectTransform target)
        {
            target.GetWorldCorners(_corners);
            foreach (var corner in _corners)
                bounds.Encapsulate(CanvasRect.InverseTransformPoint(corner));
        }

        private Vector2 ToParentSpace(Vector2 canvasOffset)
        {
            var parent = Rect.parent != null ? Rect.parent : CanvasRect;
            return parent.InverseTransformVector(CanvasRect.TransformVector(canvasOffset));
        }

        private static Vector2 Shorter(Vector2 first, Vector2 second)
            => second.sqrMagnitude < first.sqrMagnitude ? second : first;
    }
}
