using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public abstract class IAPProductElementView : MonoBehaviour
    {
        [SerializeField] private ProductKeyType _product;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Text _priceText;

        private IIAPShopService _iapShopService;
        private SignalSubscriptions _signals;

        protected ProductKeyType Product => _product;
        protected Button BuyButton => _buyButton;

        [Inject]
        public void Construct(IIAPShopService iapShopService, SignalBus signalBus)
        {
            _iapShopService = iapShopService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<IAPInitializedSignal>(Refresh);

            SubscribeSignals(_signals);
        }

        private void Awake()
        {
            _buyButton.onClick.AddListener(Buy);
        }

        private void OnDestroy()
        {
            _buyButton.onClick.RemoveListener(Buy);
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

        protected virtual void SubscribeSignals(SignalSubscriptions signals)
        {
        }

        protected abstract void RefreshContent();

        protected void Refresh()
        {
            if (_iapShopService == null)
                return;

            _priceText.text = _iapShopService.GetPrice(_product);
            _buyButton.interactable = _iapShopService.CanPurchase(_product);
            RefreshContent();
        }

        private void Buy()
            => _iapShopService.PurchaseAsync(_product, destroyCancellationToken).Forget();
    }
}
