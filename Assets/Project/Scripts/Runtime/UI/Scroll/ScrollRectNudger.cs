using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.Scroll
{
    public sealed class ScrollRectNudger
    {
        private const float EdgeEpsilon = 0.0005f;

        private readonly ScrollRect _scrollRect;

        public ScrollRectNudger(ScrollRect scrollRect)
        {
            _scrollRect = scrollRect;
        }

        public void Nudge(ScrollNudgeDirection direction, float pixels)
        {
            if (_scrollRect == null || pixels <= 0f)
                return;

            var range = ScrollableRange(direction);
            if (range <= 0f)
                return;

            var delta = pixels / range;

            if (IsVertical(direction))
            {
                var sign = direction == ScrollNudgeDirection.Up ? 1f : -1f;
                _scrollRect.verticalNormalizedPosition =
                    Mathf.Clamp01(_scrollRect.verticalNormalizedPosition + sign * delta);
            }
            else
            {
                var sign = direction == ScrollNudgeDirection.Right ? 1f : -1f;
                _scrollRect.horizontalNormalizedPosition =
                    Mathf.Clamp01(_scrollRect.horizontalNormalizedPosition + sign * delta);
            }

            _scrollRect.velocity = Vector2.zero;
        }

        public bool CanScroll(ScrollNudgeDirection direction)
        {
            if (_scrollRect == null || ScrollableRange(direction) <= 0f)
                return false;

            return direction switch
            {
                ScrollNudgeDirection.Up => _scrollRect.verticalNormalizedPosition < 1f - EdgeEpsilon,
                ScrollNudgeDirection.Down => _scrollRect.verticalNormalizedPosition > EdgeEpsilon,
                ScrollNudgeDirection.Right => _scrollRect.horizontalNormalizedPosition < 1f - EdgeEpsilon,
                ScrollNudgeDirection.Left => _scrollRect.horizontalNormalizedPosition > EdgeEpsilon,
                _ => false,
            };
        }

        private float ScrollableRange(ScrollNudgeDirection direction)
        {
            var content = _scrollRect.content;
            var viewport = _scrollRect.viewport != null
                ? _scrollRect.viewport
                : (RectTransform)_scrollRect.transform;

            if (content == null || viewport == null)
                return 0f;

            return IsVertical(direction)
                ? content.rect.height - viewport.rect.height
                : content.rect.width - viewport.rect.width;
        }

        private static bool IsVertical(ScrollNudgeDirection direction) =>
            direction == ScrollNudgeDirection.Up || direction == ScrollNudgeDirection.Down;
    }
}
