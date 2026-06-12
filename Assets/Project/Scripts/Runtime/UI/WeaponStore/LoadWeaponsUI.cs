using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadWeaponsUI : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("rect")]
        private RectTransform _rect;

        [SerializeField, FormerlySerializedAs("elements")]
        private List<LoadWeaponUIInfo> _elements;

        [SerializeField, FormerlySerializedAs("offset"), Range(200, 500)]
        private float _offset = 200;

        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
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
            _rect.sizeDelta = new Vector2(0, _rect.sizeDelta.y);
            foreach (LoadWeaponUIInfo element in _elements)
            {
                var shortInfo = _progressService.GetWeaponShortInfo((int)element.Type);
                var weaponStore = _assetProvider.WeaponStoreDatabase;
                var weapon = weaponStore.GetWeapon((int)element.Type);

                if (shortInfo.AmmoCount > 0)
                {
                    element.Initialized(shortInfo.AmmoCount);
                    element.gameObject.SetActive(true);
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x + _offset, _rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
