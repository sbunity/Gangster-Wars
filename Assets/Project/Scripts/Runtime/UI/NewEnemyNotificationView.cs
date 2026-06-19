using System.Collections.Generic;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class NewEnemyNotificationView : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsRoot;
        [SerializeField] private NewEnemyNotificationElementView _elementPrefab;
        [SerializeField] private Vector2 _buttonSize = new(86f, 86f);
        [SerializeField] private float _spacing = 12f;
        [SerializeField] private int _maxVisibleButtons = 5;

        private readonly List<NewEnemyNotificationElementView> _elements = new List<NewEnemyNotificationElementView>();
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<NewEnemyDiscoveredSignal>(Show);
            _signals.Enable();
        }

        private void Awake()
        {
            EnsureRoot();
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void Show(NewEnemyDiscoveredSignal signal)
        {
            if (_itemsRoot == null || _elementPrefab == null)
                return;

            while (_elements.Count >= _maxVisibleButtons)
            {
                Destroy(_elements[0].gameObject);
                _elements.RemoveAt(0);
            }

            var element = Instantiate(_elementPrefab, _itemsRoot);
            element.gameObject.layer = gameObject.layer;
            element.Initialize(signal.EnemyId, signal.EnemyName, signal.Icon);
            _elements.Add(element);
            LayoutButtons();
            Animate(element.transform);
        }

        private void LayoutButtons()
        {
            for (var i = 0; i < _elements.Count; i++)
            {
                var rect = _elements[i].RectTransform;
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = _buttonSize;
                rect.anchoredPosition = new Vector2(0f, -i * (_buttonSize.y + _spacing));
            }
        }

        private void Animate(Transform target)
        {
            target.localScale = Vector3.one * 0.55f;
            target.DOScale(1f, 0.28f).SetEase(Ease.OutBack);
        }

        private void EnsureRoot()
        {
            if (_itemsRoot != null)
                return;

            _itemsRoot = (RectTransform)transform;
            _itemsRoot.anchorMin = new Vector2(0f, 1f);
            _itemsRoot.anchorMax = new Vector2(0f, 1f);
            _itemsRoot.pivot = new Vector2(0f, 1f);
        }
    }
}
