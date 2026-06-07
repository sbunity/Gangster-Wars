using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class LoadWeaponsUI : MonoBehaviour
    {
        [Header("Елемент LayoutGroup")]
        public RectTransform rect;

        [Header("Елементи UI")]
        public List<LoadWeaponUIInfo> elements;

        [Header("Відступ")]
        [Range(200, 500)]
        public float offset = 200;

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

            foreach (LoadWeaponUIInfo element in elements)
            {
                WeaponShortInfo shortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo((int)element.type);

                Weapon wepaon = PersistableSO.Instance.WeaponStore.GetWeapon((int)element.type);

                if (shortInfo.countPatrons > 0)
                {
                    element.Initialized(shortInfo.countPatrons);

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
