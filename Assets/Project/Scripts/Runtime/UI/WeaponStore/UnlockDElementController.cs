using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

        public override void Initialisation(int _id)
        {
            id = _id;

            ///Отримуєм зброю з бази
            var defenceStore = _assetProvider.DefenseStoreDatabase;
            defenceInfo = defenceStore.GetDefense(id);

            if (priceBuy)
            {
                priceBuy.text = defenceInfo.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = _progressService.CanBuy(defenceInfo.price);
            }

            var defenceShortInfo = _progressService.GetDefenceShortInfo(_id);

            var selectedDefenceId = _progressService.SelectedDefenceId;

            if (levelUp)
            {
                levelUp.gameObject.SetActive(defenceShortInfo.upgradeID < defenceInfo.countUpgrades - 1 || selectedDefenceId == -1);
            }

            if (changed)
            {
                changed.gameObject.SetActive(selectedDefenceId == _id);
            }

            if (select)
            {
                select.gameObject.SetActive(selectedDefenceId != _id);
            }
        }

        /// <summary>
        /// Поукупка апгрейда перепони
        /// </summary>
        public override void Buy()
        {
            _progressService.BuyDefenceUpgrade(id);
        }

        /// <summary>
        /// Вибираєм перепону
        /// </summary>
        public void Select()
        {
            _progressService.SelectDefence(id);
        }
    }
}
