using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.SafeArea;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class SafeAreaService : ISafeAreaService, ITickable
    {
        private static readonly Rect FullArea = new(0f, 0f, 1f, 1f);

        private readonly IScreenSafeAreaSource _source;
        private readonly SignalBus _signalBus;

        private Rect _safeArea;
        private Vector2Int _resolution;

        public SafeAreaService(IScreenSafeAreaSource source, SignalBus signalBus)
        {
            _source = source;
            _signalBus = signalBus;
            Capture();
        }

        public Rect NormalizedArea { get; private set; }

        public void Tick()
        {
            if (_source.SafeArea == _safeArea && _source.Resolution == _resolution)
                return;

            Capture();
            _signalBus.Fire(new SafeAreaChangedSignal(NormalizedArea));
        }

        private void Capture()
        {
            _safeArea = _source.SafeArea;
            _resolution = _source.Resolution;
            NormalizedArea = Normalize(_safeArea, _resolution);
        }

        private static Rect Normalize(Rect safeArea, Vector2Int resolution)
        {
            if (resolution.x <= 0 || resolution.y <= 0)
                return FullArea;

            var min = new Vector2(safeArea.xMin / resolution.x, safeArea.yMin / resolution.y);
            var max = new Vector2(safeArea.xMax / resolution.x, safeArea.yMax / resolution.y);

            return Rect.MinMaxRect(
                Mathf.Clamp01(min.x),
                Mathf.Clamp01(min.y),
                Mathf.Clamp01(max.x),
                Mathf.Clamp01(max.y));
        }
    }
}
