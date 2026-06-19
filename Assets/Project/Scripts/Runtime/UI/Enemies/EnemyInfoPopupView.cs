using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.Enemies
{
    public sealed class EnemyInfoPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _kindText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Transform _statsRoot;
        [SerializeField] private EnemyInfoStatRowView _rowTemplate;
        [SerializeField] private bool _pauseGameOnOpen = true;

        private readonly List<EnemyInfoStatRowView> _rows = new();
        private IEnemyInfoService _enemyInfoService;
        private SignalSubscriptions _signals;
        private float _previousTimeScale = 1f;
        private bool _isOpen;

        [Inject]
        public void Construct(IEnemyInfoService enemyInfoService, SignalBus signalBus)
        {
            _enemyInfoService = enemyInfoService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<NewEnemyNotificationSelectedSignal>(Open);
            _signals.Enable();
        }

        private void Awake()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(Close);

            if (_rowTemplate != null)
                _rowTemplate.gameObject.SetActive(false);

            if (_panel != null)
                _panel.SetActive(false);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(Close);
        }

        public void Open(NewEnemyNotificationSelectedSignal signal)
        {
            Open(signal.EnemyId);
        }

        public void Open(int enemyId)
        {
            var snapshot = _enemyInfoService?.GetInfo(enemyId);
            if (snapshot == null)
                return;

            Render(snapshot);
            PauseGame();

            if (_panel != null)
                _panel.SetActive(true);

            _isOpen = true;
        }

        public void Close()
        {
            if (!_isOpen)
                return;

            if (_panel != null)
                _panel.SetActive(false);

            ResumeGame();
            _isOpen = false;
        }

        private void Render(EnemyInfoSnapshot snapshot)
        {
            if (_icon != null)
            {
                _icon.sprite = snapshot.Icon;
                _icon.enabled = snapshot.Icon != null;
                _icon.preserveAspect = true;
            }

            if (_nameText != null)
                _nameText.text = snapshot.Name;

            if (_kindText != null)
                _kindText.text = snapshot.Kind;

            if (_descriptionText != null)
                _descriptionText.text = snapshot.Description;

            RenderRows(snapshot.Stats);
        }

        private void RenderRows(IReadOnlyList<EnemyInfoStatSnapshot> stats)
        {
            EnsureRows(stats.Count);

            for (var i = 0; i < _rows.Count; i++)
            {
                var isActive = i < stats.Count;
                _rows[i].gameObject.SetActive(isActive);

                if (isActive)
                    _rows[i].Render(stats[i]);
            }
        }

        private void EnsureRows(int count)
        {
            if (_rowTemplate == null || _statsRoot == null)
                return;

            while (_rows.Count < count)
            {
                var row = Instantiate(_rowTemplate, _statsRoot);
                _rows.Add(row);
            }
        }

        private void PauseGame()
        {
            if (!_pauseGameOnOpen || _isOpen)
                return;

            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void ResumeGame()
        {
            if (_pauseGameOnOpen)
                Time.timeScale = _previousTimeScale;
        }
    }
}
