using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.Constructor
{
    public class ConstructorController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("on")]
        private mySwitch _on = mySwitch.Off;

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        [SerializeField] private SettingsController _settingsController;

        private void Start()
        {
            _panel.SetActive(_on == mySwitch.On);
        }

        public void Settings()
        {
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            if (_settingsController == null)
                _settingsController = GetComponentInChildren<SettingsController>(true);
                
            _settingsController.ChangeVisible(Time.timeScale != 1);
        }
    }
}
