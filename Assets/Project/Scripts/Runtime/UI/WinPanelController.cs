using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class WinPanelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private Panels _type;

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        private ISceneTransitionService _sceneTransitionService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(ISceneTransitionService sceneTransitionService, SignalBus signalBus)
        {
            _sceneTransitionService = sceneTransitionService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        public void Show(Panels panelType)
        {
            if (_type == panelType)
            {
                _panel.SetActive(true);
                Time.timeScale = 0;
            }
        }

        public void Hide()
        {
            _panel.SetActive(true);
        }

        private void OnGameFinished(GameFinishedSignal signal)
        {
            Show(signal.Panel);
        }

        public void SwitchScene()
        {
            Time.timeScale = 1;
            _sceneTransitionService?.TransitionToAsync(Scene.MainScene).Forget();
            Hide();
        }
    }
}
