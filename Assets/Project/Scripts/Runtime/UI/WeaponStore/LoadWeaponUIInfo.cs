using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus?.Subscribe<WeaponAmmoChangedSignal>(OnWeaponAmmoChanged);
        }

        private void OnDisable()
        {
            _signalBus?.Unsubscribe<WeaponAmmoChangedSignal>(OnWeaponAmmoChanged);
        }

        private void OnWeaponAmmoChanged(WeaponAmmoChangedSignal signal)
        {
            UpdateWeaponPatrons(signal.Weapon, signal.Count);
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
