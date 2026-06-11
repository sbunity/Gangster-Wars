using System;
using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BuyPlayerController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("personage")]
        private PersonagesName _personage;

        [SerializeField, FormerlySerializedAs("unlockElement")]
        private GameObject _unlockElement;

        [SerializeField, FormerlySerializedAs("lockElement")]
        private GameObject _lockElement;

        [SerializeField, FormerlySerializedAs("panel")]
        private UIPanelController _panel;

        private PersonageShortInfo _personageShortInfo;
        private Personage _personageInfo;

        private Collider2D _coll;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        void OnEnable()
        {
            EasyTouch.On_TouchUp += OnTouchUp;
            _signalBus.Subscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
            _signalBus.Subscribe<UIPanelReplaceSignal>(Replace);
        }

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
            _coll = GetComponent<Collider2D>();
            CheckInteractive();
        }

        private void CheckInteractive()
        {
            _personageShortInfo = _progressService.GetPersonageShortInfo((int)_personage);
            ChangeLock(_personageShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            _unlockElement.SetActive(_value);
            _lockElement.SetActive(!_value);
        }

        private void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                _coll.enabled = false;
                _panel.Show(_panel);
            }
        }

        private void Replace(UIPanelReplaceSignal signal)
        {
            if (signal.Panel != _panel)
                _coll.enabled = true;
        }

        public void Hide()
        {
            _panel.Hide(_panel);
            _coll.enabled = true;
        }

        public void Buy()
        {
            _progressService.BuyPersonage((int)_personage);
        }
    }
}
