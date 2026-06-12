using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadWeaponsUI : LoadStoreListUI
    {
        [SerializeField, FormerlySerializedAs("rect")]
        private RectTransform _rect;

        [SerializeField, FormerlySerializedAs("elements")]
        private List<LoadWeaponUIInfo> _elements;

        [SerializeField, FormerlySerializedAs("offset"), Range(200, 500)]
        private float _offset = 200;

        protected override void Refresh()
        {
            RebuildList(_rect, _elements, _offset, element =>
            {
                var shortInfo = ProgressService.GetWeaponShortInfo((int)element.Type);
                if (shortInfo.AmmoCount <= 0)
                    return false;

                element.Initialized(shortInfo.AmmoCount);
                return true;
            });
        }
    }
}
