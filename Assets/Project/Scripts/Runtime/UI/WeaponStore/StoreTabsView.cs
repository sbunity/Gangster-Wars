using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public sealed class StoreTabsView : MonoBehaviour, IStoreTabs
    {
        [SerializeField] private List<StoreTabEntry> _tabs = new();

        public StoreTab Current
        {
            get
            {
                var active = _tabs.Find(entry => entry.Controller != null && entry.Controller.IsActive);
                return active?.Tab ?? StoreTab.None;
            }
        }

        public void Select(StoreTab tab)
        {
            var entry = _tabs.Find(candidate => candidate.Tab == tab);
            if (entry?.Controller != null)
                entry.Controller.SetFocus();
        }

        [Serializable]
        private sealed class StoreTabEntry
        {
            [SerializeField] private StoreTab _tab = StoreTab.None;
            [SerializeField] private TabController _controller;

            public StoreTab Tab => _tab;
            public TabController Controller => _controller;
        }
    }
}
