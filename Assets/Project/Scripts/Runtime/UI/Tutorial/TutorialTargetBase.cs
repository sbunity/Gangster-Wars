using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.Tutorial
{
    public abstract class TutorialTargetBase : MonoBehaviour, ITutorialAnchor
    {
        [SerializeField] private TutorialTargetId _id = TutorialTargetId.None;
        [SerializeField] private int _index;

        private ITutorialTargetRegistry _registry;
        private bool _isRegistered;

        public bool IsValid => this != null && isActiveAndEnabled;

        public abstract Rect ScreenRect { get; }

        private TutorialTargetKey Key => new(_id, _index);

        [Inject]
        public void Construct(ITutorialTargetRegistry registry)
        {
            _registry = registry;
            TryRegister();
        }

        private void OnEnable() => TryRegister();

        private void OnDisable() => Unregister();

        private void TryRegister()
        {
            if (_isRegistered || _registry == null || !isActiveAndEnabled || _id == TutorialTargetId.None)
                return;

            _registry.Register(Key, this);
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered || _registry == null)
                return;

            _registry.Unregister(Key, this);
            _isRegistered = false;
        }
    }
}
