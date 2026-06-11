using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILeaderWeaponController
    {
        bool IsAttacking { get; }
        void Attack();
        void StopAttack();
        Vector3 GetAimOrigin();
    }
}
