using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class LoadWeaponsUI : MonoBehaviour
    {
        [Header("Елемент LayoutGroup")]
        public RectTransform rect;

        [Header("Елементи UI")]
        public List<LoadWeaponUIInfo> elements;

        [Header("Відступ")]
        [Range(200, 500)]
        public float offset = 200;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus?.Subscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnDisable()
        {
            _signalBus?.Unsubscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            Initialized();
        }

        private void Start()
        {
            Initialized();
        }

        private void Initialized()
        {
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);

            foreach (LoadWeaponUIInfo element in elements)
            {
                WeaponShortInfo shortInfo = _progressService.GetWeaponShortInfo((int)element.type);

                var weaponStore = _assetProvider.WeaponStoreDatabase;
                Weapon weapon = weaponStore.GetWeapon((int)element.type);

                if (shortInfo.countPatrons > 0)
                {
                    element.Initialized(shortInfo.countPatrons);

                    element.gameObject.SetActive(true);

                    rect.sizeDelta = new Vector2(rect.sizeDelta.x + offset, rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
