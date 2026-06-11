using UnityEngine;

namespace SBabchuk.Runtime.Architecture
{
    public readonly struct EnemyDiedSignal
    {
        public EnemyDiedSignal(int enemyId)
        {
            EnemyId = enemyId;
        }

        public int EnemyId { get; }
    }

    public readonly struct BonusPoppedSignal
    {
        public BonusPoppedSignal(BonusController bonus)
        {
            Bonus = bonus;
        }

        public BonusController Bonus { get; }
    }

    public readonly struct GameFinishedSignal
    {
        public GameFinishedSignal(Panels panel)
        {
            Panel = panel;
        }

        public Panels Panel { get; }
    }

    public readonly struct CoinsChangedSignal
    {
        public CoinsChangedSignal(int coins)
        {
            Coins = coins;
        }

        public int Coins { get; }
    }

    public readonly struct ProgressUpgradedSignal
    {
    }

    public readonly struct WeaponAmmoChangedSignal
    {
        public WeaponAmmoChangedSignal(WeaponsName weapon, int count)
        {
            Weapon = weapon;
            Count = count;
        }

        public WeaponsName Weapon { get; }
        public int Count { get; }
    }

    public readonly struct LeaderMagazineInitializedSignal
    {
        public LeaderMagazineInitializedSignal(int capacity)
        {
            Capacity = capacity;
        }

        public int Capacity { get; }
    }

    public readonly struct LeaderPatronsChangedSignal
    {
        public LeaderPatronsChangedSignal(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }

    public readonly struct BarricadeHealthChangedSignal
    {
        public BarricadeHealthChangedSignal(float normalizedHealth)
        {
            NormalizedHealth = normalizedHealth;
        }

        public float NormalizedHealth { get; }
    }

    public readonly struct GrenadeDamageSignal
    {
        public GrenadeDamageSignal(Vector3 position, int damage, float radius)
        {
            Position = position;
            Damage = damage;
            Radius = radius;
        }

        public Vector3 Position { get; }
        public int Damage { get; }
        public float Radius { get; }
    }

    public readonly struct AudioSettingsChangedSignal
    {
    }

    public readonly struct UIPanelReplaceSignal
    {
        public UIPanelReplaceSignal(UIPanelController panel)
        {
            Panel = panel;
        }

        public UIPanelController Panel { get; }
    }
}
