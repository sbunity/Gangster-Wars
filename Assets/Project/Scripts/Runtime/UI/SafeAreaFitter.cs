using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.SafeArea;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private SafeAreaEdges _edges = SafeAreaEdges.All;

        private ISafeAreaService _safeArea;
        private SignalSubscriptions _signals;
        private RectTransform _rect;

        [Inject]
        public void Construct(ISafeAreaService safeArea, SignalBus signalBus)
        {
            _safeArea = safeArea;
            _signals = new SignalSubscriptions(signalBus)
                .Add<SafeAreaChangedSignal>(OnSafeAreaChanged);
        }

        private void Awake()
        {
            _rect = (RectTransform)transform;
        }

        private void OnEnable()
        {
            _signals?.Enable();

            if (_safeArea != null)
                Apply(_safeArea.NormalizedArea);
        }

        private void OnDisable() => _signals?.Disable();

        private void OnSafeAreaChanged(SafeAreaChangedSignal signal) => Apply(signal.NormalizedArea);

        private void Apply(Rect area)
        {
            _rect.anchorMin = new Vector2(
                HasEdge(SafeAreaEdges.Left) ? area.xMin : 0f,
                HasEdge(SafeAreaEdges.Bottom) ? area.yMin : 0f);
            _rect.anchorMax = new Vector2(
                HasEdge(SafeAreaEdges.Right) ? area.xMax : 1f,
                HasEdge(SafeAreaEdges.Top) ? area.yMax : 1f);
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }

        private bool HasEdge(SafeAreaEdges edge) => (_edges & edge) == edge;
    }
}
