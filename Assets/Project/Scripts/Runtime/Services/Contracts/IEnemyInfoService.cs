using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IEnemyInfoService
    {
        EnemyInfoSnapshot GetInfo(int enemyId);
    }
}
