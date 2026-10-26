using UnityEngine;

namespace SBabchuk.Runtime.UI.Tutorial
{
    public sealed class TutorialHoleRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        private RectTransform _space;
        private Rect _hole;
        private bool _hasHole;

        public void SetHole(RectTransform space, Rect localHole)
        {
            _space = space;
            _hole = localHole;
            _hasHole = true;
        }

        public void ClearHole() => _hasHole = false;

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!_hasHole || _space == null)
                return true;

            return !RectTransformUtility.ScreenPointToLocalPointInRectangle(_space, screenPoint, eventCamera, out var localPoint)
                   || !_hole.Contains(localPoint);
        }
    }
}
