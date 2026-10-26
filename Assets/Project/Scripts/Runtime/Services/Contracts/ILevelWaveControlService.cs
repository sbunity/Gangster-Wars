using System;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelWaveControlService
    {
        bool CanStartNextWave { get; }

        float SecondsUntilNextWave { get; }

        IDisposable HoldCountdown();

        int StartNextWave();
    }
}
