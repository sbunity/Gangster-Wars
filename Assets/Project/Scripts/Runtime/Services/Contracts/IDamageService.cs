using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IDamageService
    {
        void DamageEnemy(EnemyControllerBase enemy, int damage);
        void DamageBarricade(BarricadeController barricade, int damage);
    }
}
