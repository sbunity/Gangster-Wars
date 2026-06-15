namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ICameraShakeService
    {
        void Shake(float strength);
        void Shake(float strength, float duration);
    }
}
