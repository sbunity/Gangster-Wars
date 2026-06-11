using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    // Handles trigger collisions for an enemy: bullet hits, grenade detonation, and the fire DoT.
    // Must live on the same GameObject as EnemyControllerBase and its trigger Collider2D so that
    // Unity routes OnTriggerEnter2D/OnTriggerExit2D here.
    public sealed class EnemyCollisionHandler : MonoBehaviour
    {
        private EnemyControllerBase _controller;
        private ICombatService _combatService;
        private Tween _fireTween;

        [Inject]
        public void Construct(ICombatService combatService)
        {
            _combatService = combatService;
        }

        private void Awake()
        {
            _controller = GetComponent<EnemyControllerBase>();
        }

        private void OnDisable()
        {
            _fireTween?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_controller == null)
                return;

            if (other.CompareTag("BulletHero") && !_controller.IsCollided)
            {
                var bullet = other.GetComponent<BaseBulletController>();
                _combatService.ApplyBulletHit(_controller, bullet);
            }
            else if (other.CompareTag("Grenade") && !_controller.IsCollided)
            {
                var grenade = other.GetComponent<GrenadeController>();
                if ((GrenadesName)grenade.Properties.Id == GrenadesName.Grenade_3)
                    grenade.Action(0);
            }

            if (other.CompareTag("Fire"))
            {
                _fireTween = DOVirtual.DelayedCall(1, () =>
                {
                    if (!_controller.IsCollided)
                        _controller.TakeDamage(1);
                }).SetLoops(-1);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _fireTween?.Kill();
        }
    }
}
