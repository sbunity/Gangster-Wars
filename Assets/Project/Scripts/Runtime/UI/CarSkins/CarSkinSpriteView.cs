using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.CarSkins
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CarSkinSpriteView : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private ICarSkinService _carSkinService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(ICarSkinService carSkinService, SignalBus signalBus)
        {
            _carSkinService = carSkinService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<CarSkinChangedSignal>(Apply);
        }

        private void Awake()
            => _renderer = GetComponent<SpriteRenderer>();

        private void OnEnable()
            => _signals?.Enable();

        private void OnDisable()
            => _signals?.Disable();

        private void Start()
            => Apply();

        private void Apply()
        {
            var sprite = _carSkinService?.SelectedSkin?.Sprite;
            if (sprite != null)
                _renderer.sprite = sprite;
        }
    }
}
