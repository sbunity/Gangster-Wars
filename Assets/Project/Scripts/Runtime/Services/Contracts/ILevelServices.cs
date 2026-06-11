using System;
using UnityEngine;

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

    public interface IAimService
    {
        Vector2 CurrentAimPosition { get; }
    }
}