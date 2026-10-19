using UnityEngine;

namespace SBabchuk.Runtime.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class MinWidthScaler : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float _minWidth = 2666f;

        private RectTransform _rect;
        private float _appliedParentWidth = -1f;
        private float _appliedMinWidth = -1f;

        private void OnEnable()
        {
            _appliedParentWidth = -1f;
            Apply();
        }

        private void LateUpdate() => Apply();

        private void OnValidate() => _appliedParentWidth = -1f;

        private void Apply()
        {
            if (_rect == null)
                _rect = (RectTransform)transform;

            if (_rect.parent is not RectTransform parent)
                return;

            var parentWidth = parent.rect.width;
            if (parentWidth <= 0f)
                return;

            if (Mathf.Approximately(parentWidth, _appliedParentWidth) && Mathf.Approximately(_minWidth, _appliedMinWidth))
                return;

            _appliedParentWidth = parentWidth;
            _appliedMinWidth = _minWidth;

            var scale = Mathf.Min(1f, parentWidth / _minWidth);
            var width = parentWidth / scale;

            _rect.anchorMin = new Vector2(0f, _rect.anchorMin.y);
            _rect.anchorMax = new Vector2(1f, _rect.anchorMax.y);
            _rect.pivot = new Vector2(0.5f, _rect.pivot.y);
            _rect.anchoredPosition = new Vector2(0f, _rect.anchoredPosition.y);
            _rect.sizeDelta = new Vector2(width - parentWidth, _rect.sizeDelta.y);
            _rect.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
