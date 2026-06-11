using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

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
        public int Coins => PlayerPrefs.coin;
        public int CurrentLevelId => PlayerPrefs.levelID;
        public int SelectedWeaponId => PlayerPrefs.selectedWeaponID;
        public int SelectedGrenadeId => PlayerPrefs.selectedGrenadeID;
        public int SelectedDefenceId => PlayerPrefs.selectedDefenceID;

        public void AddCoins(int value)
        {
            PlayerPrefs.coin += value;
            if (PlayerPrefs.coin < 0)
                PlayerPrefs.coin = 0;

            SaveProgress();
            _signalBus.Fire(new CoinsChangedSignal(PlayerPrefs.coin));
        }

        public bool CanBuy(int price)
        {
            return Preferences.OpportunityBuy(price);
        }

        public void SetCurrentLevel(int id)
        {
            PlayerPrefs.levelID = id;
            SaveProgress();
        }

        public void CompleteCurrentLevel(float barricadeHealthPercent)
        {
            var levelShortInfo = PlayerPrefs.GetLevelShortInfo(PlayerPrefs.levelID);
            if (levelShortInfo == null)
                return;

            levelShortInfo.isCompleted = mySwitch.On;
            Preferences.SetStars(levelShortInfo, barricadeHealthPercent);
            SaveProgress();
        }

        public WeaponShortInfo GetWeaponShortInfo(int id)
        {
            return PlayerPrefs.GetWeaponShortInfo(id);
        }

        public GrenadeShortInfo GetGrenadeShortInfo(int id)
        {
            return PlayerPrefs.GetGrenadeShortInfo(id);
        }

        public DefenceShortInfo GetDefenceShortInfo(int id)
        {
            return PlayerPrefs.GetDefenceShortInfo(id);
        }

        public PersonageShortInfo GetPersonageShortInfo(int id)
        {
            return PlayerPrefs.GetPersonageShortInfo(id);
        }

        public LevelShortInfo GetLevelShortInfo(int id)
        {
            return PlayerPrefs.GetLevelShortInfo(id);
        }

        public void SetWeaponAmmo(WeaponsName weapon, int value)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo((int)weapon);
            if (weaponShortInfo == null)
                return;

            weaponShortInfo.countPatrons += value;
            SaveProgress();

            _signalBus.Fire(new WeaponAmmoChangedSignal(weapon, weaponShortInfo.countPatrons));
            if (weaponShortInfo.countPatrons == 0)
                FireProgressChanged();
        }

        public void BuyWeapon(int id)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            weaponShortInfo.isBuy = mySwitch.On;
            AddCoins(-weapon.price);
            FireProgressChanged();
        }

        public void BuyWeaponMagazine(int id, bool isFree = false)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            weaponShortInfo.countPatrons += weapon.magazine;
            if (!isFree)
                AddCoins(-weapon.priceMagazine);
            else
                SaveProgress();

            FireProgressChanged();
        }

        public void BuyWeaponUpgrade(int id)
        {
            var weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            if (weaponShortInfo.upgradeID >= weapon.upgrades.Count - 1)
                return;

            weaponShortInfo.upgradeID++;
            var upgrade = _assetProvider.WeaponStoreDatabase.GetUpgrade(id, weaponShortInfo.upgradeID);
            if (upgrade != null)
                AddCoins(-upgrade.price);
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

            if (grenadeShortInfo.isBuy == mySwitch.On)
            {
                grenadeShortInfo.count++;
                if (!isFree)
                    AddCoins(-grenade.price);
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

            grenadeShortInfo.count--;
            SaveProgress();
            FireProgressChanged();
        }

        public void BuyDefence(int id)
        {
            var defenceShortInfo = PlayerPrefs.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence == null)
                return;

            defenceShortInfo.isBuy = mySwitch.On;
            AddCoins(-defence.price);
            FireProgressChanged();
        }

        public void SelectDefence(int id)
        {
            PlayerPrefs.selectedDefenceID = id;
            SaveProgress();
            FireProgressChanged();
        }

        public void BuyDefenceUpgrade(int id)
        {
            var defenceShortInfo = PlayerPrefs.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence == null)
                return;

            if (defenceShortInfo.upgradeID >= defence.upgrades.Count - 1)
                return;

            defenceShortInfo.upgradeID++;
            var upgrade = _assetProvider.DefenseStoreDatabase.GetUpgrade(id, defenceShortInfo.upgradeID);
            if (upgrade != null)
                AddCoins(-upgrade.price);
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

            personageShortInfo.isBuy = mySwitch.On;
            AddCoins(-personage.price);
            FireProgressChanged();
        }

        public void BuyPersonageUpgrade(int id)
        {
            var personageShortInfo = PlayerPrefs.GetPersonageShortInfo(id);
            var personage = _assetProvider.MainPlayerDatabase.GetPersonage(id);
            if (personageShortInfo == null || personage == null)
                return;

            if (personageShortInfo.upgradeID >= personage.upgrades.Count - 1)
                return;

            personageShortInfo.upgradeID++;
            var upgrade = _assetProvider.MainPlayerDatabase.GetUpgrade(id, personageShortInfo.upgradeID);
            if (upgrade != null)
                AddCoins(-upgrade.price);
            else
                SaveProgress();

            FireProgressChanged();
        }

        private void SaveProgress()
        {
            _saveService.SaveAsync(Preferences).Forget();
        }

        private void FireProgressChanged()
        {
            _signalBus.Fire<ProgressUpgradedSignal>();
        }
    }
}
