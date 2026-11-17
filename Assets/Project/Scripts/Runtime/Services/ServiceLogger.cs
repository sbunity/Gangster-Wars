using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class ServiceLogger : IServiceLogger
    {
        public void Log(string message)
        {
#if DEBUG_LOGS_SERVICES
            UnityEngine.Debug.Log(message);
#endif
        }

        public void LogWarning(string message)
        {
#if DEBUG_LOGS_SERVICES
            UnityEngine.Debug.LogWarning(message);
#endif
        }

        public void LogError(string message)
        {
#if DEBUG_LOGS_SERVICES
            UnityEngine.Debug.LogError(message);
#endif
        }
    }
}
