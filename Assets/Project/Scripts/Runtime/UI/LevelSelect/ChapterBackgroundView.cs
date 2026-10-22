using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.LevelSelect
{
    [RequireComponent(typeof(Image))]
    public sealed class ChapterBackgroundView : MonoBehaviour
    {
        private static readonly Vector2 Center = new(0.5f, 0.5f);

        [SerializeField] private Image _image;

        public float FitToViewport(Vector2 viewportSize)
        {
            var size = CalculateCoverSize(viewportSize);
            var rectTransform = _image.rectTransform;

            rectTransform.anchorMin = Center;
            rectTransform.anchorMax = Center;
            rectTransform.pivot = Center;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            return size.x;
        }

        private Vector2 CalculateCoverSize(Vector2 viewportSize)
        {
            if (_image.sprite == null)
                return viewportSize;

            var spriteSize = _image.sprite.rect.size;
            var scale = Mathf.Max(viewportSize.x / spriteSize.x, viewportSize.y / spriteSize.y);
            return spriteSize * scale;
        }

        private void Reset()
            => _image = GetComponent<Image>();
    }
}
