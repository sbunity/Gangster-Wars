using UnityEngine;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    [DefaultExecutionOrder(100)]
    public sealed class MainPlayerPopupPlacement : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector2 _targetOffset;
        [SerializeField] private Vector2 _targetSize = Vector2.one;
        [SerializeField, Range(0f, 1f)] private float _pointHeight = 0.5f;
        [SerializeField] private RectTransform _bubble;
        [SerializeField, Range(0f, 1f)] private float _tailHeight = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _bodyCenter = 0.5f;
        [SerializeField] private RectTransform[] _frame;
        [SerializeField] private RectTransform _bounds;
        [SerializeField] private RectTransform[] _rightLimits;
        [SerializeField] private float _margin = 24f;

        private readonly Vector3[] _corners = new Vector3[4];
        private RectTransform _rect;
        private RectTransform _parent;
        private Camera _camera;
        private Vector2 _bubblePosition;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _parent = (RectTransform)transform.parent;
            _camera = GetComponentInParent<Canvas>().rootCanvas.worldCamera;
            if (_bubble)
                _bubblePosition = _bubble.anchoredPosition;
        }

        private void LateUpdate()
        {
            Place();
        }

        private void Place()
        {
            if (!_target || !_bubble || !_camera)
                return;

            var area = GetArea();
            var rightOverflow = Arrange(false, area);
            if (rightOverflow > 0f && Arrange(true, area) >= rightOverflow)
                Arrange(false, area);

            ClampInto(area);
        }

        private float Arrange(bool mirrored, Rect area)
        {
            var scale = _bubble.localScale;
            scale.x = Mathf.Abs(scale.x) * (mirrored ? -1f : 1f);
            _bubble.localScale = scale;
            var bodyShift = mirrored ? (2f * _bodyCenter - 1f) * _bubble.rect.width : 0f;
            _bubble.anchoredPosition = _bubblePosition + new Vector2(bodyShift, 0f);

            _rect.localPosition += (Vector3)(GetTargetPoint(mirrored) - GetTailPoint());

            var frame = GetFrame();
            return Mathf.Max(0f, area.xMin - frame.xMin) + Mathf.Max(0f, frame.xMax - area.xMax);
        }

        private void ClampInto(Rect area)
        {
            var frame = GetFrame();
            var shift = new Vector2(Shift(frame.xMin, frame.xMax, area.xMin, area.xMax), Shift(frame.yMin, frame.yMax, area.yMin, area.yMax));
            _rect.localPosition += (Vector3)shift;
        }

        private static float Shift(float min, float max, float areaMin, float areaMax)
        {
            if (min < areaMin)
                return areaMin - min;

            return max > areaMax ? Mathf.Max(areaMax - max, areaMin - min) : 0f;
        }

        private Vector2 GetTargetPoint(bool leftSide)
        {
            var center = _target.position + (Vector3)_targetOffset;
            var x = _targetSize.x * (leftSide ? -0.5f : 0.5f);
            var y = _targetSize.y * (_pointHeight - 0.5f);
            Vector2 screenPoint = _camera.WorldToScreenPoint(center + new Vector3(x, y, 0f));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screenPoint, _camera, out var localPoint);
            return localPoint;
        }

        private Vector2 GetTailPoint()
        {
            var rect = _bubble.rect;
            var tail = new Vector3(rect.xMin, rect.yMin + rect.height * _tailHeight, 0f);
            return _parent.InverseTransformPoint(_bubble.TransformPoint(tail));
        }

        private Rect GetArea()
        {
            var area = GetLocalRect(_bounds);
            area.xMin += _margin;
            area.yMin += _margin;
            area.xMax -= _margin;
            area.yMax -= _margin;

            if (_rightLimits == null)
                return area;

            foreach (var limit in _rightLimits)
            {
                if (limit && limit.gameObject.activeInHierarchy)
                    area.xMax = Mathf.Min(area.xMax, GetLocalRect(limit).xMin - _margin);
            }

            return area;
        }

        private Rect GetFrame()
        {
            var frame = GetLocalRect(_bubble);
            if (_frame == null)
                return frame;

            foreach (var part in _frame)
            {
                if (!part || !part.gameObject.activeInHierarchy)
                    continue;

                var rect = GetLocalRect(part);
                frame = Rect.MinMaxRect(
                    Mathf.Min(frame.xMin, rect.xMin),
                    Mathf.Min(frame.yMin, rect.yMin),
                    Mathf.Max(frame.xMax, rect.xMax),
                    Mathf.Max(frame.yMax, rect.yMax));
            }

            return frame;
        }

        private Rect GetLocalRect(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_corners);
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var corner in _corners)
            {
                Vector2 point = _parent.InverseTransformPoint(corner);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_target)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_target.position + (Vector3)_targetOffset, _targetSize);
        }
    }
}
