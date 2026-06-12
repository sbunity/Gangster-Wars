using System;
using SBabchuk.Runtime.Databases.Levels;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelFlowService
    {
        event Action<Panels> Finished;
        Panels LastPanel { get; }
        bool IsFinished { get; }
        void Start(Level level);
        void Finish(Panels panel);
    }
}
