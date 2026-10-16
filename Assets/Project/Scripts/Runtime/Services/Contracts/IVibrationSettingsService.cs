namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IVibrationSettingsService
    {
        bool IsVibrationEnabled { get; }

        void SetVibrationEnabled(bool value);
    }
}
