using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Tutorial
{
    public sealed class TutorialPointerView : MonoBehaviour
    {
        [SerializeField] private RectTransform _hand;
        [SerializeField] private CanvasGroup _handGroup;
        [SerializeField] private RectTransform _ripple;
        [SerializeField] private CanvasGroup _rippleGroup;
        [SerializeField] private Vector2 _restOffset = new(70f, -70f);
        [SerializeField] private Vector2 _dragOffset = new(-160f, 60f);
        [SerializeField, Range(0.5f, 1f)] private float _pressScale = 0.85f;
        [SerializeField, Min(0.01f)] private float _moveDuration = 0.3f;
        [SerializeField, Min(0.01f)] private float _dragDuration = 0.8f;
        [SerializeField, Min(0.01f)] private float _rippleDuration = 0.5f;

        private Sequence _sequence;
        private Tween _rippleTween;
        private TutorialGesture _gesture = TutorialGesture.None;
        private float _verticalSign = 1f;
        private Vector2? _dragTarget;

        public bool HasFootprint => _gesture != TutorialGesture.None && isActiveAndEnabled;

        public Rect Footprint
        {
            get
            {
                var footprint = Union(HandRectAt(Offset(_restOffset)), HandRectAt(Vector2.zero));
                return _gesture == TutorialGesture.HoldAndDrag
                    ? Union(footprint, HandRectAt(DragOffset))
                    : footprint;
            }
        }

        private Vector2 DragOffset => _dragTarget ?? _dragOffset;

        private void OnDisable() => StopAnimation();

        public void Play(TutorialGesture gesture, bool pointDown)
        {
            StopAnimation();
            _gesture = gesture;
            _verticalSign = pointDown ? -1f : 1f;

            var isVisible = gesture != TutorialGesture.None;
            gameObject.SetActive(isVisible);
            if (!isVisible)
                return;

            _sequence = gesture switch
            {
                TutorialGesture.Tap => BuildTap(),
                TutorialGesture.HoldAndDrag => BuildHoldAndDrag(),
                _ => BuildPoint(),
            };
            _sequence.SetLoops(-1).SetUpdate(true);
        }

        public void Stop()
        {
            StopAnimation();
            _gesture = TutorialGesture.None;
            _dragTarget = null;
        }

        public void SetDragTarget(Vector2? localOffset)
            => _dragTarget = localOffset;

        private void StopAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
            _rippleTween?.Kill();
            _rippleTween = null;
            ResetPose(_restOffset);
        }

        private Sequence BuildTap()
            => DOTween.Sequence()
                .AppendCallback(() => ResetPose(_restOffset))
                .Append(_hand.DOAnchorPos(Vector2.zero, _moveDuration).SetEase(Ease.OutQuad))
                .Append(_hand.DOScale(Scale(_pressScale), 0.12f))
                .AppendCallback(PlayRipple)
                .AppendInterval(0.12f)
                .Append(_hand.DOScale(Scale(1f), 0.12f))
                .Append(_hand.DOAnchorPos(Offset(_restOffset), _moveDuration).SetEase(Ease.InOutQuad))
                .AppendInterval(0.35f);

        private Sequence BuildHoldAndDrag()
            => DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ResetPose(_restOffset);
                    SetHandAlpha(0f);
                })
                .Append(_handGroup.DOFade(1f, 0.15f))
                .Join(_hand.DOAnchorPos(Vector2.zero, _moveDuration).SetEase(Ease.OutQuad))
                .Append(_hand.DOScale(Scale(_pressScale), 0.12f))
                .AppendCallback(PlayRipple)
                .AppendInterval(0.2f)
                .Append(DOVirtual.Float(0f, 1f, _dragDuration, progress => _hand.anchoredPosition = DragOffset * progress).SetEase(Ease.InOutSine))
                .Append(_hand.DOScale(Scale(1f), 0.12f))
                .Append(_handGroup.DOFade(0f, 0.2f))
                .AppendInterval(0.25f);

        private Sequence BuildPoint()
            => DOTween.Sequence()
                .AppendCallback(() => ResetPose(_restOffset))
                .Append(_hand.DOAnchorPos(Offset(_restOffset * 0.45f), 0.45f).SetEase(Ease.InOutSine))
                .Append(_hand.DOAnchorPos(Offset(_restOffset), 0.45f).SetEase(Ease.InOutSine));

        private void PlayRipple()
        {
            if (_ripple == null || _rippleGroup == null)
                return;

            _rippleTween?.Kill();
            _ripple.localScale = Vector3.one * 0.4f;
            _rippleGroup.alpha = 0.9f;
            _rippleTween = DOTween.Sequence()
                .Append(_ripple.DOScale(1.6f, _rippleDuration).SetEase(Ease.OutQuad))
                .Join(_rippleGroup.DOFade(0f, _rippleDuration))
                .SetUpdate(true);
        }

        private void ResetPose(Vector2 offset)
        {
            if (_hand != null)
            {
                _hand.anchoredPosition = Offset(offset);
                _hand.localScale = Scale(1f);
            }

            SetHandAlpha(1f);

            if (_rippleGroup != null)
                _rippleGroup.alpha = 0f;
        }

        private void SetHandAlpha(float alpha)
        {
            if (_handGroup != null)
                _handGroup.alpha = alpha;
        }

        private Vector2 Offset(Vector2 offset)
            => new(offset.x, offset.y * _verticalSign);

        private Vector3 Scale(float scale)
            => new(scale, scale * _verticalSign, 1f);

        private Rect HandRectAt(Vector2 position)
        {
            var rect = _hand.rect;
            var yMin = _verticalSign > 0f ? rect.yMin : -rect.yMax;
            var yMax = _verticalSign > 0f ? rect.yMax : -rect.yMin;
            return Rect.MinMaxRect(position.x + rect.xMin, position.y + yMin, position.x + rect.xMax, position.y + yMax);
        }

        private static Rect Union(Rect a, Rect b)
            => Rect.MinMaxRect(Mathf.Min(a.xMin, b.xMin), Mathf.Min(a.yMin, b.yMin), Mathf.Max(a.xMax, b.xMax), Mathf.Max(a.yMax, b.yMax));
    }
}
