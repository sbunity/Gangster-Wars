using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;

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

        public static void ApplyPrice(Text priceLabel, Button buyButton, int? price, IPlayerProgressService progressService)
        {
            if (priceLabel)
                priceLabel.text = price?.ToString() ?? string.Empty;

            if (buyButton)
                buyButton.interactable = price.HasValue && progressService.CanBuy(price.Value);
        }
    }
}
