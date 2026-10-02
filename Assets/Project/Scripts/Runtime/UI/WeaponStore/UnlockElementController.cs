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

            var hasUpgrade = _progressService.TryGetNextWeaponUpgradePrice(id, out var price);
            StoreElementView.ApplyPrice(_priceUpgrade, _bttnUpgrade, hasUpgrade ? price : (int?)null, _progressService);

            if (_buyUpgradeElements)
                _buyUpgradeElements.SetActive(hasUpgrade);
        }

        private void InitialisationMagazine(int id)
        {
            var weapon = _assetProvider.WeaponStoreDatabase.GetWeapon(id);
            StoreElementView.ApplyPrice(_priceMagazine, _bttnMagazine, weapon?.PriceMagazine, _progressService);
        }

        private void InitialisationBuyMagazine(int id)
        {
            if (_buyMagazineElements)
                _buyMagazineElements.SetActive(id != 0);
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
