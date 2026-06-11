using System;
using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class BuyPlayerController : MonoBehaviour
    {
        public PersonagesName personage;

        private PersonageShortInfo personageShortInfo;

        private Personage personageInfo;

        [Header("Елементи при розблокуванні")]
        public GameObject unlockElement;

        [Header("Елементи при заблокуванні")]
        public GameObject lockElement;

        [Header("UI панелька")]
        public UIPanelController panel;

        Collider2D coll;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Встановити підписку на евенти
        /// </summary>
        void OnEnable()
        {
            EasyTouch.On_TouchUp += OnTouchUp;
            _signalBus.Subscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
            _signalBus.Subscribe<UIPanelReplaceSignal>(Replace);
        }

        /// <summary>
        /// Втратити підписку на евенти
        /// </summary>
        void OnDisable()
        {
            EasyTouch.On_TouchUp -= OnTouchUp;
            _signalBus.Unsubscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
            _signalBus.Unsubscribe<UIPanelReplaceSignal>(Replace);
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            CheckInteractive();
        }

        private void Start()
        {
            coll = GetComponent<Collider2D>();

            CheckInteractive();
        }

        /// <summary>
        /// Перевірка на інтерактивність
        /// </summary>
        private void CheckInteractive()
        {
            personageShortInfo = _progressService.GetPersonageShortInfo((int)personage);

            ChangeLock(personageShortInfo.isBuy == mySwitch.On);
        }

        /// <summary>
        /// Включаєм певні елементи 
        /// </summary>
        /// <param name="_value"></param>
        private void ChangeLock(bool _value = false)
        {
            unlockElement.SetActive(_value);

            lockElement.SetActive(!_value);
        }

        private void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                coll.enabled = false;

                panel.Show(panel);
            }
        }

        private void Replace(UIPanelReplaceSignal signal)
        {
            if(signal.Panel != panel)
                coll.enabled = true;
        }

        public void Hide()
        {
            panel.Hide(panel);

            coll.enabled = true;
        }

        public void Buy()
        {
            //panel.Hide(panel);

            _progressService.BuyPersonage((int)personage);
        }
    }
}
