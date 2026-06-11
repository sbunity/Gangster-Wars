using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class LockElementController : LockElementControllerBase
    {
        private Weapon weaponInfo;
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
            var weaponStore = _assetProvider.WeaponStoreDatabase;
            weaponInfo = weaponStore.GetWeapon(id);

            if (priceBuy)
            {
                priceBuy.text = weaponInfo.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = _progressService.CanBuy(weaponInfo.price);
            }
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public override void Buy()
        {
            _progressService.BuyWeapon(id);
        }
    }
}
