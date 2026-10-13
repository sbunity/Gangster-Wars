using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILeaderWeaponController
    {
        bool IsAttacking { get; }
        float AimOffsetY { get; }

        void Attack();
        void StopAttack();
        void CancelAttack();
        Vector3 GetAimOrigin();
    }
}
