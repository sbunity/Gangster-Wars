using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Gameplay.Barricades
{
    public sealed class DefaultBarricadeDamagePolicy : IBarricadeDamagePolicy
    {
        public int Apply(int damage, int currentHealth, int maxHealth) => damage;
    }
}
