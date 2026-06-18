using System.Collections.Generic;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.WeaponStore;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI.WeaponStore.Info;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class StoreItemStatsService : IStoreItemStatsService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressService _progressService;

        public StoreItemStatsService(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public StoreItemStatsSnapshot GetStats(StoreInfoItemType itemType, int itemId) 
            => itemType switch
            {
                StoreInfoItemType.Weapon => CreateWeaponStats(itemId),
                StoreInfoItemType.Grenade => CreateGrenadeStats(itemId),
                StoreInfoItemType.Defense => CreateDefenseStats(itemId),
                _ => EmptySnapshot(),
            };

        private StoreItemStatsSnapshot CreateWeaponStats(int itemId)
        {
            var database = _assetProvider.WeaponStoreDatabase;
            var weapon = database.GetWeapon(itemId);
            if (weapon == null)
                return EmptySnapshot();

            var progress = _progressService.GetWeaponShortInfo(itemId);
            var isOwned = progress != null && progress.IsBuy == mySwitch.On;
            var currentUpgrade = progress != null ? database.GetUpgrade(itemId, progress.UpgradeId) : null;
            var nextUpgrade = isOwned ? database.GetUpgrade(itemId, progress.UpgradeId + 1) : null;
            var currentDamage = currentUpgrade?.Settings?.Damage ?? weapon.Settings.Damage;
            var upgradedDamage = nextUpgrade?.Settings?.Damage ?? currentDamage;
            var maxDamage = GetMaxWeaponDamage(database);
            var maxMagazine = GetMaxWeaponMagazine(database);
            var maxReload = GetMaxWeaponReload(database);

            var stats = new List<StoreStatSnapshot>
            {
                new("Damage", currentDamage, upgradedDamage, maxDamage),
                new("Magazine", weapon.Magazine, weapon.Magazine, maxMagazine),
                new("Reload Time", weapon.SpeedReload, weapon.SpeedReload, maxReload)
            };

            return new StoreItemStatsSnapshot(weapon.Name, weapon.Icon, stats);
        }

        private StoreItemStatsSnapshot CreateGrenadeStats(int itemId)
        {
            var database = _assetProvider.BombStoreDatabase;
            var grenade = database.GetGrenade(itemId);
            if (grenade == null)
                return EmptySnapshot();

            var stats = new List<StoreStatSnapshot>
            {
                new("Damage", grenade.Damage, grenade.Damage, GetMaxGrenadeDamage(database)),
                new("Radius", grenade.Radius, grenade.Radius, GetMaxGrenadeRadius(database))
            };

            if ((GrenadesName)itemId == GrenadesName.Grenade_2)
                stats.Add(new StoreStatSnapshot("Delay", grenade.Delay, grenade.Delay, GetMaxGrenadeDelay(database)));

            if ((GrenadesName)itemId == GrenadesName.Grenade_3)
                stats.Add(new StoreStatSnapshot("Duration", grenade.Time, grenade.Time, GetMaxGrenadeDuration(database)));

            return new StoreItemStatsSnapshot(grenade.Name, grenade.Icon, stats);
        }

        private StoreItemStatsSnapshot CreateDefenseStats(int itemId)
        {
            var database = _assetProvider.DefenseStoreDatabase;
            var defense = database.GetDefense(itemId);
            if (defense == null)
                return EmptySnapshot();

            var progress = _progressService.GetDefenceShortInfo(itemId);
            var isOwned = progress != null && progress.IsBuy == mySwitch.On;
            var currentUpgrade = progress != null ? database.GetUpgrade(itemId, progress.UpgradeId) : null;
            var nextUpgrade = isOwned ? database.GetUpgrade(itemId, progress.UpgradeId + 1) : null;
            var currentHealth = currentUpgrade?.Settings?.Health ?? defense.Settings.Health;
            var upgradedHealth = nextUpgrade?.Settings?.Health ?? currentHealth;

            var stats = new List<StoreStatSnapshot>
            {
                new("Health", currentHealth, upgradedHealth, GetMaxDefenseHealth(database))
            };

            return new StoreItemStatsSnapshot(defense.Name, defense.Icon, stats);
        }

        private StoreItemStatsSnapshot EmptySnapshot()
            => new("Unknown", null, new List<StoreStatSnapshot>());

        private float GetMaxWeaponDamage(WeaponStoreDatabase database)
        {
            var max = 1f;
            foreach (var weapon in database.Weapons)
            {
                if (weapon == null)
                    continue;

                max = Mathf.Max(max, weapon.Settings.Damage);
                foreach (var upgrade in weapon.Upgrades)
                {
                    if (upgrade?.Settings != null)
                        max = Mathf.Max(max, upgrade.Settings.Damage);
                }
            }

            return max;
        }

        private float GetMaxWeaponMagazine(WeaponStoreDatabase database)
        {
            var max = 1f;
            foreach (var weapon in database.Weapons)
            {
                if (weapon != null)
                    max = Mathf.Max(max, weapon.Magazine);
            }

            return max;
        }

        private float GetMaxWeaponReload(WeaponStoreDatabase database)
        {
            var max = 1f;
            foreach (var weapon in database.Weapons)
            {
                if (weapon != null)
                    max = Mathf.Max(max, weapon.SpeedReload);
            }

            return max;
        }

        private float GetMaxGrenadeDamage(BombStoreDatabase database)
        {
            var max = 1f;
            foreach (var grenade in database.Grenades)
            {
                if (grenade != null)
                    max = Mathf.Max(max, grenade.Damage);
            }

            return max;
        }

        private float GetMaxGrenadeRadius(BombStoreDatabase database)
        {
            var max = 1f;
            foreach (var grenade in database.Grenades)
            {
                if (grenade != null)
                    max = Mathf.Max(max, grenade.Radius);
            }

            return max;
        }

        private float GetMaxGrenadeDelay(BombStoreDatabase database)
        {
            var max = 1f;
            foreach (var grenade in database.Grenades)
            {
                if (grenade != null)
                    max = Mathf.Max(max, grenade.Delay);
            }

            return max;
        }

        private float GetMaxGrenadeDuration(BombStoreDatabase database)
        {
            var max = 1f;
            foreach (var grenade in database.Grenades)
            {
                if (grenade != null)
                    max = Mathf.Max(max, grenade.Time);
            }

            return max;
        }

        private float GetMaxDefenseHealth(DefenseStoreDatabase database)
        {
            var max = 1f;
            foreach (var defense in database.Defenses)
            {
                if (defense == null)
                    continue;

                max = Mathf.Max(max, defense.Settings.Health);
                foreach (var upgrade in defense.Upgrades)
                {
                    if (upgrade?.Settings != null)
                        max = Mathf.Max(max, upgrade.Settings.Health);
                }
            }

            return max;
        }
    }
}
