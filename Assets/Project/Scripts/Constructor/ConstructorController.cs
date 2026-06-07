using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class ConstructorController : MonoBehaviour
    {
        public delegate void ChangeVisibleSettings(bool _value);
        public static ChangeVisibleSettings OnChangeVisibleSettings;

        [Header("Чи включений конструктор?")]
        public mySwitch on = mySwitch.Off;

        [Header("Панел")]
        public GameObject panel;

        static public mySwitch On;

        void Awake()
        {
            On = on;
        }

        private void Start()
        {
            panel.SetActive(ConstructorController.On == mySwitch.On);
        }

        public void Settings()
        {
            Time.timeScale = Time.timeScale == 1? 0 : 1;

            OnChangeVisibleSettings?.Invoke(Time.timeScale != 1);
        }
    }
}
