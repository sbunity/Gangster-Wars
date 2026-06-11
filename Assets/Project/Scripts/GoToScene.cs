using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class GoToScene : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("target")]
        private Scene _target;

        private ISceneLoaderService _sceneLoaderService;
        
        [Inject]
        public void Construct(ISceneLoaderService sceneLoaderService)
        {
            _sceneLoaderService = sceneLoaderService;
        }

        public void SwitchScene()
        {
            _sceneLoaderService?.LoadAsync(_target).Forget();
        }
    }
}