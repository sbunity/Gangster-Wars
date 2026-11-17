using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class LogSystemMessageService : ISystemMessageService
    {
        public void ShowMessage(string message, float duration = 2.5f)
            => Debug.Log($"[System Message]: {message}");

        public void ShowLoadingPanel()
            => Debug.Log("[System Message]: Loading panel shown.");

        public void ChangeLoadingMessage(string message)
            => Debug.Log($"[System Message]: Loading - {message}");

        public void HideLoadingPanel()
            => Debug.Log("[System Message]: Loading panel hidden.");
    }
}
