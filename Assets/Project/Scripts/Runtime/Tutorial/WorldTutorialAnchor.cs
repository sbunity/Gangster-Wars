using System;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Tutorial
{
    public sealed class WorldTutorialAnchor : ITutorialAnchor
    {
        private readonly Func<bool> _isTargetValid;
        private readonly Func<Vector3> _position;
        private readonly Camera _camera;
        private readonly Vector2 _worldSize;

        public WorldTutorialAnchor(Transform target, Camera camera, Vector2 worldSize)
            : this(() => target != null && target.gameObject.activeInHierarchy, () => target.position, camera, worldSize)
        {
        }

        public WorldTutorialAnchor(Func<bool> isTargetValid, Func<Vector3> position, Camera camera, Vector2 worldSize)
        {
            _isTargetValid = isTargetValid;
            _position = position;
            _camera = camera;
            _worldSize = worldSize;
        }

        public bool IsValid => _camera != null && _isTargetValid();

        public Rect ScreenRect
        {
            get
            {
                if (!IsValid)
                    return Rect.zero;

                var center = _position();
                var halfSize = (Vector3)(_worldSize * 0.5f);
                Vector2 min = _camera.WorldToScreenPoint(center - halfSize);
                Vector2 max = _camera.WorldToScreenPoint(center + halfSize);
                return Rect.MinMaxRect(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y), Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));
            }
        }
    }
}
