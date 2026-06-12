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
    public sealed class PlayerProgressService : IPlayerProgressService
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

            if (grenadeShortInfo.IsBuy == mySwitch.On)
            {
                grenadeShortInfo.Count++;
                if (!isFree)
                    AddCoins(-grenade.Price);
                else
                    SaveProgress();
            }

            FireProgressChanged();
        }

        public void UseGrenade(int id)
        {
            var grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(id);
            if (grenadeShortInfo == null)
                return;

            grenadeShortInfo.Count--;
            SaveProgress();
            FireProgressChanged();
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

        private void FireWeaponAmmoChanged(WeaponsName weapon, int count)
            => _signalBus.Fire(new WeaponAmmoChangedSignal(weapon, count));

        private static bool HasWeaponAvailabilityChanged(int previousAmmoCount, int currentAmmoCount)
            => (previousAmmoCount <= 0) != (currentAmmoCount <= 0);

        private void FireProgressChanged() 
            => _signalBus.Fire<ProgressUpgradedSignal>();
    }
}
