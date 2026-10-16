using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBarricadeTarget
    {
        Vector2 GetHitPoint(Vector2 lanePoint);
        Vector2 GetContactPoint(Vector2 lanePoint);
    }
}
