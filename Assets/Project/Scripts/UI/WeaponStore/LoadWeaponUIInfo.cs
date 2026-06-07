using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{

    public class LoadWeaponUIInfo : MonoBehaviour
    {
        [Header("Тип гранати")]
        public WeaponsName type;

        [Header("Кількість")]
        public Text count;

        [Header("Кнопка")]
        public Button bttn;

        private void OnEnable()
        {
            PlayerPrefsDatabase.OnUpdateWeaponPatrons += UpdateWeaponPatrons;
        }

        private void OnDisable()
        {
            PlayerPrefsDatabase.OnUpdateWeaponPatrons -= UpdateWeaponPatrons;
        }

        public void Initialized(int _count)
        {
            count.text = _count.ToString();
        }

        public void UpdateWeaponPatrons(WeaponsName _weaponsName, int _count)
        {
            if (type == _weaponsName)
            {
                count.text = _count.ToString();
            }
        }
    }
}
