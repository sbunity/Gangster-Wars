using System;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IWeaponAmmoService
    {
        WeaponsName ActiveWeapon { get; }
        int Current { get; }
        int Capacity { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        void SelectWeapon(int weaponId);

        bool TryConsumeRound();

        void BeginReload();
        void StopReload();

        event Action ReloadAdvanced;

        event Action ReloadCompleted;

        event Action MagazineEmptied;
    }
}
