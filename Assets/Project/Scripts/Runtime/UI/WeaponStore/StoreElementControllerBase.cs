using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public abstract class StoreElementControllerBase : MonoBehaviour
    {
        private SignalSubscriptions _signals;
        private bool _isViewCached;
        private bool _isStarted;
        private bool _isActive;
        private bool _isDefinitionBound;

        protected IAssetProvider AssetProvider { get; private set; }
        protected IPlayerProgressService ProgressService { get; private set; }

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            AssetProvider = assetProvider;
            ProgressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<ProgressUpgradedSignal>(OnProgressUpgraded);

            TryActivate();
        }

        protected virtual void Awake()
        {
            CacheView();
            _isViewCached = true;
            TryActivate();
        }

        protected virtual void Start()
        {
            _isStarted = true;
            TryActivate();
        }

        protected virtual void OnEnable()
        {
            _isActive = true;
            TryActivate();
        }

        protected virtual void OnDisable()
        {
            _isActive = false;
            _signals?.Disable();
        }

        protected abstract void CacheView();

        protected abstract void RefreshState();

        protected virtual void BindDefinition()
        {
        }

        private void TryActivate()
        {
            if (!_isViewCached || !_isStarted || !_isActive || _signals == null)
                return;

            if (!_isDefinitionBound)
            {
                BindDefinition();
                _isDefinitionBound = true;
            }

            _signals.Enable();
            RefreshState();
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            RefreshState();
        }
    }
}
