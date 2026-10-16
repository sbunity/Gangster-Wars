namespace SBabchuk.Runtime.Vibration
{
    public interface IHapticDevice
    {
        bool IsSupported { get; }

        void Play(VibrationConfig vibration);
        void Cancel();
    }
}
