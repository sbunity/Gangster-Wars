using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class LockDElementController : LockElementControllerBase
    {
        private Defense defenceInfo;

        public override void Initialisation(int _id)
        {
            id = _id;

            ///Отримуєм зброю з бази
            defenceInfo = PersistableSO.Instance.DefenceStore.GetDefense(id);

            if (priceBuy)
            {
                priceBuy.text = defenceInfo.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(defenceInfo.price);
            }
        }

        /// <summary>
        /// Поукупка перепони
        /// </summary>
        public override void Buy()
        {
            PersistableSO.Instance.PlayerPrefs.BuyDefence(id);
        }
    }
}
