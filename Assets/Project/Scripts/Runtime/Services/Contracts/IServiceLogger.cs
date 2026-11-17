namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IServiceLogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
