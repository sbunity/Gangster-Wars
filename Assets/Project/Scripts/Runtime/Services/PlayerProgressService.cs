using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressService : IPlayerProgressService, IInitializable
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ISaveService _saveService;
        private readonly SignalBus _signalBus;

        public PlayerProgressService(IAssetProvider assetProvider, ISaveService saveService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _saveService = saveService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            if (!EnsureProgressData())
                return;

            SaveProgress();
            FireProgressChanged();
        }

        public PlayerPrefs PlayerPrefs => Preferences.PlayerPrefs;
        public PlayerPrefsDatabase Preferences => _assetProvider.PlayerPrefsDatabase;
        public int Coins => PlayerPrefs.Coin;
        public int CurrentLevelId => PlayerPrefs.LevelId;
        public int SelectedWeaponId => PlayerPrefs.SelectedWeaponId;
        public int SelectedGrenadeId => PlayerPrefs.SelectedGrenadeId;
        public int SelectedDefenceId => PlayerPrefs.SelectedDefenceId;

        public void AddCoins(int value)
        {
            PlayerPrefs.Coin += value;
            if (PlayerPrefs.Coin < 0)
                PlayerPrefs.Coin = 0;

            SaveProgress();
            _signalBus.Fire(new CoinsChangedSignal(PlayerPrefs.Coin));
        }

        public bool CanBuy(int price) 
            => Preferences.OpportunityBuy(price);

        public void SetCurrentLevel(int id)
        {
            PlayerPrefs.LevelId = id;
            SaveProgress();
        }

        public void CompleteCurrentLevel(float barricadeHealthPercent)
        {
            var levelShortInfo = PlayerPrefs.GetLevelShortInfo(PlayerPrefs.LevelId);
            if (levelShortInfo == null)
                return;

            levelShortInfo.IsCompleted = mySwitch.On;
            Preferences.SetStars(levelShortInfo, barricadeHealthPercent);
            SaveProgress();
        }

        public WeaponShortInfo GetWeaponShortInfo(int id) 
            => PlayerPrefs.GetWeaponShortInfo(id);

        public GrenadeShortInfo GetGrenadeShortInfo(int id) 
            => PlayerPrefs.GetGrenadeShortInfo(id);

        public DefenceShortInfo GetDefenceShortInfo(int id) 
            => PlayerPrefs.GetDefenceShortInfo(id);

        public PersonageShortInfo GetPersonageShortInfo(int id) 
            => PlayerPrefs.GetPersonageShortInfo(id);

        public LevelShortInfo GetLevelShortInfo(int id) 
            => PlayerPrefs.GetLevelShortInfo(id);

        public void SetWeaponAmmo(WeaponsName weapon, int value)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo((int)weapon);
            if (weaponShortInfo == null)
                return;

            var previousAmmoCount = weaponShortInfo.AmmoCount;
            weaponShortInfo.AmmoCount += value;
            if (weaponShortInfo.AmmoCount < 0)
                weaponShortInfo.AmmoCount = 0;

            SaveProgress();
            FireWeaponAmmoChanged(weapon, weaponShortInfo.AmmoCount);

            if (HasWeaponAvailabilityChanged(previousAmmoCount, weaponShortInfo.AmmoCount))
                FireProgressChanged();
        }

        public void BuyWeapon(int id)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            weaponShortInfo.IsBuy = mySwitch.On;
            AddCoins(-weapon.Price);
            FireProgressChanged();
        }

        public void BuyWeaponMagazine(int id, bool isFree = false)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            weaponShortInfo.AmmoCount += weapon.Magazine;

            if (!isFree)
                AddCoins(-weapon.PriceMagazine);
            else
                SaveProgress();

            FireWeaponAmmoChanged((WeaponsName)id, weaponShortInfo.AmmoCount);
            FireProgressChanged();
        }

        public void BuyWeaponUpgrade(int id)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            if (weaponShortInfo.UpgradeId >= weapon.Upgrades.Count - 1)
                return;

            weaponShortInfo.UpgradeId++;
            var upgrade = _assetProvider.WeaponStoreDatabase.GetUpgrade(id, weaponShortInfo.UpgradeId);
            
            if (upgrade != null)
                AddCoins(-upgrade.Price);
            else
                SaveProgress();

            FireProgressChanged();
        }

        public void BuyGrenade(int id, bool isFree = false)
        {
            var grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(id);
            var grenade = _assetProvider.BombStoreDatabase.GetGrenade(id);
            if (grenadeShortInfo == null || grenade == null)
                return;

            NormalizeGrenadeCount(grenadeShortInfo);

            grenadeShortInfo.IsBuy = mySwitch.On;
            grenadeShortInfo.Count++;

            if (!isFree)
                AddCoins(-grenade.Price);
            else
                SaveProgress();

            FireProgressChanged();
        }

        public bool CanUseGrenade(int id)
        {
            var grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(id);
            if (grenadeShortInfo == null)
                return false;

            if (NormalizeGrenadeCount(grenadeShortInfo))
            {
                SaveProgress();
                FireProgressChanged();
            }

            return grenadeShortInfo.Count > 0;
        }

        public bool UseGrenade(int id)
        {
            var grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(id);
            if (grenadeShortInfo == null)
                return false;

            var wasNormalized = NormalizeGrenadeCount(grenadeShortInfo);
            if (grenadeShortInfo.Count <= 0)
            {
                if (wasNormalized)
                {
                    SaveProgress();
                    FireProgressChanged();
                }

                return false;
            }

            grenadeShortInfo.Count--;
            SaveProgress();
            FireProgressChanged();
            return true;
        }

        public void BuyDefence(int id)
        {
            var defenceShortInfo = PlayerPrefs.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence == null)
                return;

            defenceShortInfo.IsBuy = mySwitch.On;
            AddCoins(-defence.Price);
            FireProgressChanged();
        }

        public void SelectDefence(int id)
        {
            PlayerPrefs.SelectedDefenceId = id;
            SaveProgress();
            FireProgressChanged();
        }

        public void BuyDefenceUpgrade(int id)
        {
            var defenceShortInfo = PlayerPrefs.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence == null)
                return;

            if (defenceShortInfo.UpgradeId >= defence.Upgrades.Count - 1)
                return;

            defenceShortInfo.UpgradeId++;
            var upgrade = _assetProvider.DefenseStoreDatabase.GetUpgrade(id, defenceShortInfo.UpgradeId);
            
            if (upgrade != null)
                AddCoins(-upgrade.Price);
            else
                SaveProgress();

            FireProgressChanged();
        }

        public void BuyPersonage(int id)
        {
            var personageShortInfo = PlayerPrefs.GetPersonageShortInfo(id);
            var personage = _assetProvider.MainPlayerDatabase.GetPersonage(id);
            if (personageShortInfo == null || personage == null)
                return;

            personageShortInfo.IsBuy = mySwitch.On;
            AddCoins(-personage.Price);
            FireProgressChanged();
        }

        public void BuyPersonageUpgrade(int id)
        {
            var personageShortInfo = PlayerPrefs.GetPersonageShortInfo(id);
            var personage = _assetProvider.MainPlayerDatabase.GetPersonage(id);
            if (personageShortInfo == null || personage == null)
                return;

            if (personageShortInfo.UpgradeId >= personage.Upgrades.Count - 1)
                return;

            personageShortInfo.UpgradeId++;
            var upgrade = _assetProvider.MainPlayerDatabase.GetUpgrade(id, personageShortInfo.UpgradeId);
            
            if (upgrade != null)
                AddCoins(-upgrade.Price);
            else
                SaveProgress();

            FireProgressChanged();
        }

        private void SaveProgress() 
            => _saveService.SaveAsync(Preferences).Forget();

        private bool EnsureProgressData()
        {
            var changed = false;
            changed |= EnsureProgressLists();
            changed |= EnsureWeaponProgress();
            changed |= EnsureGrenadeProgress();
            changed |= EnsureDefenceProgress();
            changed |= EnsurePersonageProgress();
            changed |= EnsureLevelProgress();
            changed |= NormalizeGrenadeInventory();
            return changed;
        }

        private bool EnsureProgressLists()
        {
            var changed = false;

            if (PlayerPrefs.Weapons == null)
            {
                PlayerPrefs.Weapons = new List<WeaponShortInfo>();
                changed = true;
            }

            if (PlayerPrefs.Grenades == null)
            {
                PlayerPrefs.Grenades = new List<GrenadeShortInfo>();
                changed = true;
            }

            if (PlayerPrefs.Defences == null)
            {
                PlayerPrefs.Defences = new List<DefenceShortInfo>();
                changed = true;
            }

            if (PlayerPrefs.Personages == null)
            {
                PlayerPrefs.Personages = new List<PersonageShortInfo>();
                changed = true;
            }

            if (PlayerPrefs.Levels == null)
            {
                PlayerPrefs.Levels = new List<LevelShortInfo>();
                changed = true;
            }

            changed |= RemoveNullEntries(PlayerPrefs.Weapons);
            changed |= RemoveNullEntries(PlayerPrefs.Grenades);
            changed |= RemoveNullEntries(PlayerPrefs.Defences);
            changed |= RemoveNullEntries(PlayerPrefs.Personages);
            changed |= RemoveNullEntries(PlayerPrefs.Levels);

            return changed;
        }

        private bool EnsureWeaponProgress()
        {
            var weapons = _assetProvider.WeaponStoreDatabase.Weapons;
            if (weapons == null)
                return false;

            var changed = false;
            foreach (var weapon in weapons)
            {
                if (weapon != null && PlayerPrefs.GetWeaponShortInfo(weapon.Id) == null)
                {
                    PlayerPrefs.Weapons.Add(new WeaponShortInfo(weapon));
                    changed = true;
                }
            }

            return changed;
        }

        private bool EnsureGrenadeProgress()
        {
            var grenades = _assetProvider.BombStoreDatabase.Grenades;
            if (grenades == null)
                return false;

            var changed = false;
            foreach (var grenade in grenades)
            {
                if (grenade != null && PlayerPrefs.GetGrenadeShortInfo(grenade.Id) == null)
                {
                    PlayerPrefs.Grenades.Add(new GrenadeShortInfo(grenade));
                    changed = true;
                }
            }

            return changed;
        }

        private bool EnsureDefenceProgress()
        {
            var defences = _assetProvider.DefenseStoreDatabase.Defenses;
            if (defences == null)
                return false;

            var changed = false;
            foreach (var defence in defences)
            {
                if (defence != null && PlayerPrefs.GetDefenceShortInfo(defence.Id) == null)
                {
                    PlayerPrefs.Defences.Add(new DefenceShortInfo(defence));
                    changed = true;
                }
            }

            return changed;
        }

        private bool EnsurePersonageProgress()
        {
            var personages = _assetProvider.MainPlayerDatabase.Personages;
            if (personages == null)
                return false;

            var changed = false;
            foreach (var personage in personages)
            {
                if (personage != null && PlayerPrefs.GetPersonageShortInfo(personage.Id) == null)
                {
                    PlayerPrefs.Personages.Add(new PersonageShortInfo(personage));
                    changed = true;
                }
            }

            return changed;
        }

        private bool EnsureLevelProgress()
        {
            var levels = _assetProvider.LevelDatabase.Levels;
            if (levels == null)
                return false;

            var changed = false;
            foreach (var level in levels)
            {
                if (level != null && PlayerPrefs.GetLevelShortInfo(level.Id) == null)
                {
                    PlayerPrefs.Levels.Add(new LevelShortInfo(level));
                    changed = true;
                }
            }

            return changed;
        }

        private bool NormalizeGrenadeInventory()
        {
            var grenades = PlayerPrefs?.Grenades;
            if (grenades == null)
                return false;

            var hasChanges = false;
            foreach (var grenade in grenades)
                hasChanges |= NormalizeGrenadeCount(grenade);

            return hasChanges;
        }

        private bool RemoveNullEntries<T>(List<T> entries) where T : class
        {
            if (entries == null)
                return false;

            var removedCount = entries.RemoveAll(entry => entry == null);
            return removedCount > 0;
        }

        private bool NormalizeGrenadeCount(GrenadeShortInfo grenadeShortInfo)
        {
            if (grenadeShortInfo == null || grenadeShortInfo.Count >= 0)
                return false;

            grenadeShortInfo.Count = 0;
            return true;
        }

        private void FireWeaponAmmoChanged(WeaponsName weapon, int count)
            => _signalBus.Fire(new WeaponAmmoChangedSignal(weapon, count));

        private static bool HasWeaponAvailabilityChanged(int previousAmmoCount, int currentAmmoCount)
            => (previousAmmoCount <= 0) != (currentAmmoCount <= 0);

        private void FireProgressChanged() 
            => _signalBus.Fire<ProgressUpgradedSignal>();
    }
}
