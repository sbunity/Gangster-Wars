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
            AddChoice(_continueButton, ReturnToMainMenu);
            AddChoice(_restartButton, RestartLevel);
        }

        public void ReturnToMainMenu() => TransitionTo(Scene.MainScene);

        public void RestartLevel() => TransitionTo(Scene.GameScene);
    }
}
