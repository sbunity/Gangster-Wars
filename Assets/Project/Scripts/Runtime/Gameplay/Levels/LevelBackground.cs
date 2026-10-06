using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelBackground : MonoBehaviour
    {
        [SerializeField] private List<Transform> _spawnPoints = new();
        [SerializeField] private List<Transform> _targetPoints = new();
        [SerializeField] private Transform _barricadePoint;
        [SerializeField] private Transform _leaderPoint;
        [SerializeField] private Transform _sniperPoint;
        [SerializeField] private Transform _bomberPoint;

        public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;
        public IReadOnlyList<Transform> TargetPoints => _targetPoints;
        public int PathCount => Mathf.Min(_spawnPoints.Count, _targetPoints.Count);

        public Transform BarricadePoint => _barricadePoint;
        public Transform LeaderPoint => _leaderPoint;
        public Transform SniperPoint => _sniperPoint;
        public Transform BomberPoint => _bomberPoint;
    }
}
