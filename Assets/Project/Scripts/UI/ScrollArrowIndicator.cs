using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class ScrollArrowIndicator : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject arrowDown;
        [SerializeField] private GameObject arrowUp;

        private const float Threshold = 0.01f;

        private void Awake()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();

            var parent = transform.parent;
            if (parent == null) return;

            if (arrowDown == null)
            {
                var t = parent.Find("Image");
                if (t != null) arrowDown = t.gameObject;
            }

            if (arrowUp == null)
            {
                var t = parent.Find("ArrowUp");
                if (t != null) arrowUp = t.gameObject;
            }
        }

        private void Update()
        {
            if (scrollRect == null) return;

            float y = scrollRect.normalizedPosition.y;

            if (arrowDown != null)
                arrowDown.SetActive(y > Threshold);

            if (arrowUp != null)
                arrowUp.SetActive(y < 1f - Threshold);
        }
    }
}