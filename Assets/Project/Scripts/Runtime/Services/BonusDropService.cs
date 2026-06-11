using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    // Decides which bonus (if any) is eligible to drop, based on owned weapons/grenades.
    // Extracted from LevelController; selection probabilities are unchanged.
    public sealed class BonusDropService : IBonusDropService
    {
        private const int AMMO_BONUS_DROP_CHANCE = 35;

        private readonly IPlayerProgressService _progressService;

        public BonusDropService(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        public int GetAvailableBonusId()
        {
            var ammoBonuses = new List<int>();
            var otherBonuses = new List<int>();

            for (int bonusID = 0; bonusID < 8; bonusID++)
            {
                if (!CanDropBonus(bonusID))
                    continue;

                if (IsAmmoBonus(bonusID))
                    ammoBonuses.Add(bonusID);
                else
                    otherBonuses.Add(bonusID);
            }

            if (ammoBonuses.Count == 0 && otherBonuses.Count == 0)
                return -1;

            if (ammoBonuses.Count > 0 && Random.Range(0, 100) < AMMO_BONUS_DROP_CHANCE)
                return ammoBonuses[Random.Range(0, ammoBonuses.Count)];

            if (otherBonuses.Count > 0)
                return otherBonuses[Random.Range(0, otherBonuses.Count)];

            return -1;
        }

        private bool IsAmmoBonus(int bonusID)
        {
            return bonusID >= 0 && bonusID <= 3;
        }

        private bool CanDropBonus(int bonusID)
        {
            if (_progressService == null)
                return false;

            if (bonusID >= 0 && bonusID <= 3)
            {
                var weaponInfo = _progressService.GetWeaponShortInfo(bonusID + 1);
                return weaponInfo != null && weaponInfo.IsBuy == mySwitch.On;
            }

            if (bonusID >= 4 && bonusID <= 7)
            {
                var grenadeInfo = _progressService.GetGrenadeShortInfo(bonusID - 4);
                return grenadeInfo != null && grenadeInfo.IsBuy == mySwitch.On;
            }

            return false;
        }
    }
}
