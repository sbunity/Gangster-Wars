using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Services.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class LevelChapterCarouselController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Button _previousButton;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private List<RectTransform> _chapterPanels = new List<RectTransform>();

        [SerializeField]
        private float _snapDuration = 0.28f;

        [SerializeField]
        private float _swipeThreshold = 80f;

        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private Coroutine _snapCoroutine;
        private int _currentChapterIndex;
        private float _dragStartPosition;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        private void Awake()
        {
            if (_previousButton != null)
                _previousButton.onClick.AddListener(ShowPreviousChapter);

            if (_nextButton != null)
                _nextButton.onClick.AddListener(ShowNextChapter);
        }

        private IEnumerator Start()
        {
            yield return null;

            InitializeChapters();
            SetChapter(GetInitialChapterIndex(), true);
        }

        private void OnDestroy()
        {
            if (_previousButton != null)
                _previousButton.onClick.RemoveListener(ShowPreviousChapter);

            if (_nextButton != null)
                _nextButton.onClick.RemoveListener(ShowNextChapter);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStartPosition = eventData.position.x;

            if (_snapCoroutine == null)
                return;

            StopCoroutine(_snapCoroutine);
            _snapCoroutine = null;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _scrollRect.StopMovement();

            var delta = eventData.position.x - _dragStartPosition;
            if (Mathf.Abs(delta) < _swipeThreshold)
            {
                SetChapter(_currentChapterIndex, false);
                return;
            }

            SetChapter(delta < 0f ? _currentChapterIndex + 1 : _currentChapterIndex - 1, false);
        }

        private void ShowPreviousChapter()
            => SetChapter(_currentChapterIndex - 1, false);

        private void ShowNextChapter()
            => SetChapter(_currentChapterIndex + 1, false);

        private void InitializeChapters()
        {
            var chapters = _assetProvider.LevelDatabase.Chapters;
            var count = Mathf.Min(_chapterPanels.Count, chapters.Count);

            for (var i = 0; i < count; i++)
                InitializeChapterPanel(_chapterPanels[i], chapters[i]);
        }

        private void InitializeChapterPanel(RectTransform panel, ChapterDatabase chapter)
        {
            if (panel == null || chapter == null)
                return;

            SetChapterTitle(panel, chapter.Name);

            var buttonsRoot = panel.Find("Buttons");
            if (buttonsRoot == null || chapter.Levels == null)
                return;

            var buttons = new List<BttnSelectLvlController>();
            foreach (Transform child in buttonsRoot)
            {
                if (child.TryGetComponent(out BttnSelectLvlController button))
                    buttons.Add(button);
            }

            buttons.Sort((left, right) => left.transform.GetSiblingIndex().CompareTo(right.transform.GetSiblingIndex()));

            var count = Mathf.Min(buttons.Count, chapter.Levels.Count);
            for (var i = 0; i < count; i++)
                buttons[i].Init(chapter.Levels[i].Id);
        }

        private static void SetChapterTitle(RectTransform panel, string title)
        {
            var titleTransform = panel.Find("ChapterName");
            if (titleTransform == null)
                return;

            if (titleTransform.TryGetComponent(out TMP_Text tmpText))
            {
                tmpText.text = title;
                return;
            }

            if (titleTransform.TryGetComponent(out Text text))
                text.text = title;
        }

        private int GetInitialChapterIndex()
        {
            var chapters = _assetProvider.LevelDatabase.Chapters;
            var currentChapterId = _progressService.CurrentChapterId;
            for (var i = 0; i < chapters.Count; i++)
            {
                if (chapters[i] != null && chapters[i].Id == currentChapterId)
                    return i;
            }

            return 0;
        }

        private void SetChapter(int index, bool immediate)
        {
            if (_chapterPanels.Count == 0)
                return;

            _scrollRect.StopMovement();
            _currentChapterIndex = Mathf.Clamp(index, 0, _chapterPanels.Count - 1);
            UpdateArrowState();

            var target = GetNormalizedPosition(_currentChapterIndex);
            if (immediate || _snapDuration <= 0f)
            {
                SetNormalizedPosition(target);
                return;
            }

            if (_snapCoroutine != null)
                StopCoroutine(_snapCoroutine);

            _snapCoroutine = StartCoroutine(SnapTo(target));
        }

        private IEnumerator SnapTo(float target)
        {
            var start = _scrollRect.horizontalNormalizedPosition;
            var elapsed = 0f;

            while (elapsed < _snapDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _snapDuration);
                SetNormalizedPosition(Mathf.SmoothStep(start, target, t));
                yield return null;
            }

            SetNormalizedPosition(target);
            _snapCoroutine = null;
        }

        private float GetNormalizedPosition(int index)
        {
            if (_chapterPanels.Count <= 1)
                return 0f;

            return index / (float)(_chapterPanels.Count - 1);
        }

        private void SetNormalizedPosition(float value)
        {
            if (_scrollRect == null)
                return;

            _scrollRect.horizontalNormalizedPosition = value;
        }

        private void UpdateArrowState()
        {
            if (_previousButton != null)
                _previousButton.interactable = _currentChapterIndex > 0;

            if (_nextButton != null)
                _nextButton.interactable = _currentChapterIndex < _chapterPanels.Count - 1;
        }
    }
}
