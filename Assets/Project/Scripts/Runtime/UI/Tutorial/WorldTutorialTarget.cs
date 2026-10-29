using SBabchuk.Runtime.Tutorial;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Tutorial
{
    public sealed class WorldTutorialTarget : TutorialTargetBase
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector2 _size = Vector2.one;

        private WorldTutorialAnchor _anchor;

        public override Rect ScreenRect => Anchor.ScreenRect;

        private WorldTutorialAnchor Anchor => _anchor ??= new WorldTutorialAnchor(transform, _camera, _size);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _size);
        }
    }
}
