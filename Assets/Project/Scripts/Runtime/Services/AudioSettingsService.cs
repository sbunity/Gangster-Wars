using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class AudioSettingsService : IAudioSettingsService
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ISaveService _saveService;
        private readonly SignalBus _signalBus;
        
        public AudioSettingsService(IPlayerProgressService progressService, ISaveService saveService, SignalBus signalBus)
        {
            _progressService = progressService;
            _saveService = saveService;
            _signalBus = signalBus;
        }

        public bool IsMusicEnabled => _progressService.Preferences.IsMusicEnabled();
        public bool IsSoundEnabled => _progressService.Preferences.IsSoundEnabled();

        public void SetMusicEnabled(bool value)
        {
            _progressService.PlayerPrefs.Music = value ? mySwitch.On : mySwitch.Off;
            _saveService.SaveAsync(_progressService.Preferences).Forget();
            Apply();
        }

        public void SetSoundEnabled(bool value)
        {
            _progressService.PlayerPrefs.Sound = value ? mySwitch.On : mySwitch.Off;
            _saveService.SaveAsync(_progressService.Preferences).Forget();
            Apply();
        }

        public void Apply()
        {
            _signalBus.Fire<AudioSettingsChangedSignal>();
        }
    }
}