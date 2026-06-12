using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using SBabchuk.Runtime.Gameplay.Grenades;
using SBabchuk.Runtime.Gameplay.Projectiles;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    // Handles direct trigger collisions for an enemy: bullet hits and grenade contact detonation.
    // Must live on the same GameObject as EnemyControllerBase and its trigger Collider2D so that
    // Unity routes OnTriggerEnter2D/OnTriggerExit2D here.
    public sealed class EnemyCollisionHandler : MonoBehaviour
    {
        private EnemyControllerBase _controller;
        private ICombatService _combatService;

        [Inject]
        public void Construct(ICombatService combatService)
        {
            _combatService = combatService;
        }

        private void Awake()
        {
            _controller = GetComponent<EnemyControllerBase>();
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
                other.GetComponent<GrenadeController>()?.NotifyContact();
            }
        }
    }
}
