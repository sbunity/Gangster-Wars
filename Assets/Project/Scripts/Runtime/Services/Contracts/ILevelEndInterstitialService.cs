using System;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelEndInterstitialService
    {
        void ScheduleAfter(UniTask presentation);
        void RunAfterAd(Action action);
    }
}
