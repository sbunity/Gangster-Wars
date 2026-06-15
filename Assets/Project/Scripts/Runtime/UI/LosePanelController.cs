using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI
{
    public class LosePanelController : GameResultPanelController
    {
        [SerializeField] private Button _continueButton;

        [SerializeField] private Button _restartButton;

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(ReturnToMainMenu);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(RestartLevel);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(ReturnToMainMenu);

            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(RestartLevel);
        }

        public void ReturnToMainMenu() => TransitionTo(Scene.MainScene);

        public void RestartLevel() => TransitionTo(Scene.GameScene);
    }
}
