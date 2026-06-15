using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Factories
{
    public sealed class LoadingScreenFactory : ILoadingScreenFactory
    {
        private readonly DiContainer _container;
        private readonly LoadingScreenView _prefab;

        public LoadingScreenFactory(DiContainer container, LoadingScreenView prefab)
        {
            _container = container;
            _prefab = prefab;
        }

        public ILoadingScreen Create()
        {
            var view = _container.InstantiatePrefabForComponent<LoadingScreenView>(_prefab);
            view.transform.SetParent(null, false);
            Object.DontDestroyOnLoad(view.gameObject);
            return view;
        }
    }
}
