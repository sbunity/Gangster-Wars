namespace SBabchuk.Runtime.Services.Models
{
    public readonly struct BonusReward
    {
        public BonusReward(WeaponsName weapon, GrenadesName grenade)
        {
            Weapon = weapon;
            Grenade = grenade;
        }

        public WeaponsName Weapon { get; }
        public GrenadesName Grenade { get; }
    }
}
