using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class UnlockGElementController : LockElementControllerBase
    {
        private Grenade grenadeInfo;

        public override void Initialisation(int _id)
        {
            id = _id;

            ///Отримуєм зброю з бази
            grenadeInfo = PersistableSO.Instance.BombStore.GetGrenade(id);

            if (priceBuy)
            {
                priceBuy.text = grenadeInfo.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(grenadeInfo.price);
            }
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public override void Buy()
        {
            PersistableSO.Instance.PlayerPrefs.BuyGrenade(id);
        }

    }
}
