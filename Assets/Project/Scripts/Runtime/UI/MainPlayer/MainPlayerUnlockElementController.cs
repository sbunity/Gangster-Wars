using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        }

        private void InitialisationUpgrade(int _id)
        {
            personageID = _id;

            var personageShortInfo = _progressService.GetPersonageShortInfo(_id);

            int _upgradeID = personageShortInfo.upgradeID + 1;

            var playerStore = _assetProvider.MainPlayerDatabase;
            PUpgrade _upgrade = playerStore.GetUpgrade(_id, _upgradeID);

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

        /// <summary>
        /// Поукупка апгрейда зброї
        /// </summary>
        public void BuyUpgrade()
        {
            _progressService.BuyPersonageUpgrade(personageID);
        }
    }
}
