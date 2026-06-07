using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class MainPlayerLockElementController : LockElementControllerBase
    {
        private Personage personage;

        public override void Initialisation(int _id)
        {
            Debug.Log("Initialisation" + _id);
            id = _id;

            ///Отримуєм зброю з бази
            personage = PersistableSO.Instance.PlayerStore.GetPersonage(id);

            if (priceBuy)
            {
                priceBuy.text = personage.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(personage.price);
            }
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public override void Buy()
        {
            PersistableSO.Instance.PlayerPrefs.BuyPersonage(id);
        }
    }
}
