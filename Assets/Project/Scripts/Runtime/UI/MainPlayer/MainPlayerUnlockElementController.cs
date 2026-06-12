using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.MainPlayers;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public class MainPlayerUnlockElementController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("priceUpgrade")]
        private Text _priceUpgrade;

        [SerializeField, FormerlySerializedAs("bttnUpgrade")]
        private Button _bttnUpgrade;

        [SerializeField, FormerlySerializedAs("BuyUpgradeElements")]
        private GameObject _buyUpgradeElements;

        private int _price;
        private int _personageID;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public void Initialisation(int _id)
        {
            InitialisationUpgrade(_id);
        }

        private void InitialisationUpgrade(int _id)
        {
            _personageID = _id;
            var personageShortInfo = _progressService.GetPersonageShortInfo(_id);
            int _upgradeID = personageShortInfo.UpgradeId + 1;
            var playerStore = _assetProvider.MainPlayerDatabase;
            PUpgrade _upgrade = playerStore.GetUpgrade(_id, _upgradeID);
            
            if (_upgrade != null)
            {
                _price = _upgrade.Price;
                if (_priceUpgrade)
                {
                    _priceUpgrade.text = _price.ToString();
                }

                if (_bttnUpgrade)
                {
                    _bttnUpgrade.interactable = _progressService.CanBuy(_price);
                }
            }
            else
            {
                _buyUpgradeElements.SetActive(false);
            }
        }

        public void BuyUpgrade()
        {
            _progressService.BuyPersonageUpgrade(_personageID);
        }
    }
}
