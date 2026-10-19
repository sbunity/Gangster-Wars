using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.EditorTools
{
    public sealed class LiveProgressEditSource : IProgressEditSource
    {
        private readonly IPlayerProgressRepository _repository;
        private readonly SignalBus _signalBus;

        public LiveProgressEditSource(IPlayerProgressRepository repository, SignalBus signalBus)
        {
            _repository = repository;
            _signalBus = signalBus;
        }

        public string Title => "Live progress (Play Mode)";
        public bool CanDelete => false;

        public bool TryLoad(out PlayerProgress progress)
        {
            progress = JsonUtility.FromJson<PlayerProgress>(JsonUtility.ToJson(_repository.Progress));
            return true;
        }

        public void Save(PlayerProgress progress)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(progress), _repository.Progress);
            _repository.Save();

            _signalBus.Fire(new CoinsChangedSignal(_repository.Progress.Coin));
            _signalBus.Fire<ProgressUpgradedSignal>();
            _signalBus.Fire<AudioSettingsChangedSignal>();
        }

        public void Delete()
        {
        }
    }
}
