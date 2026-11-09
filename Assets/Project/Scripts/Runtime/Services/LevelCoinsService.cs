using System;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class LevelCoinsService : ILevelCoinsService, ICoinBalance, IInitializable, IDisposable
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ILevelFlowService _levelFlowService;
        private bool _isSettled;

        public LevelCoinsService(IPlayerProgressService progressService, ILevelFlowService levelFlowService)
        {
            _progressService = progressService;
            _levelFlowService = levelFlowService;
        }

        public event Action<int> Changed;

        public int Earned { get; private set; }

        public int Coins => _isSettled ? _progressService.Coins : _progressService.Coins + Earned;

        public void Initialize() => _levelFlowService.Finished += Settle;

        public void Dispose() => _levelFlowService.Finished -= Settle;

        public void Earn(int amount)
        {
            if (_isSettled || amount <= 0)
                return;

            Earned += amount;
            Changed?.Invoke(amount);
        }

        private void Settle(Panels result)
        {
            if (_isSettled)
                return;

            _isSettled = true;
            if (result == Panels.Win)
                Bank();
            else
                Forfeit();
        }

        private void Bank()
        {
            if (Earned > 0)
                _progressService.AddCoins(Earned);
        }

        private void Forfeit()
        {
            var forfeited = Earned;
            Earned = 0;
            Changed?.Invoke(-forfeited);
        }
    }
}
