using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class MainPlayerUnlockElementController : MonoBehaviour
    {
        [Header("Поле для виведення вартості покупки апгрейда")]
        public Text priceUpgrade;

        [Header("Кнопка покупки апгрейда")]
        public Button bttnUpgrade;

        [Header("Кнопка і ціна для апгрейдів")]
        public GameObject BuyUpgradeElements;

        int price;

        int personageID;

        private void CheckActive(bool _value = false)
        {
            gameObject.SetActive(_value);
        }

        public void Initialisation(int _id)
        {
            InitialisationUpgrade(_id);
        }

        private void InitialisationUpgrade(int _id)
        {
            personageID = _id;

            int _upgradeID = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetPersonageShortInfo(_id).upgradeID + 1;

            PUpgrade _upgrade = PersistableSO.Instance.PlayerStore.GetUpgrade(_id, _upgradeID);

            if (_upgrade != null)
            {
                price = _upgrade.price;

                if (priceUpgrade)
                {
                    priceUpgrade.text = price.ToString();
                }

                if (bttnUpgrade)
                {
                    bttnUpgrade.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(price);
                }
            }
            else
            {
                BuyUpgradeElements.SetActive(false);
            }
        }

        /// <summary>
        /// Поукупка апгрейда зброї
        /// </summary>
        public void BuyUpgrade()
        {
            PersistableSO.Instance.PlayerPrefs.BuyUpgradePersonage(personageID);
        }
    }
}
