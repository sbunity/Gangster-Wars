using SBabchuk.Runtime.Audio;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IMusicService
    {
        void Play(MusicTrack track);
        void Stop();
    }
}
