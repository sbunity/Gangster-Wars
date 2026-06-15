using System;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class DefaultWeaponFallbackService : IInitializable, IDisposable
    {
        private const WeaponsName DefaultWeapon = WeaponsName.Weapon_1;

        private readonly ILeaderWeaponSelectionService _weaponSelection;
        private readonly SignalSubscriptions _signals;

        public DefaultWeaponFallbackService(
            ILeaderWeaponSelectionService weaponSelection,
            SignalBus signalBus)
        {
            _weaponSelection = weaponSelection;
            _signals = new SignalSubscriptions(signalBus)
                .Add<WeaponAmmoChangedSignal>(OnWeaponAmmoChanged);
        }

        public void Initialize()
        {
            _signals.Enable();
        }

        public void Dispose()
        {
            _signals.Disable();
        }

        private void OnWeaponAmmoChanged(WeaponAmmoChangedSignal signal)
        {
            if (signal.Weapon == DefaultWeapon || signal.Count > 0)
                return;

            if (_weaponSelection.CurrentWeapon != signal.Weapon)
                return;

            _weaponSelection.SelectWeapon(DefaultWeapon);
        }
    }
}
