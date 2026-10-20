using System.Threading;
using Cysharp.Threading.Tasks;
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

        public UniTask<PlayerProgress> LoadAsync(CancellationToken cancellationToken)
            => UniTask.FromResult(JsonUtility.FromJson<PlayerProgress>(JsonUtility.ToJson(_repository.Progress)));

        public UniTask SaveAsync(PlayerProgress progress, CancellationToken cancellationToken)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(progress), _repository.Progress);
            _repository.Save();

            _signalBus.Fire(new CoinsChangedSignal(_repository.Progress.Coin));
            _signalBus.Fire<ProgressUpgradedSignal>();
            _signalBus.Fire(new CarSkinChangedSignal(_repository.Progress.SelectedCarSkinId));
            _signalBus.Fire<AudioSettingsChangedSignal>();
            return UniTask.CompletedTask;
        }

        public void Delete()
        {
        }
    }
}
