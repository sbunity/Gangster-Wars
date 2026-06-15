using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class SurrenderButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private SettingsPopupController _settingsPopup;

        private ILevelFlowService _levelFlowService;

        [Inject]
        public void Construct(ILevelFlowService levelFlowService)
        {
            _levelFlowService = levelFlowService;
        }

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(Surrender);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Surrender);
        }

        private void Surrender()
        {
            if (_levelFlowService == null || _levelFlowService.IsFinished)
                return;

            _settingsPopup?.Close();
            _levelFlowService.Finish(Panels.Lose);
        }
    }
}
