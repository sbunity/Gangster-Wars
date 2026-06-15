using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class NullCameraShakeService : ICameraShakeService
    {
        public void Shake(float strength)
        {
        }

        public void Shake(float strength, float duration)
        {
        }
    }
}
