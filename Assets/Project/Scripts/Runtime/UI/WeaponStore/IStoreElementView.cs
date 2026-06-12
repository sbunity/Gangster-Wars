using UnityEngine;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public interface IStoreElementView
    {
        void Initialisation(int id);
    }

    public static class StoreElementView
    {
        public static void Apply(
            SpriteSwap panel,
            GameObject lockObj,
            GameObject unlockObj,
            int id,
            bool isUnlocked,
            IStoreElementView lockInit = null,
            IStoreElementView unlockInit = null,
            AmmunitionsController ammo = null)
        {
            if (panel)
                panel.Change(isUnlocked);

            lockObj.SetActive(!isUnlocked);
            unlockObj.SetActive(isUnlocked);

            if (isUnlocked)
            {
                unlockInit?.Initialisation(id);
                ammo?.Initialisation(id);
            }
            else
            {
                lockInit?.Initialisation(id);
            }
        }
    }
}
