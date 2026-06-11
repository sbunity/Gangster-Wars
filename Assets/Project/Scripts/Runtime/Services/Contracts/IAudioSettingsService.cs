namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAudioSettingsService
    {
        bool IsMusicEnabled { get; }

        bool IsSoundEnabled { get; }

        void SetMusicEnabled(bool value);
        void SetSoundEnabled(bool value);
        void Apply();
    }
}