using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Button))]
    public sealed class NewEnemyNotificationElementView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _badgeText;

        private RectTransform _rectTransform;
        private Button _button;
        private int _enemyId;

        public event Action<int> Clicked;
        public int EnemyId => _enemyId;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = (RectTransform)transform;

                return _rectTransform;
            }
        }

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);
        }

        public void Initialize(int enemyId, string enemyName, Sprite icon)
        {
            _enemyId = enemyId;
            gameObject.name = "NewEnemy_" + enemyId;

            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.enabled = icon != null;
                _icon.preserveAspect = true;
            }

            if (_badgeText != null)
                _badgeText.text = "!";
        }

        private void HandleClick()
        {
            Clicked?.Invoke(_enemyId);
        }
    }
}
