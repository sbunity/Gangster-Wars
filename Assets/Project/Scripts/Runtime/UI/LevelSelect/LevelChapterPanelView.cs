using SBabchuk.Runtime.Databases.Levels;
using TMPro;
using UnityEngine;

namespace SBabchuk.Runtime.UI.LevelSelect
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class LevelChapterPanelView : MonoBehaviour
    {
        private static readonly Vector2 LeftMiddle = new(0f, 0.5f);
        private static readonly Vector2 Center = new(0.5f, 0.5f);

        [SerializeField] private ChapterBackgroundView _background;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform _levelButtonsRoot;

        private RectTransform RectTransform => (RectTransform)transform;

        public void Bind(ChapterDatabase chapter)
        {
            if (chapter == null)
                return;

            if (_title != null)
                _title.text = chapter.Name;

            BindLevelButtons(chapter);
        }

        public float ApplyLayout(Vector2 viewportSize)
        {
            var backgroundWidth = _background != null ? _background.FitToViewport(viewportSize) : 0f;
            var slotWidth = Mathf.Max(viewportSize.x, backgroundWidth);

            var rectTransform = RectTransform;
            rectTransform.anchorMin = LeftMiddle;
            rectTransform.anchorMax = LeftMiddle;
            rectTransform.pivot = Center;
            rectTransform.sizeDelta = new Vector2(slotWidth, viewportSize.y);

            return slotWidth;
        }

        public void SetCenter(float x)
            => RectTransform.anchoredPosition = new Vector2(x, 0f);

        private void BindLevelButtons(ChapterDatabase chapter)
        {
            if (_levelButtonsRoot == null || chapter.Levels == null)
                return;

            var levelIndex = 0;
            foreach (Transform child in _levelButtonsRoot)
            {
                if (levelIndex >= chapter.Levels.Count)
                    return;

                if (child.TryGetComponent(out BttnSelectLvlController button))
                    button.Init(chapter.Levels[levelIndex++].Id);
            }
        }
    }
}
