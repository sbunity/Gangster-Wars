namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelWaveControlService
    {
        bool CanStartNextWave { get; }
        void StartNextWave();
    }
}
