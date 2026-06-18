using System.Collections.Generic;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI.WeaponStore.Info;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class MainPlayerStatsService : IMainPlayerStatsService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressService _progressService;

        public MainPlayerStatsService(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public StoreItemStatsSnapshot GetStats(int personageId, bool includeUpgradeDelta)
        {
            var database = _assetProvider.MainPlayerDatabase;
            var personage = database.GetPersonage(personageId);
            if (personage == null)
                return new StoreItemStatsSnapshot("Unknown", null, new List<StoreStatSnapshot>());

            var progress = _progressService.GetPersonageShortInfo(personageId);
            var isOwned = progress != null && progress.IsBuy == mySwitch.On;
            var currentUpgrade = progress != null ? database.GetUpgrade(personageId, progress.UpgradeId) : null;
            var nextUpgrade = includeUpgradeDelta && isOwned
                ? database.GetUpgrade(personageId, progress.UpgradeId + 1)
                : null;

            var currentSettings = currentUpgrade?.Settings ?? personage.Settings;
            var nextSettings = nextUpgrade?.Settings ?? currentSettings;

            var stats = new List<StoreStatSnapshot>
            {
                new(
                    "Fire Rate",
                    ToFireRate(currentSettings.AttackSpeed),
                    ToFireRate(nextSettings.AttackSpeed),
                    GetMaxFireRate(database)),
                new(
                    "Damage",
                    currentSettings.Damage,
                    nextSettings.Damage,
                    GetMaxDamage(database))
            };

            return new StoreItemStatsSnapshot(personage.Name, personage.Icon, stats);
        }

        private float ToFireRate(float attackInterval)
            => attackInterval <= 0f ? 0f : 60f / attackInterval;

        private float GetMaxFireRate(MainPlayerDatabase database)
        {
            var max = 1f;
            foreach (var personage in database.Personages)
            {
                if (personage == null)
                    continue;

                max = Mathf.Max(max, ToFireRate(personage.Settings.AttackSpeed));
                foreach (var upgrade in personage.Upgrades)
                {
                    if (upgrade?.Settings != null)
                        max = Mathf.Max(max, ToFireRate(upgrade.Settings.AttackSpeed));
                }
            }

            return max;
        }

        private float GetMaxDamage(MainPlayerDatabase database)
        {
            var max = 1f;
            foreach (var personage in database.Personages)
            {
                if (personage == null)
                    continue;

                max = Mathf.Max(max, personage.Settings.Damage);
                foreach (var upgrade in personage.Upgrades)
                {
                    if (upgrade?.Settings != null)
                        max = Mathf.Max(max, upgrade.Settings.Damage);
                }
            }

            return max;
        }
    }
}
