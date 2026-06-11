using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class WinPanelController : MonoBehaviour
    {
        public Panels type;

        public GameObject panel;
        private ISceneLoaderService _sceneLoaderService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            ISceneLoaderService sceneLoaderService,
            SignalBus signalBus)
        {
            _sceneLoaderService = sceneLoaderService;
            _signalBus = signalBus;
        }

        public void OnEnable()
        {
            if (_signalBus != null)
                _signalBus.Subscribe<GameFinishedSignal>(OnGameFinished);
        }

        public void OnDisable()
        {
            if (_signalBus != null)
                _signalBus.Unsubscribe<GameFinishedSignal>(OnGameFinished);
        }

        public void Show(Panels _panel)
        {
            Debug.Log("Show" + _panel);
            if (type == _panel)
            {
                panel.SetActive(true);

                Time.timeScale = 0;
            }
        }

        public void Hide()
        {
            panel.SetActive(true);
        }

        private void OnGameFinished(GameFinishedSignal signal)
        {
            Show(signal.Panel);
        }

        public void SwitchScene()
        {
            Time.timeScale = 1;

            _sceneLoaderService?.LoadAsync(Scene.MainScene).Forget();

            Hide();
        }
    }
}
