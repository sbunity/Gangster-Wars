using System;
using SBabchuk.Runtime.Databases.CarSkins;
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
        [SerializeField] private Text _priceText;
        [SerializeField] private Button _selectButton;
        [SerializeField] private GameObject _selectedMark;

        private Action<int> _buyRequested;
        private Action<int> _selectRequested;

        public int SkinId { get; private set; }

        private void Awake()
        {
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
            _priceText.text = skin.Price.ToString();
        }

        public void Render(CarSkinItemState state, bool canBuy)
        {
            var isLocked = state == CarSkinItemState.Locked;

            _background.Change(!isLocked);
            _lockMark.SetActive(isLocked);
            _buyButton.gameObject.SetActive(isLocked);
            _buyButton.interactable = canBuy;
            _selectButton.gameObject.SetActive(state == CarSkinItemState.Owned);
            _selectedMark.SetActive(state == CarSkinItemState.Selected);
        }

        private void OnBuyClicked()
            => _buyRequested?.Invoke(SkinId);

        private void OnSelectClicked()
            => _selectRequested?.Invoke(SkinId);
    }
}
