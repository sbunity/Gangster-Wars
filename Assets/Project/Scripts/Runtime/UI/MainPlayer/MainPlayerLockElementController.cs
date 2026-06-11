using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class MainPlayerLockElementController : LockElementControllerBase
    {
        private Personage personage;
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
            Debug.Log("Initialisation" + _id);
            id = _id;

            ///Отримуєм зброю з бази
            var playerStore = _assetProvider.MainPlayerDatabase;
            personage = playerStore.GetPersonage(id);

            if (priceBuy)
            {
                priceBuy.text = personage.price.ToString();
            }

            if (bttnBuy)
            {
                bttnBuy.interactable = _progressService.CanBuy(personage.price);
            }
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public override void Buy()
        {
            _progressService.BuyPersonage(id);
        }
    }
}
