using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class LockDElementController : LockElementControllerBase
    {
        private Defense defenceInfo;
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
        }

        /// <summary>
        /// Поукупка перепони
        /// </summary>
        public override void Buy()
        {
            _progressService.BuyDefence(id);
        }
    }
}
