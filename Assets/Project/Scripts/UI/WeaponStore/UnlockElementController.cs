using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class UnlockElementController : MonoBehaviour
    {
        [Header("Поле для виведення вартості покупки апгрейда зброї")]
        public Text priceUpgrade;

        [Header("Кнопка покупки апгрейда зброї")]
        public Button bttnUpgrade;

        [Header("Поле для виведення вартості покупки патрон")]
        public Text priceMagazine;

        [Header("Кнопка покупки патрон")]
        public Button bttnMagazine;

        [Header("Кнопка і ціна для апгрейдів")]
        public GameObject BuyUpgradeElements;

        [Header("Кнопка і ціна для патронів")]
        public GameObject BuyMagazineElements;

        int price;

        int weaponID;

        private void CheckActive(bool _value = false)
        {
            gameObject.SetActive(_value);
        }

        public void Initialisation(int _id)
        {
            InitialisationUpgrade(_id);

            InitialisationMagazine(_id);
        }

        private void InitialisationUpgrade(int _id)
        {
            weaponID = _id;

            int _upgradeID = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo(_id).upgradeID + 1;

            WUpgrade _upgrade = PersistableSO.Instance.WeaponStore.GetUpgrade(_id, _upgradeID);

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

        private void InitialisationMagazine(int _id)
        {
            WeaponShortInfo weaponShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo(_id);

            Weapon weapon = PersistableSO.Instance.WeaponStore.GetWeapon(_id);

            //if (weaponShortInfo.currentMagazine < weapon.magazine)
            //{

                price = PersistableSO.Instance.WeaponStore.GetWeapon(_id).priceMagazine;

                if (priceMagazine)
                {
                    priceMagazine.text = price.ToString();
                }

                if (bttnMagazine)
                {
                    bttnMagazine.interactable = PersistableSO.Instance.PlayerPrefs.OpportunityBuy(price);
                }
            //}
            //else
            //{
            //    BuyMagazineElements.SetActive(false);
            //}
        }

        /// <summary>
        /// Поукупка апгрейда зброї
        /// </summary>
        public void BuyUpgrade()
        {
            PersistableSO.Instance.PlayerPrefs.BuyUpgrade(weaponID);
        }

        /// <summary>
        /// Поукупка апгрейда магазина
        /// </summary>
        public void BuyMagazine()
        {
            PersistableSO.Instance.PlayerPrefs.BuyMagazine(weaponID);
        }
    }
}
