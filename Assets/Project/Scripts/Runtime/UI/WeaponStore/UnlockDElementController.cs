using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.DefenseStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class UnlockDElementController : LockElementControllerBase
    {
        [SerializeField, FormerlySerializedAs("changed")]
        private Image _changed;

        [SerializeField, FormerlySerializedAs("select")]
        private Button _select;

        [SerializeField, FormerlySerializedAs("levelUp")]
        private GameObject _levelUp;

        private Defense _defenceInfo;

        public override void Initialisation(int id)
        {
            Id = id;
            var defenceStore = _assetProvider.DefenseStoreDatabase;
            _defenceInfo = defenceStore.GetDefense(Id);

            if (PriceBuy)
                PriceBuy.text = _defenceInfo.Price.ToString();

            if (BttnBuy)
                BttnBuy.interactable = _progressService.CanBuy(_defenceInfo.Price);

            var defenceShortInfo = _progressService.GetDefenceShortInfo(id);
            var selectedDefenceId = _progressService.SelectedDefenceId;

            if (_levelUp)
                _levelUp.SetActive(defenceShortInfo.UpgradeId < _defenceInfo.CountUpgrades - 1 || selectedDefenceId == -1);

            if (_changed)
                _changed.gameObject.SetActive(selectedDefenceId == id);

            if (_select)
                _select.gameObject.SetActive(selectedDefenceId != id);
        }

        public override void Buy()
        {
            _progressService.BuyDefenceUpgrade(Id);
        }

        public void Select()
        {
            _progressService.SelectDefence(Id);
        }
    }
}
