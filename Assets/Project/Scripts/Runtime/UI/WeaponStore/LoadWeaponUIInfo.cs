using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadWeaponUIInfo : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private WeaponsName _type;
        public WeaponsName Type { get => _type; set => _type = value; }

        [SerializeField, FormerlySerializedAs("count")]
        private Text _count;

        [SerializeField, FormerlySerializedAs("bttn")]
        private Button _bttn;

        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<WeaponAmmoChangedSignal>(OnWeaponAmmoChanged);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void OnWeaponAmmoChanged(WeaponAmmoChangedSignal signal)
        {
            UpdateWeaponPatrons(signal.Weapon, signal.Count);
        }

        public void Initialized(int count)
        {
            _count.text = count.ToString();
        }

        public void UpdateWeaponPatrons(WeaponsName weaponsName, int count)
        {
            if (_type == weaponsName)
                _count.text = count.ToString();
        }
    }
}
