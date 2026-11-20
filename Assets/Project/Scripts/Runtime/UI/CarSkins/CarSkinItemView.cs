using System;
using SBabchuk.Runtime.Databases.CarSkins;
using SBabchuk.Runtime.Services.Models;
using SBabchuk.Runtime.UI.WeaponStore;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.CarSkins
{
    public sealed class CarSkinItemView : MonoBehaviour
    {
        [SerializeField] private SpriteSwap _background;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _title;
        [SerializeField] private GameObject _lockMark;
        [SerializeField] private Button _buyButton;
        [SerializeField] private StorePriceView _price;
        [SerializeField] private Button _selectButton;
        [SerializeField] private GameObject _selectedMark;

        private Action<int> _buyRequested;
        private Action<int> _selectRequested;
        private UIClickSound _buySound;

        public int SkinId { get; private set; }

        private void Awake()
        {
            _buyButton.TryGetComponent(out _buySound);
            _buyButton.onClick.AddListener(OnBuyClicked);
            _selectButton.onClick.AddListener(OnSelectClicked);
        }

        private void OnDestroy()
        {
            _buyButton.onClick.RemoveListener(OnBuyClicked);
            _selectButton.onClick.RemoveListener(OnSelectClicked);
        }

        public void Bind(CarSkin skin, Action<int> buyRequested, Action<int> selectRequested)
        {
            SkinId = skin.Id;
            _buyRequested = buyRequested;
            _selectRequested = selectRequested;

            _icon.sprite = skin.Sprite;
            _title.text = skin.Name;
        }

        public void Render(CarSkinItemState state, StoreItemPrice price)
        {
            var isLocked = state == CarSkinItemState.Locked;

            _background.Change(!isLocked);
            _lockMark.SetActive(isLocked);
            _buyButton.gameObject.SetActive(isLocked);
            _buyButton.interactable = price.CanBuy;
            _price.Show(price);

            if (_buySound)
                _buySound.DeferSound = price.IsRealMoney;
            _selectButton.gameObject.SetActive(state == CarSkinItemState.Owned);
            _selectedMark.SetActive(state == CarSkinItemState.Selected);
        }

        public void PlayPurchaseSound()
        {
            if (_buySound)
                _buySound.PlayDeferredSound();
        }

        private void OnBuyClicked()
            => _buyRequested?.Invoke(SkinId);

        private void OnSelectClicked()
            => _selectRequested?.Invoke(SkinId);
    }
}
