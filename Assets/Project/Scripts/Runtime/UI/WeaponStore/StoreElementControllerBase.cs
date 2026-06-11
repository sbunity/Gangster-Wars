using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public abstract class StoreElementControllerBase : MonoBehaviour
    {
        private SignalBus _signalBus;

        protected IAssetProvider AssetProvider { get; private set; }
        protected IPlayerProgressService ProgressService { get; private set; }

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            AssetProvider = assetProvider;
            ProgressService = progressService;
            _signalBus = signalBus;
        }

        protected virtual void OnEnable()
        {
            _signalBus?.Subscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        protected virtual void OnDisable()
        {
            _signalBus?.Unsubscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        protected abstract void RefreshState();

        protected void ApplyLockState(SpriteSwap panel, GameObject lockView, GameObject unlockView, bool isUnlocked)
        {
            panel.Change(isUnlocked);
            lockView.SetActive(!isUnlocked);
            unlockView.SetActive(isUnlocked);
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            RefreshState();
        }
    }
}
