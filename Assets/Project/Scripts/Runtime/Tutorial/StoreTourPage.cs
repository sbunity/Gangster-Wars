using System;
using UnityEngine;

namespace SBabchuk.Runtime.Tutorial
{
    [Serializable]
    public sealed class StoreTourPage
    {
        [SerializeField] private StoreTab _tab = StoreTab.None;
        [SerializeField] private TutorialTargetId _target = TutorialTargetId.None;
        [SerializeField] private string _caption;

        public StoreTourPage()
        {
        }

        public StoreTourPage(StoreTab tab, TutorialTargetId target, string caption)
        {
            _tab = tab;
            _target = target;
            _caption = caption;
        }

        public StoreTab Tab => _tab;
        public TutorialTargetId Target => _target;
        public string Caption => _caption;
    }
}
