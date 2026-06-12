using SBabchuk.Runtime.Databases.PlayerPrefs;
namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IPlayerProgressService
    {
        PlayerPrefs PlayerPrefs { get; }

        PlayerPrefsDatabase Preferences { get; }

        int Coins { get; }

        int CurrentLevelId { get; }

        int SelectedWeaponId { get; }

        int SelectedGrenadeId { get; }

        int SelectedDefenceId { get; }

        void AddCoins(int value);
        bool CanBuy(int price);
        void SetCurrentLevel(int id);
        void CompleteCurrentLevel(float barricadeHealthPercent);
        WeaponShortInfo GetWeaponShortInfo(int id);
        GrenadeShortInfo GetGrenadeShortInfo(int id);
        DefenceShortInfo GetDefenceShortInfo(int id);
        PersonageShortInfo GetPersonageShortInfo(int id);
        LevelShortInfo GetLevelShortInfo(int id);
        void SetWeaponAmmo(WeaponsName weapon, int value);
        void BuyWeapon(int id);
        void BuyWeaponMagazine(int id, bool isFree = false);
        void BuyWeaponUpgrade(int id);
        void BuyGrenade(int id, bool isFree = false);
        void UseGrenade(int id);
        void BuyDefence(int id);
        void SelectDefence(int id);
        void BuyDefenceUpgrade(int id);
        void BuyPersonage(int id);
        void BuyPersonageUpgrade(int id);
    }
}
