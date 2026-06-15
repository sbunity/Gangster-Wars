namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILeaderWeaponSelectionService
    {
        WeaponsName CurrentWeapon { get; }

        void SelectWeapon(WeaponsName weapon);
    }
}
