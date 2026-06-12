using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Enemies;

namespace SBabchuk.Runtime.Services
{
    public sealed class DamageService : IDamageService
    {
        public void DamageEnemy(EnemyControllerBase enemy, int damage)
        {
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        public void DamageBarricade(BarricadeController barricade, int damage)
        {
            if (barricade != null)
                barricade.TakeDamage(damage);
        }
    }
}
