using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class BonusRewardService : IBonusRewardService
    {
        private const float FREE_AMMO_MAGAZINE_MULTIPLIER = 0.6f;

        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressService _progressService;

        public BonusRewardService(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public void Grant(BonusReward reward)
        {
            if (reward.Weapon != WeaponsName.None)
                GrantAmmo(reward.Weapon);
            else if (reward.Grenade != GrenadesName.None)
                _progressService.BuyGrenade((int)reward.Grenade, true);
        }

        private void GrantAmmo(WeaponsName weaponName)
        {
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon((int)weaponName);
            if (weapon == null)
                return;

            var ammoCount = Mathf.Max(1, Mathf.CeilToInt(weapon.Magazine * FREE_AMMO_MAGAZINE_MULTIPLIER));
            _progressService.SetWeaponAmmo(weaponName, ammoCount);
        }
    }
}
