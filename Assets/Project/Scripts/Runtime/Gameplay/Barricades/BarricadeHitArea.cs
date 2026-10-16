using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Barricades
{
    public sealed class BarricadeHitArea : MonoBehaviour, IBarricadeTarget
    {
        [SerializeField] private PolygonCollider2D _collider;
        [SerializeField, Min(0f)] private float _hitHeight = 1f;
        [SerializeField, Min(0.01f)] private float _shapeSliceStep = 0.1f;
        [SerializeField, Min(0f)] private float _shapeTolerance = 0.05f;

        private readonly List<Vector2> _shapePoints = new();
        private readonly List<Vector2[]> _spriteShapes = new();

        public void ApplyShape(Sprite sprite)
        {
            if (_collider == null || sprite == null)
                return;

            _spriteShapes.Clear();
            var shapeCount = sprite.GetPhysicsShapeCount();
            for (var i = 0; i < shapeCount; i++)
            {
                sprite.GetPhysicsShape(i, _shapePoints);
                _spriteShapes.Add(_shapePoints.ToArray());
            }

            var outline = BuildSliceOutline(_spriteShapes, sprite.bounds, _shapeSliceStep);
            if (outline.Count < 3)
                return;

            _collider.offset = Vector2.zero;
            _collider.pathCount = 1;
            _collider.SetPath(0, Simplify(outline, _shapeTolerance));
        }

        public Vector2 GetHitPoint(Vector2 lanePoint)
        {
            var point = lanePoint + Vector2.up * _hitHeight;
            return _collider != null ? _collider.ClosestPoint(point) : point;
        }

        public Vector2 GetContactPoint(Vector2 lanePoint)
        {
            if (_collider == null || _collider.pathCount == 0)
                return lanePoint;

            _collider.GetPath(0, _shapePoints);
            var colliderTransform = _collider.transform;

            for (var i = 0; i < _shapePoints.Count; i++)
                _shapePoints[i] = colliderTransform.TransformPoint(_shapePoints[i] + _collider.offset);

            return TryGetSpan(_shapePoints, lanePoint.y, out _, out var maxX)
                ? new Vector2(maxX, lanePoint.y)
                : _collider.ClosestPoint(lanePoint);
        }

        private List<Vector2> BuildSliceOutline(List<Vector2[]> shapes, Bounds bounds, float step)
        {
            var left = new List<Vector2>();
            var right = new List<Vector2>();

            for (var y = bounds.min.y + step * 0.5f; y < bounds.max.y; y += step)
            {
                var minX = float.MaxValue;
                var maxX = float.MinValue;
                var found = false;

                foreach (var shape in shapes)
                {
                    if (!TryGetSpan(shape, y, out var shapeMin, out var shapeMax))
                        continue;

                    minX = Mathf.Min(minX, shapeMin);
                    maxX = Mathf.Max(maxX, shapeMax);
                    found = true;
                }

                if (!found)
                    continue;

                left.Add(new Vector2(minX, y));
                right.Add(new Vector2(maxX, y));
            }

            right.Reverse();
            left.AddRange(right);
            return left;
        }

        private bool TryGetSpan(IReadOnlyList<Vector2> path, float y, out float minX, out float maxX)
        {
            minX = float.MaxValue;
            maxX = float.MinValue;

            for (var i = 0; i < path.Count; i++)
            {
                var a = path[i];
                var b = path[(i + 1) % path.Count];

                if (Mathf.Approximately(a.y, b.y) || y < Mathf.Min(a.y, b.y) || y > Mathf.Max(a.y, b.y))
                    continue;

                var x = Mathf.Lerp(a.x, b.x, (y - a.y) / (b.y - a.y));
                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x);
            }

            return maxX >= minX;
        }

        private Vector2[] Simplify(List<Vector2> points, float tolerance)
        {
            var keep = new bool[points.Count];
            keep[0] = true;
            keep[points.Count - 1] = true;
            MarkKeptPoints(points, 0, points.Count - 1, tolerance, keep);

            var result = new List<Vector2>();
            for (var i = 0; i < points.Count; i++)
            {
                if (keep[i])
                    result.Add(points[i]);
            }

            return result.ToArray();
        }

        private void MarkKeptPoints(List<Vector2> points, int first, int last, float tolerance, bool[] keep)
        {
            if (last - first < 2)
                return;

            var maxDistance = 0f;
            var index = first;

            for (var i = first + 1; i < last; i++)
            {
                var distance = DistanceToSegment(points[i], points[first], points[last]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    index = i;
                }
            }

            if (maxDistance <= tolerance)
                return;

            keep[index] = true;
            MarkKeptPoints(points, first, index, tolerance, keep);
            MarkKeptPoints(points, index, last, tolerance, keep);
        }

        private float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            var segment = b - a;
            var lengthSqr = segment.sqrMagnitude;
            if (lengthSqr <= Mathf.Epsilon)
                return Vector2.Distance(point, a);

            var t = Mathf.Clamp01(Vector2.Dot(point - a, segment) / lengthSqr);
            return Vector2.Distance(point, a + segment * t);
        }
    }
}
