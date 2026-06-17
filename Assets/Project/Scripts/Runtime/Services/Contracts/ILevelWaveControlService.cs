namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelWaveControlService
    {
        bool CanStartNextWave { get; }

        /// <summary>
        /// Starts the next wave. Returns the coin reward granted for skipping
        /// (0 when the wave could not be started or no reward applied).
        /// </summary>
        int StartNextWave();
    }
}
