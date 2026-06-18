using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.WeaponStore.Info
{
    public sealed class StoreInfoPopupView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _window;
        [SerializeField] private Text _titleText;
        [SerializeField] private Transform _statsRoot;
        [SerializeField] private StoreInfoStatRowView _rowTemplate;
        [SerializeField] private Button _closeButton;
        [SerializeField] private float _baseHeight = 170f;
        [SerializeField] private float _rowHeight = 50f;
        [SerializeField] private float _rowSpacing = 8f;
        [SerializeField] private float _minHeight = 245f;
        [SerializeField] private float _maxHeight = 390f;

        private readonly List<StoreInfoStatRowView> _rows = new();
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private RectTransform _openedFrom;
        private int _openedFrame;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas != null ? _canvas.transform as RectTransform : null;

            if (_closeButton)
                _closeButton.onClick.AddListener(Hide);

            if (_rowTemplate)
                _rowTemplate.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Time.frameCount == _openedFrame || !Input.GetMouseButtonDown(0))
                return;

            var screenPosition = (Vector2)Input.mousePosition;
            var eventCamera = GetEventCamera();

            if (_window != null && RectTransformUtility.RectangleContainsScreenPoint(_window, screenPosition, eventCamera))
                return;

            if (_openedFrom != null && RectTransformUtility.RectangleContainsScreenPoint(_openedFrom, screenPosition, eventCamera))
                return;

            Hide();
        }

        public void Show(StoreItemStatsSnapshot snapshot, RectTransform openedFrom)
        {
            if (snapshot == null)
                return;

            gameObject.SetActive(true);
            _openedFrom = openedFrom;
            _openedFrame = Time.frameCount;

            if (_titleText)
                _titleText.text = snapshot.Title;

            if (_icon)
            {
                _icon.sprite = snapshot.Icon;
                _icon.enabled = snapshot.Icon != null;
            }

            RenderRows(snapshot);
            ResizeToStats(snapshot.Stats.Count);
            PositionNear(openedFrom);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void RenderRows(StoreItemStatsSnapshot snapshot)
        {
            EnsureRows(snapshot.Stats.Count);

            for (var i = 0; i < _rows.Count; i++)
            {
                var isActive = i < snapshot.Stats.Count;
                _rows[i].gameObject.SetActive(isActive);

                if (isActive)
                    _rows[i].Render(snapshot.Stats[i]);
            }
        }

        private void EnsureRows(int count)
        {
            if (_rowTemplate == null || _statsRoot == null)
                return;

            while (_rows.Count < count)
            {
                var row = Instantiate(_rowTemplate, _statsRoot);
                _rows.Add(row);
            }
        }

        private void ResizeToStats(int statsCount)
        {
            if (_window == null)
                return;

            var rowsHeight = statsCount > 0
                ? statsCount * _rowHeight + Mathf.Max(0, statsCount - 1) * _rowSpacing
                : 0f;

            var targetHeight = Mathf.Clamp(_baseHeight + rowsHeight, _minHeight, _maxHeight);
            _window.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
        }

        private void PositionNear(RectTransform target)
        {
            if (_window == null || _canvasRect == null || target == null)
                return;

            Canvas.ForceUpdateCanvases();

            var eventCamera = GetEventCamera();
            var targetCorners = new Vector3[4];
            target.GetWorldCorners(targetCorners);

            var leftCenterScreen = (Vector2)(RectTransformUtility.WorldToScreenPoint(eventCamera, targetCorners[0]) +
                                            RectTransformUtility.WorldToScreenPoint(eventCamera, targetCorners[1])) * 0.5f;
            var rightCenterScreen = (Vector2)(RectTransformUtility.WorldToScreenPoint(eventCamera, targetCorners[2]) +
                                             RectTransformUtility.WorldToScreenPoint(eventCamera, targetCorners[3])) * 0.5f;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, leftCenterScreen, eventCamera, out var leftCenter);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, rightCenterScreen, eventCamera, out var rightCenter);

            const float gap = 14f;
            var windowSize = _window.rect.size;
            var canvasRect = _canvasRect.rect;
            var leftPosition = new Vector2(leftCenter.x - gap, leftCenter.y);

            if (leftPosition.x - windowSize.x < canvasRect.xMin)
            {
                _window.pivot = new Vector2(0f, 0.5f);
                _window.anchoredPosition = ClampPosition(new Vector2(rightCenter.x + gap, rightCenter.y), windowSize, _window.pivot, canvasRect);
                return;
            }

            _window.pivot = new Vector2(1f, 0.5f);
            _window.anchoredPosition = ClampPosition(leftPosition, windowSize, _window.pivot, canvasRect);
        }

        private Camera GetEventCamera()
        {
            if (_canvas == null || _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return _canvas.worldCamera;
        }

        private Vector2 ClampPosition(Vector2 position, Vector2 size, Vector2 pivot, Rect bounds)
        {
            var minX = bounds.xMin + size.x * pivot.x;
            var maxX = bounds.xMax - size.x * (1f - pivot.x);
            var minY = bounds.yMin + size.y * pivot.y;
            var maxY = bounds.yMax - size.y * (1f - pivot.y);

            return new Vector2(
                Mathf.Clamp(position.x, minX, maxX),
                Mathf.Clamp(position.y, minY, maxY));
        }
    }
}
