using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class GoToScene : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("target")]
        private Scene _target;

        private ISceneTransitionService _sceneTransitionService;

        [Inject]
        public void Construct(ISceneTransitionService sceneTransitionService)
        {
            _sceneTransitionService = sceneTransitionService;
        }

        public void SwitchScene()
        {
            _sceneTransitionService?.TransitionToAsync(_target).Forget();
        }
    }
}
