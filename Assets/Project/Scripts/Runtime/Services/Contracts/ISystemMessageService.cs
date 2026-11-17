namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ISystemMessageService
    {
        void ShowMessage(string message, float duration = 2.5f);
        void ShowLoadingPanel();
        void ChangeLoadingMessage(string message);
        void HideLoadingPanel();
    }
}
