using SBabchuk.Runtime.Audio;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAudioService
    {
        void Play(SoundConfig sound);
    }
}
