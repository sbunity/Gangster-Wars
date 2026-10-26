using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class ShootTutorialStep : ITutorialStep
    {
        private readonly SignalBus _signalBus;
        private readonly ILeaderWeaponController _leader;
        private readonly ITutorialView _view;
        private readonly TutorialConfig _config;
        private EnemyControllerBase _enemy;

        public ShootTutorialStep(SignalBus signalBus, ILeaderWeaponController leader, ITutorialView view, IAssetProvider assetProvider)
        {
            _signalBus = signalBus;
            _leader = leader;
            _view = view;
            _config = assetProvider.TutorialConfig;
        }

        public TutorialStepId Id => TutorialStepId.Shoot;
        public TutorialStepScope Scope => TutorialStepScope.Battle;

        public async UniTask WaitForTriggerAsync(CancellationToken cancellationToken)
            => _enemy = await TutorialEnemies.WaitForSpawnedEnemyAsync(_signalBus, _config.ShootDelayAfterSpawn, () => _leader.IsAttacking, cancellationToken);

        public async UniTask<bool> RunAsync(CancellationToken cancellationToken)
        {
            if (_leader.IsAttacking)
                return true;

            if (!TutorialEnemies.IsTargetable(_enemy))
                return false;

            var timeScale = new TutorialTimeScale();
            try
            {
                timeScale.SlowDown(_config.ShootTimeScale);
                var anchor = new WorldTutorialAnchor(_enemy.Center.GetTransform(), Camera.main, _config.EnemyTargetWorldSize);
                var hint = new TutorialHint(anchor, _config.ShootCaption, TutorialGesture.HoldAndDrag, TutorialInputMode.PassThrough, true);
                await _view.ShowUntilAsync(hint, UniTask.WaitUntil(() => _leader.IsAttacking || !TutorialEnemies.IsTargetable(_enemy), cancellationToken: cancellationToken));
                return _leader.IsAttacking;
            }
            finally
            {
                timeScale.Restore();
            }
        }

        public void Dispose()
        {
        }
    }
}
