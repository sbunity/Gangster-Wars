using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class VibrationSettingsService : IVibrationSettingsService
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ISaveService _saveService;

        public VibrationSettingsService(IPlayerProgressService progressService, ISaveService saveService)
        {
            _progressService = progressService;
            _saveService = saveService;
        }

        public bool IsVibrationEnabled => _progressService.Preferences.IsVibrationEnabled();

        public void SetVibrationEnabled(bool value)
        {
            _progressService.PlayerPrefs.Vibration = value ? mySwitch.On : mySwitch.Off;
            _saveService.SaveAsync(_progressService.Preferences).Forget();
        }
    }
}
