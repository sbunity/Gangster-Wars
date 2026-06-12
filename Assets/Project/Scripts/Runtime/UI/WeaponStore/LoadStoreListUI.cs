using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    /// <summary>
    /// Shared base for store list loaders (owned weapons / grenades panels): handles DI, the
    /// ProgressUpgraded subscription and the "lay out only the owned elements, sizing the rect" loop.
    /// Subclasses keep their own serialized element list and decide per element whether it is shown.
    /// </summary>
    public abstract class LoadStoreListUI : MonoBehaviour
    {
        protected IAssetProvider AssetProvider { get; private set; }
        protected IPlayerProgressService ProgressService { get; private set; }
        private SignalSubscriptions _signals;

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

        protected virtual void Start() => Refresh();

        protected abstract void Refresh();

        private void OnProgressUpgraded(ProgressUpgradedSignal signal) => Refresh();

        /// <summary>
        /// Resets the rect width, then for each element calls <paramref name="tryActivate"/>:
        /// shown elements are activated and grow the rect by <paramref name="offset"/>, the rest are hidden.
        /// </summary>
        protected static void RebuildList<TElement>(RectTransform rect, List<TElement> elements, float offset, Func<TElement, bool> tryActivate)
            where TElement : Component
        {
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
            foreach (var element in elements)
            {
                if (tryActivate(element))
                {
                    element.gameObject.SetActive(true);
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x + offset, rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
