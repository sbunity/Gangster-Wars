using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class LoadGrenadesUI : MonoBehaviour
    {
        [Header("Елемент LayoutGroup")]
        public RectTransform rect;

        [Header("Елементи UI")]
        public List<LoadGrenadeInfoUI> elements;

        private void OnEnable()
        {
            PlayerPrefsDatabase.OnUpgraded += Initialized;
        }

        private void OnDisable()
        {
            PlayerPrefsDatabase.OnUpgraded -= Initialized;
        }

        private void Start()
        {
            Initialized();
        }

        private void Initialized()
        {
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);

            foreach (LoadGrenadeInfoUI element in elements)
            {
                GrenadeShortInfo shortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetGrenadeShortInfo((int)element.type);

                Grenade grenade = PersistableSO.Instance.BombStore.GetGrenade((int)element.type);

                if (shortInfo.count > 0)
                {
                    element.Initialized(grenade.ico, shortInfo.count);

                    element.gameObject.SetActive(true);

                    rect.sizeDelta = new Vector2(rect.sizeDelta.x + 198, rect.sizeDelta.y);
                }
                else
                {
                    element.gameObject.SetActive(false);
                }
            }
        }
    }
}
