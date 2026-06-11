using System;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class LevelService : ILevelService, ILevelFlowService
    {
        private readonly SignalBus _signalBus;
        public event Action<Panels> Finished;
        public Level CurrentLevel { get; private set; }
        public int CurrentWave { get; private set; }
        public bool IsFinished { get; private set; }
        public Panels LastPanel { get; private set; } = Panels.None;

        public LevelService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize(Level level)
        {
            CurrentLevel = level;
            CurrentWave = 0;
            IsFinished = false;
            LastPanel = Panels.None;
        }

        public void Start(Level level)
        {
            Initialize(level);
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