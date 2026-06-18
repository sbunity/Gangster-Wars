using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI.WeaponStore;
using SBabchuk.Runtime.UI.WeaponStore.Info;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public sealed class MainPlayerStatsView : MonoBehaviour
    {
        [SerializeField] private PersonagesName _personage;
        [SerializeField] private bool _showUpgradeDelta;
        [SerializeField] private Transform _statsRoot;
        [SerializeField] private StoreInfoStatRowView _rowTemplate;

        private readonly List<StoreInfoStatRowView> _rows = new();
        private IMainPlayerStatsService _statsService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(IMainPlayerStatsService statsService, SignalBus signalBus)
        {
            _statsService = statsService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnEnable()
        {
            _signals?.Enable();
            Refresh();
        }

        private void OnDisable()
        {
            _signals?.Disable();
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_statsService == null || _rowTemplate == null)
                return;

            var snapshot = _statsService.GetStats((int)_personage, _showUpgradeDelta);
            EnsureRows(snapshot.Stats.Count);

            for (var i = 0; i < _rows.Count; i++)
            {
                var isActive = i < snapshot.Stats.Count;
                _rows[i].gameObject.SetActive(isActive);
                if (isActive)
                    _rows[i].Render(snapshot.Stats[i]);
            }
        }

        private void EnsureRows(int count)
        {
            var root = _statsRoot ? _statsRoot : transform;
            while (_rows.Count < count)
            {
                var row = Instantiate(_rowTemplate, root);
                row.gameObject.SetActive(true);
                _rows.Add(row);
            }
        }
    }
}
