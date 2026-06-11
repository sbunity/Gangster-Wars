using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

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

            var weaponShortInfo = _progressService.GetWeaponShortInfo(_id);

            int _upgradeID = weaponShortInfo.upgradeID + 1;

            var weaponStore = _assetProvider.WeaponStoreDatabase;
            WUpgrade _upgrade = weaponStore.GetUpgrade(_id, _upgradeID);

            if (_upgrade != null)
            {
                price = _upgrade.price;

                if (priceUpgrade)
                {
                    priceUpgrade.text = price.ToString();
                }

                if (bttnUpgrade)
                {
                    bttnUpgrade.interactable = _progressService.CanBuy(price);
                }
            }
            else
            {
                BuyUpgradeElements.SetActive(false);
            }
        }

        private void InitialisationMagazine(int _id)
        {
            WeaponShortInfo weaponShortInfo = _progressService.GetWeaponShortInfo(_id);

            var weaponStore = _assetProvider.WeaponStoreDatabase;
            Weapon weapon = weaponStore.GetWeapon(_id);

            //if (weaponShortInfo.currentMagazine < weapon.magazine)
            //{

                price = weaponStore.GetWeapon(_id).priceMagazine;

                if (priceMagazine)
                {
                    priceMagazine.text = price.ToString();
                }

                if (bttnMagazine)
                {
                    bttnMagazine.interactable = _progressService.CanBuy(price);
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
            _progressService.BuyWeaponUpgrade(weaponID);
        }

        /// <summary>
        /// Поукупка апгрейда магазина
        /// </summary>
        public void BuyMagazine()
        {
            _progressService.BuyWeaponMagazine(weaponID);
        }
    }
}
