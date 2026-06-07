using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class UnlockDElementController : LockElementControllerBase
    {
        private Defense defenceInfo;

        [Header("Чи використовується")]
        public Image changed;

        [Header("Чи використовується")]
        public Button select;

        [Header("Апгрейд")]
        public GameObject levelUp;

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

            if (levelUp)
            {
                levelUp.gameObject.SetActive(PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetDefenceShortInfo(_id).upgradeID < defenceInfo.countUpgrades - 1 || PersistableSO.Instance.PlayerPrefs.PlayerPrefs.selectedDefenceID == -1);
            }

            if (changed)
            {
                changed.gameObject.SetActive(PersistableSO.Instance.PlayerPrefs.PlayerPrefs.selectedDefenceID == _id);
            }

            if (select)
            {
                select.gameObject.SetActive(PersistableSO.Instance.PlayerPrefs.PlayerPrefs.selectedDefenceID != _id);
            }
        }

        /// <summary>
        /// Поукупка апгрейда перепони
        /// </summary>
        public override void Buy()
        {
            PersistableSO.Instance.PlayerPrefs.BuyUpgradeDefence(id);
        }

        /// <summary>
        /// Вибираєм перепону
        /// </summary>
        public void Select()
        {
            PersistableSO.Instance.PlayerPrefs.SelectDefence(id);
        }
    }
}
