using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.Scroll
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class ScrollNudgeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ScrollNudgeDirection _direction = ScrollNudgeDirection.Up;

        [Header("Motion (pixels)")]
        [SerializeField] private float _tapStep = 90f;
        [SerializeField] private float _tapDuration = 0.25f;
        [SerializeField] private Ease _tapEase = Ease.OutCubic;
        [SerializeField] private float _holdSpeed = 900f;
        [SerializeField] private float _holdDelay = 0.2f;

        [Header("Edge visibility")]
        [SerializeField] private bool _hideWhenAtEdge = true;

        private ScrollRectNudger _nudger;
        private Graphic _graphic;
        private Tween _tapTween;
        private bool _pressed;
        private float _pressedAt;

        private void Awake()
        {
            if (_scrollRect == null)
                _scrollRect = GetComponentInParent<ScrollRect>();

            _graphic = GetComponent<Graphic>();
            _nudger = _scrollRect != null ? new ScrollRectNudger(_scrollRect) : null;
        }

        private void OnDisable()
        {
            _pressed = false;
            KillTapTween();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_nudger == null)
                return;

            KillTapTween();
            _pressed = true;
            _pressedAt = Time.unscaledTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_pressed)
                return;

            if (_nudger != null && Time.unscaledTime - _pressedAt < _holdDelay)
                PlayTapStep();

            _pressed = false;
        }

        public void OnPointerExit(PointerEventData eventData) => _pressed = false;

        private void PlayTapStep()
        {
            KillTapTween();

            var moved = 0f;
            _tapTween = DOTween.To(() => moved, target =>
                {
                    _nudger.Nudge(_direction, target - moved);
                    moved = target;
                }, _tapStep, _tapDuration)
                .SetEase(_tapEase)
                .SetUpdate(true);
        }

        private void KillTapTween()
        {
            _tapTween?.Kill();
            _tapTween = null;
        }

        private void Update()
        {
            if (_nudger == null)
                return;

            if (_pressed)
                _nudger.Nudge(_direction, _holdSpeed * Time.unscaledDeltaTime);

            UpdateEdgeVisibility();
        }

        private void UpdateEdgeVisibility()
        {
            if (!_hideWhenAtEdge || _graphic == null)
                return;

            var canScroll = _nudger.CanScroll(_direction);
            if (_graphic.enabled == canScroll)
                return;

            _graphic.enabled = canScroll;
            _graphic.raycastTarget = canScroll;
            if (!canScroll)
                _pressed = false;
        }
    }
}
