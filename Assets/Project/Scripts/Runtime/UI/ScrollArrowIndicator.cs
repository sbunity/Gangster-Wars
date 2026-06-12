using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI
{
    public class ScrollArrowIndicator : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject arrowDown;
        [SerializeField] private GameObject arrowUp;

        private const float THRESHOLD = 0.01f;

        private void Awake()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();

            var parent = transform.parent;
            if (parent == null)
                return;

            if (arrowDown == null)
            {
                var t = parent.Find("Image");
                if (t != null)
                    arrowDown = t.gameObject;
            }

            if (arrowUp == null)
            {
                var t = parent.Find("ArrowUp");
                if (t != null)
                    arrowUp = t.gameObject;
            }
        }

        private void Update()
        {
            if (scrollRect == null)
                return;

            var y = scrollRect.normalizedPosition.y;

            if (arrowDown != null)
                arrowDown.SetActive(y > THRESHOLD);
                
            if (arrowUp != null)
                arrowUp.SetActive(y < 1f - THRESHOLD);
        }
    }
}
