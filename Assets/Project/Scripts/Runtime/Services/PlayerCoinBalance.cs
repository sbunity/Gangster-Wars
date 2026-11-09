using System;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerCoinBalance : ICoinBalance, IInitializable, IDisposable
    {
        private readonly IPlayerProgressService _progressService;
        private readonly SignalSubscriptions _signals;

        public PlayerCoinBalance(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<CoinsChangedSignal>(OnCoinsChanged);
        }

        public event Action<int> Changed;

        public int Coins => _progressService.Coins;

        public void Initialize() => _signals.Enable();

        public void Dispose() => _signals.Disable();

        private void OnCoinsChanged(CoinsChangedSignal signal)
            => Changed?.Invoke(signal.Delta);
    }
}
