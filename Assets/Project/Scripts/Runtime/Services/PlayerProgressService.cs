using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressService : IPlayerProgressService
    {
        private const float ThreeStarHealthThreshold = 0.75f;
        private const float TwoStarHealthThreshold = 0.50f;

        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressRepository _repository;
        private readonly SignalBus _signalBus;

        public PlayerProgressService(IAssetProvider assetProvider, IPlayerProgressRepository repository, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _repository = repository;
            _signalBus = signalBus;
        }

        private PlayerProgress Progress => _repository.Progress;

        public int Coins => Progress.Coin;
        public int CurrentLevelId => Progress.LevelId;
        public int CurrentChapterId => Progress.ChapterId;
        public int SelectedWeaponId => Progress.SelectedWeaponId;
        public int SelectedGrenadeId => Progress.SelectedGrenadeId;
        public int SelectedDefenceId => Progress.SelectedDefenceId;

        public void AddCoins(int value)
        {
            var previousCoins = Progress.Coin;
            Progress.Coin += value;
            if (Progress.Coin < 0)
                Progress.Coin = 0;

            SaveProgress();
            _signalBus.Fire(new CoinsChangedSignal(Progress.Coin, Progress.Coin - previousCoins));
        }

        public bool CanBuy(int price)
            => Progress.Coin >= price;

        public bool TryGetNextWeaponUpgradePrice(int id, out int price)
        {
            price = 0;
            var weaponShortInfo = Progress.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon?.Upgrades == null || weaponShortInfo.UpgradeId >= weapon.Upgrades.Count - 1)
                return false;

            var upgrade = _assetProvider.WeaponStoreDatabase.GetUpgrade(id, weaponShortInfo.UpgradeId + 1);
            if (upgrade == null)
                return false;

            price = upgrade.Price;
            return true;
        }

        public bool TryGetNextDefenceUpgradePrice(int id, out int price)
        {
            price = 0;
            var defenceShortInfo = Progress.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence?.Upgrades == null || defenceShortInfo.UpgradeId >= defence.Upgrades.Count - 1)
                return false;

            var upgrade = _assetProvider.DefenseStoreDatabase.GetUpgrade(id, defenceShortInfo.UpgradeId + 1);
            if (upgrade == null)
                return false;

            price = upgrade.Price;
            return true;
        }

        public bool TryGetNextPersonageUpgradePrice(int id, out int price)
        {
            price = 0;
            var personageShortInfo = Progress.GetPersonageShortInfo(id);
            var personage = _assetProvider.MainPlayerDatabase.GetPersonage(id);
            if (personageShortInfo == null || personage?.Upgrades == null || personageShortInfo.UpgradeId >= personage.Upgrades.Count - 1)
                return false;

            var upgrade = _assetProvider.MainPlayerDatabase.GetUpgrade(id, personageShortInfo.UpgradeId + 1);
            if (upgrade == null)
                return false;

            price = upgrade.Price;
            return true;
        }

        public void SetCurrentLevel(int id)
        {
            Progress.LevelId = id;
            Progress.ChapterId = _assetProvider.LevelDatabase.GetChapterIdByLevelId(id);
            SaveProgress();
        }

        public void CompleteCurrentLevel(float barricadeHealthPercent)
        {
            var levelShortInfo = Progress.GetLevelShortInfo(Progress.LevelId);
            if (levelShortInfo == null)
                return;

            levelShortInfo.IsCompleted = mySwitch.On;
            levelShortInfo.Stars = Mathf.Max(levelShortInfo.Stars, CalculateStars(barricadeHealthPercent));
            RefreshChapterCompletion(levelShortInfo.ChapterId);
            SaveProgress();
        }

        public WeaponShortInfo GetWeaponShortInfo(int id) 
            => Progress.GetWeaponShortInfo(id);

        public GrenadeShortInfo GetGrenadeShortInfo(int id) 
            => Progress.GetGrenadeShortInfo(id);

        public DefenceShortInfo GetDefenceShortInfo(int id) 
            => Progress.GetDefenceShortInfo(id);

        public PersonageShortInfo GetPersonageShortInfo(int id) 
            => Progress.GetPersonageShortInfo(id);

        public LevelShortInfo GetLevelShortInfo(int id) 
            => Progress.GetLevelShortInfo(id);

        public ChapterShortInfo GetChapterShortInfo(int id)
            => Progress.GetChapterShortInfo(id);

        public void SetWeaponAmmo(WeaponsName weapon, int value)
        {
            var weaponShortInfo = Progress.GetWeaponShortInfo((int)weapon);
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
            var weaponShortInfo = Progress.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            if (!CanBuy(weapon.Price))
                return;

            weaponShortInfo.IsBuy = mySwitch.On;
            AddCoins(-weapon.Price);
            FireProgressChanged();
        }

        public void BuyWeaponMagazine(int id, bool isFree = false)
        {
            var weaponShortInfo = Progress.GetWeaponShortInfo(id);
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            if (weaponShortInfo == null || weapon == null)
                return;

            if (!isFree && !CanBuy(weapon.PriceMagazine))
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
            var weaponShortInfo = Progress.GetWeaponShortInfo(id);
            if (weaponShortInfo == null)
                return;

            if (!TryGetNextWeaponUpgradePrice(id, out var price) || !CanBuy(price))
                return;

            weaponShortInfo.UpgradeId++;
            AddCoins(-price);

            FireProgressChanged();
        }

        public void BuyGrenade(int id, bool isFree = false)
        {
            var grenadeShortInfo = Progress.GetGrenadeShortInfo(id);
            var grenade = _assetProvider.BombStoreDatabase.GetGrenade(id);
            if (grenadeShortInfo == null || grenade == null)
                return;

            NormalizeGrenadeCount(grenadeShortInfo);

            if (!isFree && !CanBuy(grenade.Price))
                return;

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
            var grenadeShortInfo = Progress.GetGrenadeShortInfo(id);
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
            var grenadeShortInfo = Progress.GetGrenadeShortInfo(id);
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
            var defenceShortInfo = Progress.GetDefenceShortInfo(id);
            var defence = _assetProvider.DefenseStoreDatabase.GetDefense(id);
            if (defenceShortInfo == null || defence == null)
                return;

            if (!CanBuy(defence.Price))
                return;

            defenceShortInfo.IsBuy = mySwitch.On;
            AddCoins(-defence.Price);
            FireProgressChanged();
        }

        public void SelectDefence(int id)
        {
            Progress.SelectedDefenceId = id;
            SaveProgress();
            FireProgressChanged();
        }

        public void BuyDefenceUpgrade(int id)
        {
            var defenceShortInfo = Progress.GetDefenceShortInfo(id);
            if (defenceShortInfo == null)
                return;

            if (!TryGetNextDefenceUpgradePrice(id, out var price) || !CanBuy(price))
                return;

            defenceShortInfo.UpgradeId++;
            AddCoins(-price);

            FireProgressChanged();
        }

        public void BuyPersonage(int id)
        {
            var personageShortInfo = Progress.GetPersonageShortInfo(id);
            var personage = _assetProvider.MainPlayerDatabase.GetPersonage(id);
            if (personageShortInfo == null || personage == null)
                return;

            if (!CanBuy(personage.Price))
                return;

            personageShortInfo.IsBuy = mySwitch.On;
            AddCoins(-personage.Price);
            FireProgressChanged();
        }

        public void BuyPersonageUpgrade(int id)
        {
            var personageShortInfo = Progress.GetPersonageShortInfo(id);
            if (personageShortInfo == null)
                return;

            if (!TryGetNextPersonageUpgradePrice(id, out var price) || !CanBuy(price))
                return;

            personageShortInfo.UpgradeId++;
            AddCoins(-price);

            FireProgressChanged();
        }

        private void SaveProgress()
            => _repository.Save();

        private void RefreshChapterCompletion(int chapterId)
        {
            var chapter = _assetProvider.LevelDatabase.GetChapter(chapterId);
            if (chapter != null)
                Progress.RefreshChapterCompletion(chapterId, chapter.GetLevelIds());
        }

        private static int CalculateStars(float barricadeHealthPercent)
        {
            if (barricadeHealthPercent >= ThreeStarHealthThreshold)
                return 3;

            return barricadeHealthPercent >= TwoStarHealthThreshold ? 2 : 1;
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
