using System;
using System.Linq;
using SBabchuk.Runtime.Architecture;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelEndBonusCollector : IInitializable, IDisposable
    {
        private readonly LevelEntityTracker _entityTracker;
        private readonly SignalSubscriptions _signals;

        public LevelEndBonusCollector(LevelEntityTracker entityTracker, SignalBus signalBus)
        {
            _entityTracker = entityTracker;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        public void Initialize()
        {
            _signals.Enable();
        }

        public void Dispose()
        {
            _signals.Disable();
        }

        private void OnGameFinished()
        {
            foreach (var bonus in _entityTracker.Bonuses.ToArray())
            {
                if (bonus != null)
                    bonus.CollectImmediately();

                _entityTracker.RemoveBonus(bonus);
            }
        }
    }
}
