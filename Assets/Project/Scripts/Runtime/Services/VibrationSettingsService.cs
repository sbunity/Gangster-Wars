using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class VibrationSettingsService : IVibrationSettingsService
    {
        private readonly IPlayerProgressRepository _repository;

        public VibrationSettingsService(IPlayerProgressRepository repository)
        {
            _repository = repository;
        }

        public bool IsVibrationEnabled => _repository.Progress.Vibration == mySwitch.On;

        public void SetVibrationEnabled(bool value)
        {
            _repository.Progress.Vibration = value ? mySwitch.On : mySwitch.Off;
            _repository.Save();
        }
    }
}
