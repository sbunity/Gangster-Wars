using SBabchuk.Runtime.Vibration;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IVibrationService
    {
        void Vibrate(VibrationConfig vibration);
    }
}
