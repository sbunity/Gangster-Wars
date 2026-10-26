using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Tutorial;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class TutorialInstaller : Installer<ITutorialView, IReadOnlyList<Type>, TutorialInstaller>
    {
        private readonly ITutorialView _view;
        private readonly IReadOnlyList<Type> _steps;

        public TutorialInstaller(ITutorialView view, IReadOnlyList<Type> steps)
        {
            _view = view;
            _steps = steps;
        }

        public override void InstallBindings()
        {
            if (_view == null || (_view is UnityEngine.Object viewObject && viewObject == null))
            {
                Debug.LogError($"{nameof(TutorialInstaller)} missing tutorial view; tutorial steps are not bound.");
                return;
            }

            Container.Bind<ITutorialView>().FromInstance(_view).AsSingle();
            foreach (var step in _steps)
                Container.Bind<ITutorialStep>().To(step).AsSingle();
            Container.BindInterfacesTo<TutorialDirector>().AsSingle();
        }
    }
}
