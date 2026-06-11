using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class ConstructorController : MonoBehaviour
    {
        [Header("Чи включений конструктор?")]
        public mySwitch on = mySwitch.Off;

        [Header("Панел")]
        public GameObject panel;

        [SerializeField] private SettingsController _settingsController;

        private void Start()
        {
            panel.SetActive(on == mySwitch.On);
        }

        public void Settings()
        {
            Time.timeScale = Time.timeScale == 1? 0 : 1;

            if (_settingsController == null)
                _settingsController = GetComponentInChildren<SettingsController>(true);

            _settingsController.ChangeVisible(Time.timeScale != 1);
        }
    }
}
