using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services
{
    public sealed class TutorialTargetRegistry : ITutorialTargetRegistry
    {
        private readonly Dictionary<TutorialTargetKey, ITutorialAnchor> _targets = new();

        public void Register(TutorialTargetKey key, ITutorialAnchor target)
        {
            if (target == null || key.Id == TutorialTargetId.None)
                return;

            _targets[key] = target;
        }

        public void Unregister(TutorialTargetKey key, ITutorialAnchor target)
        {
            if (_targets.TryGetValue(key, out var registeredTarget) && ReferenceEquals(registeredTarget, target))
                _targets.Remove(key);
        }

        public bool TryGet(TutorialTargetKey key, out ITutorialAnchor target)
        {
            if (_targets.TryGetValue(key, out target) && !IsDestroyed(target) && target.IsValid)
                return true;

            target = null;
            return false;
        }

        public async UniTask<ITutorialAnchor> WaitForAsync(TutorialTargetKey key, CancellationToken cancellationToken)
        {
            ITutorialAnchor target = null;
            await UniTask.WaitUntil(() => TryGet(key, out target), cancellationToken: cancellationToken);
            return target;
        }

        private static bool IsDestroyed(ITutorialAnchor target)
            => target == null || (target is UnityEngine.Object unityObject && unityObject == null);
    }
}
