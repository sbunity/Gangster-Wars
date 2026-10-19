using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class AudioSettingsService : IAudioSettingsService
    {
        private readonly IPlayerProgressRepository _repository;
        private readonly SignalBus _signalBus;

        public AudioSettingsService(IPlayerProgressRepository repository, SignalBus signalBus)
        {
            _repository = repository;
            _signalBus = signalBus;
        }

        public bool IsMusicEnabled => _repository.Progress.Music == mySwitch.On;
        public bool IsSoundEnabled => _repository.Progress.Sound == mySwitch.On;

        public void SetMusicEnabled(bool value)
        {
            _repository.Progress.Music = value ? mySwitch.On : mySwitch.Off;
            _repository.Save();
            Apply();
        }

        public void SetSoundEnabled(bool value)
        {
            _repository.Progress.Sound = value ? mySwitch.On : mySwitch.Off;
            _repository.Save();
            Apply();
        }

        public void Apply()
        {
            _signalBus.Fire<AudioSettingsChangedSignal>();
        }
    }
}
