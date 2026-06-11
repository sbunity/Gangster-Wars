using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class GoToScene : MonoBehaviour
    {
        public Scene target;
        private ISceneLoaderService _sceneLoaderService;

        [Inject]
        private void Construct(ISceneLoaderService sceneLoaderService)
        {
            _sceneLoaderService = sceneLoaderService;
        }

        public void SwitchScene()
        {
            Debug.Log("SceneName to load: " + target);

            _sceneLoaderService?.LoadAsync(target).Forget();
        }
    }
}
