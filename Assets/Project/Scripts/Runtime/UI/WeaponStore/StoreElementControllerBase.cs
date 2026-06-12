using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Services;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public abstract class StoreElementControllerBase : MonoBehaviour
    {
        private SignalSubscriptions _signals;

        protected IAssetProvider AssetProvider { get; private set; }
        protected IPlayerProgressService ProgressService { get; private set; }

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            AssetProvider = assetProvider;
            ProgressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        protected virtual void OnEnable() => _signals?.Enable();

        protected virtual void OnDisable() => _signals?.Disable();

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
