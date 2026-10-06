using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelActorsLayout : MonoBehaviour
    {
        [SerializeField] private Transform _barricade;
        [SerializeField] private Transform _leader;
        [SerializeField] private Transform _sniper;
        [SerializeField] private Transform _bomber;

        [Inject]
        public void Construct(LevelBackground background)
        {
            Place(_barricade, background.BarricadePoint);
            Place(_leader, background.LeaderPoint);
            Place(_sniper, background.SniperPoint);
            Place(_bomber, background.BomberPoint);
        }

        private static void Place(Transform actor, Transform anchor)
        {
            if (actor == null || anchor == null)
                return;

            var position = anchor.position;
            position.z = actor.position.z;
            actor.position = position;
        }
    }
}
