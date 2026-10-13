using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Barricades
{
    public sealed class BarricadeHitArea : MonoBehaviour, IBarricadeTarget
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField, Min(0f)] private float _hitHeight = 1f;

        public Vector2 GetHitPoint(Vector2 lanePoint)
        {
            var point = lanePoint + Vector2.up * _hitHeight;
            return _collider != null ? _collider.ClosestPoint(point) : point;
        }
    }
}
