using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Characters;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class WeaponAmmoService : IWeaponAmmoService, ITickable
    {
        private const float InitialReloadDelay = 1f;
        private const float MinReloadInterval = 0.01f;

        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressService _progressService;
        private readonly SignalBus _signalBus;

        private readonly Dictionary<WeaponsName, WeaponMagazine> _magazines = new();

        private WeaponMagazine _active;
        private WeaponMagazine _reloadingMagazine;
        private bool _reloading;
        private float _reloadTimer;
        private float _reloadInterval = InitialReloadDelay;

        public WeaponAmmoService(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        public WeaponsName ActiveWeapon => _active?.Weapon ?? WeaponsName.None;
        public int Current => _active?.Current ?? 0;
        public int Capacity => _active?.Capacity ?? 0;
        public bool IsEmpty => _active == null || _active.IsEmpty;
        public bool IsFull => _active != null && _active.IsFull;

        public event Action ReloadAdvanced;
        public event Action ReloadCompleted;
        public event Action MagazineEmptied;

        public void SelectWeapon(int weaponId)
        {
            var weapon = (WeaponsName)weaponId;
            var definition = _assetProvider.WeaponStoreDatabase.GetWeapon(weaponId);
            if (definition == null)
                return;

            StopReload();

            if (!_magazines.TryGetValue(weapon, out var magazine))
            {
                var seededRounds = Mathf.Min(ReserveOf(weapon), definition.Magazine);
                magazine = new WeaponMagazine(weapon, definition.Magazine, seededRounds);
                _magazines[weapon] = magazine;
            }

            _active = magazine;
            _reloadInterval = Mathf.Max(MinReloadInterval, definition.SpeedReload);

            _signalBus.Fire(new LeaderMagazineInitializedSignal(magazine.Capacity));
            _signalBus.Fire(new LeaderPatronsChangedSignal(magazine.Current));

            BeginReload();
        }

        public bool TryConsumeRound()
        {
            if (_active == null || !_active.TryConsume())
                return false;

            if (_active.Weapon != WeaponsName.Weapon_1)
                _progressService.SetWeaponAmmo(_active.Weapon, -1);

            _signalBus.Fire(new LeaderPatronsChangedSignal(_active.Current));

            if (_active.IsEmpty)
                MagazineEmptied?.Invoke();

            return true;
        }

        public void BeginReload()
        {
            if (_active == null || _active.IsFull || ReserveOf(_active.Weapon) <= _active.Current)
            {
                StopReload();
                return;
            }

            _reloading = true;
            _reloadingMagazine = _active;
            _reloadTimer = InitialReloadDelay + _reloadInterval;
        }

        public void StopReload()
        {
            if (!_reloading)
                return;

            _reloading = false;
            _reloadingMagazine = null;
            ReloadCompleted?.Invoke();
        }

        public void Tick()
        {
            if (!_reloading)
                return;

            if (_reloadingMagazine == null || _reloadingMagazine != _active)
            {
                StopReload();
                return;
            }

            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer > 0f)
                return;

            if (_active.TryLoadRound(ReserveOf(_active.Weapon)))
            {
                _signalBus.Fire(new LeaderPatronsChangedSignal(_active.Current));
                ReloadAdvanced?.Invoke();
                _reloadTimer = _reloadInterval;
            }
            else
            {
                StopReload();
            }
        }

        private int ReserveOf(WeaponsName weapon)
        {
            var info = _progressService.GetWeaponShortInfo((int)weapon);
            return info?.AmmoCount ?? 0;
        }
    }
}
