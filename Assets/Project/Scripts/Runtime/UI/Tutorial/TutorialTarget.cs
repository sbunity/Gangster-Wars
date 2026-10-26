using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.Tutorial
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class TutorialTarget : MonoBehaviour, ITutorialAnchor
    {
        [SerializeField] private TutorialTargetId _id = TutorialTargetId.None;
        [SerializeField] private int _index;
        [SerializeField] private bool _fitActiveChildren;

        private readonly Vector3[] _corners = new Vector3[4];
        private ITutorialTargetRegistry _registry;
        private RectTransform _rectTransform;
        private Camera _camera;
        private bool _isCameraResolved;
        private bool _isRegistered;

        public bool IsValid => this != null && isActiveAndEnabled;

        private TutorialTargetKey Key => new(_id, _index);

        public Rect ScreenRect
        {
            get
            {
                var camera = ResolveCamera();
                var bounds = new ScreenBounds();

                if (_fitActiveChildren)
                {
                    foreach (Transform child in transform)
                    {
                        if (child.gameObject.activeInHierarchy && child is RectTransform childRect)
                            Encapsulate(childRect, camera, ref bounds);
                    }
                }

                if (!bounds.HasValue)
                    Encapsulate(RectTransform, camera, ref bounds);

                return Rect.MinMaxRect(bounds.Min.x, bounds.Min.y, bounds.Max.x, bounds.Max.y);
            }
        }

        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = (RectTransform)transform;

                return _rectTransform;
            }
        }

        [Inject]
        public void Construct(ITutorialTargetRegistry registry)
        {
            _registry = registry;
            TryRegister();
        }

        private void OnEnable() => TryRegister();

        private void OnDisable() => Unregister();

        private void TryRegister()
        {
            if (_isRegistered || _registry == null || !isActiveAndEnabled || _id == TutorialTargetId.None)
                return;

            _registry.Register(Key, this);
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered || _registry == null)
                return;

            _registry.Unregister(Key, this);
            _isRegistered = false;
        }

        private void Encapsulate(RectTransform rect, Camera camera, ref ScreenBounds bounds)
        {
            rect.GetWorldCorners(_corners);
            foreach (var corner in _corners)
                bounds.Add(RectTransformUtility.WorldToScreenPoint(camera, corner));
        }

        private Camera ResolveCamera()
        {
            if (_isCameraResolved)
                return _camera;

            var canvas = GetComponentInParent<Canvas>();
            var rootCanvas = canvas != null ? canvas.rootCanvas : null;
            _camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera
                : null;
            _isCameraResolved = true;
            return _camera;
        }

        private struct ScreenBounds
        {
            public bool HasValue;
            public Vector2 Min;
            public Vector2 Max;

            public void Add(Vector2 point)
            {
                Min = HasValue ? Vector2.Min(Min, point) : point;
                Max = HasValue ? Vector2.Max(Max, point) : point;
                HasValue = true;
            }
        }
    }
}
