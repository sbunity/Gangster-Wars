using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class LoadGrenadesUI : MonoBehaviour
    {
        [Header("Елемент LayoutGroup")]
        public RectTransform rect;

        [Header("Елементи UI")]
        public List<LoadGrenadeInfoUI> elements;
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

            foreach (LoadGrenadeInfoUI element in elements)
            {
                GrenadeShortInfo shortInfo = _progressService.GetGrenadeShortInfo((int)element.type);

                var bombStore = _assetProvider.BombStoreDatabase;
                Grenade grenade = bombStore.GetGrenade((int)element.type);

                if (shortInfo.count > 0)
                {
                    element.Initialized(grenade.ico, shortInfo.count);

                    element.gameObject.SetActive(true);

                    rect.sizeDelta = new Vector2(rect.sizeDelta.x + 198, rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
