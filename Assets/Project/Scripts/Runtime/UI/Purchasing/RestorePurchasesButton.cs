using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.Purchasing
{
    public sealed class RestorePurchasesButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private IIAPShopService _iapShopService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(IIAPShopService iapShopService, SignalBus signalBus)
        {
            _iapShopService = iapShopService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<IAPInitializedSignal>(Refresh);

            gameObject.SetActive(iapShopService.IsRestoreSupported);
        }

        private void Awake()
        {
            _button.onClick.AddListener(Restore);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Restore);
        }

        private void OnEnable()
        {
            _signals?.Enable();
            Refresh();
        }

        private void OnDisable()
        {
            _signals?.Disable();
        }

        private void Refresh()
        {
            if (_iapShopService != null)
                _button.interactable = _iapShopService.IsReady;
        }

        private void Restore()
            => _iapShopService.RestorePurchases();
    }
}
