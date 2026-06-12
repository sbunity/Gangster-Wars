using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.PlayerPrefs;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadGrenadesUI : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("rect")]
        private RectTransform _rect;

        [SerializeField, FormerlySerializedAs("elements")]
        private List<LoadGrenadeInfoUI> _elements;

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
            foreach (LoadGrenadeInfoUI element in _elements)
            {
                GrenadeShortInfo shortInfo = _progressService.GetGrenadeShortInfo((int)element.Type);
                var bombStore = _assetProvider.BombStoreDatabase;
                Grenade grenade = bombStore.GetGrenade((int)element.Type);

                if (shortInfo.Count > 0)
                {
                    element.Initialized(grenade.Icon, shortInfo.Count);
                    element.gameObject.SetActive(true);
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x + 198, _rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
