using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ICoinFlightService
    {
        void Play(Vector3 worldOrigin, int amount);
        void PlayFromScreen(Vector2 screenOrigin, int amount);
    }
}
