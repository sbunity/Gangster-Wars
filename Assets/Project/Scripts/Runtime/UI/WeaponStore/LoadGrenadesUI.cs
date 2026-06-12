using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadGrenadesUI : LoadStoreListUI
    {
        private const float ElementOffset = 198f;

        [SerializeField, FormerlySerializedAs("rect")]
        private RectTransform _rect;

        [SerializeField, FormerlySerializedAs("elements")]
        private List<LoadGrenadeInfoUI> _elements;

        protected override void Refresh()
        {
            RebuildList(_rect, _elements, ElementOffset, element =>
            {
                var shortInfo = ProgressService.GetGrenadeShortInfo((int)element.Type);
                if (shortInfo.Count <= 0)
                    return false;

                var grenade = AssetProvider.BombStoreDatabase.GetGrenade((int)element.Type);
                element.Initialized(grenade.Icon, shortInfo.Count);
                return true;
            });
        }
    }
}
