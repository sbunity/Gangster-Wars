using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class LockElementController : LockElementControllerBase
    {
        private Weapon weaponInfo;

        public override void Initialisation(int _id)
        {
            id = _id;

            ///Отримуєм зброю з бази
            weaponInfo = PersistableSO.Instance.WeaponStore.GetWeapon(id);

            if (priceBuy)
            {
                priceBuy.text = weaponInfo.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(weaponInfo.price);
            }
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public override void Buy()
        {
            PersistableSO.Instance.PlayerPrefs.BuyWeapon(id);
        }
    }
}
