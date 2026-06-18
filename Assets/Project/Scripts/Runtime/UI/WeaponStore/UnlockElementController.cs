using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class UnlockElementController : MonoBehaviour, IStoreElementView
    {
        [SerializeField, FormerlySerializedAs("priceUpgrade")]
        private Text _priceUpgrade;

        [SerializeField, FormerlySerializedAs("bttnUpgrade")]
        private Button _bttnUpgrade;

        [SerializeField, FormerlySerializedAs("priceMagazine")]
        private Text _priceMagazine;

        [SerializeField, FormerlySerializedAs("bttnMagazine")]
        private Button _bttnMagazine;

        [SerializeField, FormerlySerializedAs("BuyUpgradeElements")]
        private GameObject _buyUpgradeElements;

        [SerializeField, FormerlySerializedAs("BuyMagazineElements")]
        private GameObject _buyMagazineElements;

        private int _price;
        private int _weaponID;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        public void Initialisation(int id)
        {
            InitialisationUpgrade(id);
            InitialisationMagazine(id);
            InitialisationBuyMagazine(id);
        }

        private void InitialisationUpgrade(int id)
        {
            _weaponID = id;

            if (_progressService.TryGetNextWeaponUpgradePrice(id, out var price))
            {
                _price = price;
                if (_priceUpgrade)
                    _priceUpgrade.text = _price.ToString();

                if (_bttnUpgrade)
                    _bttnUpgrade.interactable = _progressService.CanBuy(_price);
            }
            else
            {
                _buyUpgradeElements.SetActive(false);
            }
        }

        private void InitialisationMagazine(int id)
        {
            var weaponStore = _assetProvider.WeaponStoreDatabase;
            var weapon = weaponStore.GetWeapon(id);
            if (weapon == null)
                return;

            _price = weapon.PriceMagazine;

            if (_priceMagazine)
                _priceMagazine.text = _price.ToString();

            if (_bttnMagazine)
                _bttnMagazine.interactable = _progressService.CanBuy(_price);
        }

        private void InitialisationBuyMagazine(int id)
        {
            if (id == 0)
                _buyMagazineElements.SetActive(false);
        }

        public void BuyUpgrade()
        {
            _progressService.BuyWeaponUpgrade(_weaponID);
        }

        public void BuyMagazine()
        {
            _progressService.BuyWeaponMagazine(_weaponID);
        }
    }
}
