using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.Purchasing
{
    public sealed class IAPButton : MonoBehaviour
    {
        private const decimal PlaceholderPrice = 0.01m;

        [SerializeField] private Image _backImage;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private GameObject _loadingObject;

        [Space]
        [SerializeField] private Sprite _activeBackSprite;
        [SerializeField] private Sprite _unactiveBackSprite;

        private IIAPService _iapService;
        private ProductKeyType _key;

        [Inject]
        private void Construct(IIAPService iapService)
        {
            _iapService = iapService;
        }

        private void Awake()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void Init(ProductKeyType key)
        {
            _key = key;

            UpdateState();
        }

        public void UpdateState()
        {
            UpdateState(_iapService.GetProductData(_key));
        }

        public void UpdateState(ProductData product)
        {
            if (_loadingObject == null || _priceText == null || _backImage == null)
            {
                Debug.LogWarning($"[IAPButton] UI references are not assigned. Skipping UpdateState. Key: {_key}");

                return;
            }

            if (product != null)
            {
                _loadingObject.SetActive(false);
                _priceText.gameObject.SetActive(true);

                _backImage.sprite = _activeBackSprite;

                if (product.Price != PlaceholderPrice)
                {
                    _priceText.text = product.GetLocalPrice();
                }
                else
                {
                    var iapItem = _iapService.GetIAPItem(_key);
                    _priceText.text = iapItem != null ? $"USD {iapItem.DefaultUSDPrice}" : product.GetLocalPrice();
                }
            }
            else
            {
                SetDisabledState();
            }
        }

        private void SetDisabledState()
        {
            if (_loadingObject != null)
                _loadingObject.SetActive(true);

            if (_priceText != null)
                _priceText.gameObject.SetActive(false);

            if (_backImage != null && _unactiveBackSprite != null)
                _backImage.sprite = _unactiveBackSprite;
        }

        private void OnButtonClicked()
        {
            _iapService.BuyProduct(_key);
        }
    }
}
