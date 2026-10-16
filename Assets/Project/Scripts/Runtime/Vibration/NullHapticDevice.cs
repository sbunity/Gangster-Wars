namespace SBabchuk.Runtime.Vibration
{
    public sealed class NullHapticDevice : IHapticDevice
    {
        public bool IsSupported => false;

        public void Play(VibrationConfig vibration)
        {
        }

        public void Cancel()
        {
        }
    }
}
