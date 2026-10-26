using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Tutorial
{
    public sealed class TutorialBarricadeDamagePolicy : IBarricadeDamagePolicy
    {
        private readonly float _minHealth;

        public TutorialBarricadeDamagePolicy(IAssetProvider assetProvider)
        {
            _minHealth = assetProvider.TutorialConfig.BarricadeMinHealth;
        }

        public int Apply(int damage, int currentHealth, int maxHealth)
        {
            var floor = Mathf.CeilToInt(maxHealth * _minHealth);
            return Mathf.Clamp(damage, 0, Mathf.Max(0, currentHealth - floor));
        }
    }
}
