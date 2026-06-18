using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.UI.WeaponStore;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public class MainPlayerUnlockElementController : MonoBehaviour, IStoreElementView
    {
        [SerializeField, FormerlySerializedAs("priceUpgrade")]
        private Text _priceUpgrade;

        [SerializeField, FormerlySerializedAs("bttnUpgrade")]
        private Button _bttnUpgrade;

        [SerializeField, FormerlySerializedAs("BuyUpgradeElements")]
        private GameObject _buyUpgradeElements;

        private int _price;
        private int _personageID;
        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        public void Initialisation(int id)
        {
            InitialisationUpgrade(id);
        }

        private void InitialisationUpgrade(int id)
        {
            _personageID = id;
            
            if (_progressService.TryGetNextPersonageUpgradePrice(id, out var price))
            {
                _price = price;
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
