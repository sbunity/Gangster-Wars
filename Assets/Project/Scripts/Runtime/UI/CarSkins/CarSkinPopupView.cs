using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.CarSkins
{
    public sealed class CarSkinPopupView : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _outsideCloseButton;
        [SerializeField] private Transform _itemsRoot;
        [SerializeField] private CarSkinItemView _itemTemplate;

        private readonly List<CarSkinItemView> _items = new();
        private ICarSkinService _carSkinService;
        private IInstantiator _instantiator;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(ICarSkinService carSkinService, IInstantiator instantiator, SignalBus signalBus)
        {
            _carSkinService = carSkinService;
            _instantiator = instantiator;
            _signals = new SignalSubscriptions(signalBus)
                .Add<CarSkinChangedSignal>(Refresh)
                .Add<CoinsChangedSignal>(Refresh);
        }

        private void Awake()
        {
            _itemTemplate.gameObject.SetActive(false);
            _closeButton.onClick.AddListener(Close);
            _outsideCloseButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Close);
            _outsideCloseButton.onClick.RemoveListener(Close);
        }

        private void OnEnable()
        {
            _signals?.Enable();
            EnsureItems();
            Refresh();
        }

        private void OnDisable()
        {
            _signals?.Disable();
        }

        public void Open()
            => gameObject.SetActive(true);

        public void Close()
            => gameObject.SetActive(false);

        private void EnsureItems()
        {
            if (_items.Count > 0 || _carSkinService == null)
                return;

            foreach (var skin in _carSkinService.Skins)
            {
                if (skin == null)
                    continue;

                var item = _instantiator.InstantiatePrefabForComponent<CarSkinItemView>(_itemTemplate, _itemsRoot);
                item.gameObject.SetActive(true);
                item.Bind(skin, OnBuyRequested, OnSelectRequested);
                _items.Add(item);
            }
        }

        private void Refresh()
        {
            if (_carSkinService == null)
                return;

            foreach (var item in _items)
                item.Render(GetState(item.SkinId), _carSkinService.CanBuy(item.SkinId));
        }

        private CarSkinItemState GetState(int skinId)
        {
            if (skinId == _carSkinService.SelectedSkinId)
                return CarSkinItemState.Selected;

            return _carSkinService.IsOwned(skinId) ? CarSkinItemState.Owned : CarSkinItemState.Locked;
        }

        private void OnBuyRequested(int skinId)
            => _carSkinService.TryBuy(skinId);

        private void OnSelectRequested(int skinId)
            => _carSkinService.TrySelect(skinId);
    }
}
