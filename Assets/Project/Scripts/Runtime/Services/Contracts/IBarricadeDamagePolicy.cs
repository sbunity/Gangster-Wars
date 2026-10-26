namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBarricadeDamagePolicy
    {
        int Apply(int damage, int currentHealth, int maxHealth);
    }
}
