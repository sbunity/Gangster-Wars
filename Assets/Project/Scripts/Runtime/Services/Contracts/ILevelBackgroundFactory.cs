using SBabchuk.Runtime.Gameplay.Levels;
using UnityEngine;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ILevelBackgroundFactory
    {
        LevelBackground Create(int levelId, Transform parent);
    }
}
