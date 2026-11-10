using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI.Intro;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class SceneIntroInstaller : Installer<IUIIntroView, SceneIntroInstaller>
    {
        private readonly IUIIntroView _view;

        public SceneIntroInstaller(IUIIntroView view)
        {
            _view = view;
        }

        public override void InstallBindings()
        {
            if (_view == null || (_view is Object viewObject && viewObject == null))
            {
                Debug.LogError($"{nameof(SceneIntroInstaller)} missing intro view; the scene UI intro is not bound.");
                return;
            }

            Container.Bind<IUIIntroView>().FromInstance(_view).AsSingle();
            Container.BindInterfacesTo<SceneIntro>().AsSingle();
        }
    }
}
