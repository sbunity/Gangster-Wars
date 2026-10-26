using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.Tutorial
{
    public sealed class TutorialOverlayView : MonoBehaviour, ITutorialView
    {
        private static readonly int DimColorId = Shader.PropertyToID("_DimColor");
        private static readonly int FrameColorId = Shader.PropertyToID("_FrameColor");
        private static readonly int FrameWidthId = Shader.PropertyToID("_FrameWidth");
        private static readonly int RectSizeId = Shader.PropertyToID("_RectSize");
        private static readonly int HoleCenterId = Shader.PropertyToID("_HoleCenter");
        private static readonly int HoleHalfSizeId = Shader.PropertyToID("_HoleHalfSize");
        private static readonly int HoleRadiusId = Shader.PropertyToID("_HoleRadius");
        private static readonly int HoleSoftnessId = Shader.PropertyToID("_HoleSoftness");
        private static readonly int HoleEnabledId = Shader.PropertyToID("_HoleEnabled");
        private static readonly int Hole2CenterId = Shader.PropertyToID("_Hole2Center");
        private static readonly int Hole2HalfSizeId = Shader.PropertyToID("_Hole2HalfSize");
        private static readonly int Hole2EnabledId = Shader.PropertyToID("_Hole2Enabled");

        [SerializeField] private GameObject _content;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RawImage _spotlight;
        [SerializeField] private TutorialHoleRaycastFilter _raycastFilter;
        [SerializeField] private TutorialPointerView _pointer;
        [SerializeField] private RectTransform _caption;
        [SerializeField] private TMP_Text _captionText;

        [Header("Spotlight")]
        [SerializeField] private Color _dimColor = new(0f, 0f, 0f, 0.65f);
        [SerializeField] private Color _frameColor = new(1f, 0.76f, 0.1f, 1f);
        [SerializeField, Min(0f)] private float _frameWidth = 6f;
        [SerializeField, Min(0f)] private float _holePadding = 18f;
        [SerializeField, Min(0f)] private float _holePulse = 10f;
        [SerializeField, Min(0f)] private float _holeCornerRadius = 28f;
        [SerializeField, Min(0f)] private float _holeSoftness = 24f;
        [SerializeField, Min(0.1f)] private float _pulseDuration = 0.6f;

        [Header("Layout")]
        [SerializeField, Min(0f)] private float _captionSpacing = 36f;
        [SerializeField, Min(0f)] private float _screenMargin = 32f;
        [SerializeField, Range(0f, 1f)] private float _pointDownBelowScreenHeight = 0.35f;
        [SerializeField, Min(0f)] private float _fadeDuration = 0.2f;

        private readonly Rect[] _captionCandidates = new Rect[6];
        private Material _spotlightMaterial;
        private RectTransform _space;
        private Camera _uiCamera;
        private TutorialHint _hint;
        private Tween _fadeTween;
        private Tween _pulseTween;
        private float _pulse;

        private void Awake()
        {
            _space = _spotlight.rectTransform;
            _spotlightMaterial = new Material(_spotlight.material);
            _spotlight.material = _spotlightMaterial;
            _uiCamera = ResolveUiCamera();
            HideImmediate();
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _pulseTween?.Kill();

            if (_spotlightMaterial != null)
                Destroy(_spotlightMaterial);
        }

        private void LateUpdate()
        {
            if (_hint != null)
                UpdateLayout();
        }

        public void Show(TutorialHint hint)
        {
            if (_content == null)
                return;

            if (hint == null)
            {
                Hide();
                return;
            }

            _hint = hint;
            _content.SetActive(true);
            _spotlight.raycastTarget = hint.InputMode == TutorialInputMode.TargetOnly;
            _spotlightMaterial.SetColor(DimColorId, hint.DimBackground ? _dimColor : Color.clear);
            _spotlightMaterial.SetColor(FrameColorId, _frameColor);
            _spotlightMaterial.SetFloat(FrameWidthId, _frameWidth);
            _spotlightMaterial.SetFloat(HoleRadiusId, _holeCornerRadius);
            _spotlightMaterial.SetFloat(HoleSoftnessId, _holeSoftness);

            var hasCaption = !string.IsNullOrEmpty(hint.Caption);
            _caption.gameObject.SetActive(hasCaption);
            if (hasCaption)
            {
                _captionText.text = hint.Caption;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_caption);
            }

            _pointer.Play(hint.Gesture, ShouldPointDown(hint.Anchor));
            StartPulse();
            UpdateLayout();
            Fade(1f, null);
        }

        public void Hide()
        {
            if (_content == null || (_hint == null && !_content.activeSelf))
                return;

            _hint = null;
            _spotlight.raycastTarget = false;
            _raycastFilter.ClearHole();
            _pulseTween?.Kill();
            Fade(0f, () =>
            {
                _pointer.Stop();
                _content.SetActive(false);
            });
        }

        private void HideImmediate()
        {
            _hint = null;
            _fadeTween?.Kill();
            _pulseTween?.Kill();
            _spotlight.raycastTarget = false;
            _raycastFilter.ClearHole();
            _pointer.Stop();
            _canvasGroup.alpha = 0f;
            _content.SetActive(false);
        }

        private void UpdateLayout()
        {
            _spotlightMaterial.SetVector(RectSizeId, _space.rect.size);

            if (!TryGetLocalRect(_hint.Anchor, out var target))
            {
                _spotlightMaterial.SetFloat(HoleEnabledId, 0f);
                _spotlightMaterial.SetFloat(Hole2EnabledId, 0f);
                _raycastFilter.ClearHole();
                return;
            }

            var padding = _holePadding + _holePulse * _pulse;
            var hole = Expand(target, padding);
            SetHole(HoleCenterId, HoleHalfSizeId, hole);
            _spotlightMaterial.SetFloat(HoleEnabledId, 1f);
            _raycastFilter.SetHole(_space, hole);
            _pointer.transform.position = _space.TransformPoint(target.center);

            var hasDestination = TryGetLocalRect(_hint.Destination, out var destination);
            if (hasDestination)
                SetHole(Hole2CenterId, Hole2HalfSizeId, Expand(destination, padding));
            _spotlightMaterial.SetFloat(Hole2EnabledId, hasDestination ? 1f : 0f);
            _pointer.SetDragTarget(hasDestination ? destination.center - target.center : null);

            if (!_caption.gameObject.activeSelf)
                return;

            var maxPadding = _holePadding + _holePulse;
            var destinationHole = hasDestination ? Expand(destination, maxPadding) : (Rect?)null;
            PlaceCaption(Expand(target, maxPadding), destinationHole, target.center);
        }

        private void SetHole(int centerId, int halfSizeId, Rect hole)
        {
            _spotlightMaterial.SetVector(centerId, hole.center - _space.rect.min);
            _spotlightMaterial.SetVector(halfSizeId, hole.size * 0.5f);
        }

        private bool TryGetLocalRect(ITutorialAnchor anchor, out Rect localRect)
        {
            var isValid = anchor != null && anchor.IsValid;
            localRect = isValid ? ScreenToLocalRect(anchor.ScreenRect) : default;
            return isValid;
        }

        private void PlaceCaption(Rect hole, Rect? destinationHole, Vector2 pointerOrigin)
        {
            var bounds = _space.rect;
            var size = _caption.rect.size;
            var hasHand = _pointer.HasFootprint;
            var hand = hasHand ? Offset(_pointer.Footprint, pointerOrigin) : Rect.zero;

            var aboveY = hole.yMax + _captionSpacing + size.y * 0.5f;
            var belowY = hole.yMin - _captionSpacing - size.y * 0.5f;
            var leftOfCenterX = hole.center.x - size.x * 0.5f;
            _captionCandidates[0] = CaptionRect(new Vector2(hole.center.x, aboveY), size, bounds);
            _captionCandidates[1] = CaptionRect(new Vector2(hole.center.x, belowY), size, bounds);
            _captionCandidates[2] = CaptionRect(new Vector2(hole.xMax + _captionSpacing + size.x * 0.5f, hole.center.y), size, bounds);
            _captionCandidates[3] = CaptionRect(new Vector2(hole.xMin - _captionSpacing - size.x * 0.5f, hole.center.y), size, bounds);
            _captionCandidates[4] = CaptionRect(new Vector2(leftOfCenterX, aboveY), size, bounds);
            _captionCandidates[5] = CaptionRect(new Vector2(leftOfCenterX, belowY), size, bounds);

            var chosen = _captionCandidates[0];
            var smallestHandOverlap = float.MaxValue;
            foreach (var candidate in _captionCandidates)
            {
                if (candidate.Overlaps(hole) || (destinationHole.HasValue && candidate.Overlaps(destinationHole.Value)))
                    continue;

                var handOverlap = hasHand ? OverlapArea(candidate, hand) : 0f;
                if (handOverlap >= smallestHandOverlap)
                    continue;

                chosen = candidate;
                smallestHandOverlap = handOverlap;
                if (handOverlap <= 0f)
                    break;
            }

            _caption.position = _space.TransformPoint(chosen.center);
        }

        private Rect CaptionRect(Vector2 center, Vector2 size, Rect bounds)
        {
            var halfWidth = size.x * 0.5f + _screenMargin;
            var halfHeight = size.y * 0.5f + _screenMargin;
            center.x = Mathf.Clamp(center.x, bounds.xMin + halfWidth, Mathf.Max(bounds.xMin + halfWidth, bounds.xMax - halfWidth));
            center.y = Mathf.Clamp(center.y, bounds.yMin + halfHeight, Mathf.Max(bounds.yMin + halfHeight, bounds.yMax - halfHeight));
            return new Rect(center - size * 0.5f, size);
        }

        private bool ShouldPointDown(ITutorialAnchor anchor)
            => anchor != null && anchor.IsValid && anchor.ScreenRect.center.y < Screen.height * _pointDownBelowScreenHeight;

        private void StartPulse()
        {
            _pulseTween?.Kill();
            _pulse = 0f;
            _pulseTween = DOTween.To(() => _pulse, value => _pulse = value, 1f, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void Fade(float alpha, TweenCallback onComplete)
        {
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(alpha, _fadeDuration).SetUpdate(true);
            if (onComplete != null)
                _fadeTween.OnComplete(onComplete);
        }

        private Rect ScreenToLocalRect(Rect screenRect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_space, screenRect.min, _uiCamera, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_space, screenRect.max, _uiCamera, out var max);
            return Rect.MinMaxRect(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y), Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));
        }

        private Camera ResolveUiCamera()
        {
            var canvas = GetComponentInParent<Canvas>();
            var rootCanvas = canvas != null ? canvas.rootCanvas : null;
            return rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera
                : null;
        }

        private static Rect Expand(Rect rect, float padding)
            => new(rect.xMin - padding, rect.yMin - padding, rect.width + padding * 2f, rect.height + padding * 2f);

        private static Rect Offset(Rect rect, Vector2 offset)
            => new(rect.position + offset, rect.size);

        private static float OverlapArea(Rect a, Rect b)
        {
            var width = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin);
            var height = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
            return width > 0f && height > 0f ? width * height : 0f;
        }
    }
}
