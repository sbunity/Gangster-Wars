using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBonusCollectTarget
    {
        Vector3 ScreenPosition { get; }

        void PlayCollectFeedback();
    }
}
