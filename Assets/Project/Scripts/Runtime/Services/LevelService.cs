using System;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.Levels;

namespace SBabchuk.Runtime.Services
{
    public sealed class LevelService : ILevelFlowService
    {
        private readonly SignalBus _signalBus;
        public event Action<Panels> Finished;
        public bool IsFinished { get; private set; }
        public Panels LastPanel { get; private set; } = Panels.None;

        public LevelService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Start(Level level)
        {
            IsFinished = false;
            LastPanel = Panels.None;
        }

        public void Finish(Panels panel)
        {
            if (IsFinished)
                return;
                
            IsFinished = true;
            LastPanel = panel;
            Finished?.Invoke(panel);
            _signalBus.Fire(new GameFinishedSignal(panel));
        }
    }
}
